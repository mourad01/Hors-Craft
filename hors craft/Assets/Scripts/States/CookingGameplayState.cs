// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingGameplayState
using Common.Crosspromo;
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Cooking;
using Gameplay;
using GameUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	public class CookingGameplayState : XCraftUIState<CookingGameplayStateConnector>
	{
		public GameObject tutorialPrefab;

		public GameObject tutorialRecipePrefab;

		public GameObject orderPanel;

		public GameObject infoPanelPrefab;

		public Sprite jacob;

		public AudioClip specialCustomerCompleted;

		public AudioClip specialProgressFill;

		public Sprite specialDoneBorder;

		public Color SpecialDoneColor;

		private WorkController workController;

		private CookingTutorialGenerator tutorialGenerator;

		private float specialProgress;

		private int previousTimeLeft;

		public bool isTutorialOrMinigame
		{
			get;
			set;
		}

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		private bool shownTrashTutorial
		{
			get
			{
				return PlayerPrefs.GetInt("trash.tutorial") == 1;
			}
			set
			{
				PlayerPrefs.SetInt("trash.tutorial", value ? 1 : 0);
			}
		}

		private bool shownShowRecipeTutorial
		{
			get
			{
				return PlayerPrefs.GetInt("recipe.tutorial.shown") == 1;
			}
			set
			{
				PlayerPrefs.SetInt("recipe.tutorial.shown", value ? 1 : 0);
			}
		}

		private void ShowCrosspromo()
		{
			Manager.Get<CrosspromoManager>().TryToShowIconAt(Manager.Get<CanvasManager>().canvas, this, new Vector2(1f, 0.75f));
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			workController = Manager.Get<CookingManager>().workController;
			workController.Init(this);
			Manager.Get<CookingManager>().HideTopBar();
			base.connector.onPauseButton = OnPause;
			TimeScaleHelper.value = 1f;
			PlayerInputInfo.inputArea = base.connector.inputButton;
			UpdateConnector();
			ShowCrosspromo();
			tutorialPrefab.GetComponent<GenericTutorial>().infoPanelPrefab = tutorialRecipePrefab;
			tutorialRecipePrefab.GetComponent<RecipeTutorial>().jacobsImage.sprite = jacob;
		}

		public void ShowFillIngredientsGO()
		{
			OnFillIngredients();
		}

		public CookingOrderPanel ShowCustomerOrder(Customer customer)
		{
			List<Product> products = customer.products;
			Transform orderPanelPlace = customer.GetOrderPanelPlace();
			Vector2 v = workController.mainCam.WorldToScreenPoint(orderPanelPlace.position);
			GameObject spawnedOrder = Object.Instantiate(orderPanel);
			spawnedOrder.transform.SetParent(base.connector.transform);
			spawnedOrder.transform.localScale = Vector3.one;
			spawnedOrder.transform.position = v;
			CookingOrderPanel component = spawnedOrder.GetComponent<CookingOrderPanel>();
			component.timer.Init(customer.waitTime, Timer.Colors.NONE, Timer.Colors.GREEN);
			component.timer.onFinish = delegate
			{
				UnityEngine.Object.Destroy(spawnedOrder);
			};
			foreach (Product product in customer.products)
			{
				Product tempProduct = product;
				component.AddProduct(delegate
				{
					if (customer.CanTap(workController.worker, customer.products.IndexOf(tempProduct)))
					{
						workController.worker.AddToQueue(customer);
					}
				}, product.sprite, product.Key);
				if (!workController.workData.WasProductTutorialShown(products[customer.products.IndexOf(tempProduct)].Key))
				{
					tutorialGenerator = new CookingTutorialGenerator(this, tutorialPrefab);
					isTutorialOrMinigame = true;
					tutorialGenerator.ShowTutorial(customer, products[customer.products.IndexOf(tempProduct)]);
					clearOrders(customer, product, component);
					return component;
				}
				if (!shownShowRecipeTutorial)
				{
					tutorialGenerator = new CookingTutorialGenerator(this, tutorialPrefab);
					isTutorialOrMinigame = true;
					tutorialGenerator.ShowRecipeTutorial(spawnedOrder.transform.GetChild(0).gameObject, products[customer.products.IndexOf(tempProduct)]);
					shownShowRecipeTutorial = true;
					clearOrders(customer, product, component);
					return component;
				}
			}
			return component;
		}

		private void clearOrders(Customer customer, Product product, CookingOrderPanel orderPanel)
		{
			foreach (Product product2 in customer.products)
			{
				if (product2 != product)
				{
					orderPanel.RemoveProduct(product2);
				}
			}
			customer.products.Clear();
			customer.products.Add(product);
		}

		public void MoveToConnector(GameObject go)
		{
			go.transform.SetParent(base.connector.transform, worldPositionStays: true);
		}

		public void ShowRecipeTutorial(Product product)
		{
			CookingTutorialGenerator.ShowRecipe(product, infoPanelPrefab);
		}

		public void EndTutorial()
		{
			tutorialGenerator = null;
			isTutorialOrMinigame = false;
			if (!shownTrashTutorial)
			{
				tutorialGenerator = new CookingTutorialGenerator(this, tutorialPrefab);
				isTutorialOrMinigame = true;
				tutorialGenerator.ShowTrashTutorial();
				shownTrashTutorial = true;
			}
		}

		public override void UpdateState()
		{
			base.UpdateState();
			int num = Mathf.RoundToInt(workController.wave.timeLimit - workController.wave.timer);
			if (Mathf.Abs(num - previousTimeLeft) >= 1)
			{
				UpdateConnector();
				previousTimeLeft = num;
			}
		}

		private void UpdateConnector()
		{
			base.connector.clientsLeft.text = (workController.wave.customers.Count - workController.wave.servedCustomers.Count).ToString();
			int num = Mathf.RoundToInt(workController.wave.timeLimit - workController.wave.timer);
			base.connector.timeLeft.text = num.ToString();
			base.connector.currentEarnings.text = workController.wave.moneyCollected.ToString();
			base.connector.goalEarnings.text = workController.wave.bonusGoal.moneyRequired.ToString();
		}

		private void OnPause()
		{
			Manager.Get<StateMachineManager>().PushState<CookingPauseState>();
		}

		private void OnFillIngredients()
		{
			Manager.Get<StateMachineManager>().PushState<CookingPauseState>();
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				description = "Watch ad to refill ingredients instantly!",
				translationKey = "cooking.watch.ad.to.refill",
				type = AdsCounters.None,
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.defaultText = "ok";
					componentInChildren.translationKey = "menu.ok";
					componentInChildren.ForceRefresh();
				},
				onSuccess = delegate
				{
					RefillIngredients();
				},
				reason = StatsManager.AdReason.COOKING_RENEW_ITEMS
			});
		}

		private void RefillIngredients()
		{
			List<BaseIngredientDevice> devicesOfType = workController.workPlace.GetDevicesOfType<BaseIngredientDevice>();
			devicesOfType.ForEach(delegate(BaseIngredientDevice d)
			{
				d.Refill();
			});
		}

		public void EndWave()
		{
			Manager.Get<StateMachineManager>().SetState<CookingAfterStageState>();
		}

		public void FillContinouslySlider(float change)
		{
			StartCoroutine(FillContinouslySliderCoroutine(change));
		}

		private IEnumerator FillContinouslySliderCoroutine(float change)
		{
			float stepChange = change / 30f;
			base.connector.audioSource.clip = specialProgressFill;
			if (!base.connector.audioSource.isPlaying)
			{
				base.connector.audioSource.Play();
			}
			for (int i = 0; i <= 30; i++)
			{
				base.connector.specialProgressSlider.value += stepChange;
				if (!base.connector.audioSource.isPlaying)
				{
					base.connector.audioSource.Play();
				}
				yield return null;
			}
			base.connector.audioSource.Stop();
			if (base.connector.specialProgressSlider.value == 1f)
			{
				base.connector.ActivateOnSliderFill.SetActive(value: true);
				base.connector.borderSprite.sprite = specialDoneBorder;
				base.connector.borderSprite.color = SpecialDoneColor;
				base.connector.audioSource.clip = specialCustomerCompleted;
				base.connector.audioSource.Play();
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			ShowCrosspromo();
		}
	}
}
