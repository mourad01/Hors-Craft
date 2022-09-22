// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.TimeDeviceSlot
using Common.Managers;
using Gameplay.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking
{
	public class TimeDeviceSlot : StorageDeviceSlot, ITapObject
	{
		public enum Status
		{
			IDLE,
			COOKING,
			BURNING,
			ANIMATING
		}

		public Transform timerPivot;

		public bool canBurn = true;

		public bool hasCustomMinigameAnimation;

		private Timer timer;

		private GameObject infoPanel;

		private List<ParticleSystem> particleSystems;

		private Animator animator;

		private AudioSource audioSource;

		public TimeDevice baseCookingDevice => base.baseDevice as TimeDevice;

		private Product placedProduct
		{
			get
			{
				return base.placedItem as Product;
			}
			set
			{
				base.placedItem = value;
			}
		}

		public Status status
		{
			get;
			private set;
		}

		private void Awake()
		{
			base.baseDevice = GetComponentInParent<TimeDevice>();
			workController = GetComponentInParent<WorkController>();
			particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
			animator = GetComponentInChildren<Animator>();
			audioSource = GetComponentInChildren<AudioSource>();
			status = Status.IDLE;
		}

		public override void Reset()
		{
			base.Reset();
			if (timer != null)
			{
				UnityEngine.Object.DestroyImmediate(timer.gameObject);
			}
			timer = null;
			if (infoPanel != null)
			{
				UnityEngine.Object.DestroyImmediate(infoPanel);
			}
			infoPanel = null;
			audioSource.Stop();
			status = Status.IDLE;
		}

		public override void OnInteraction(Worker worker)
		{
			if (CanPlaceSomething(worker))
			{
				StartWorking(worker);
			}
			else if (CanUseOnSomething(worker))
			{
				Product product = worker.heldProducts.First((Product p) => workController.recipesList.CanUse(p, placedProduct));
				worker.ReplaceProduct(base.baseDevice, product, placedProduct);
				UnityEngine.Object.Destroy(placedProduct.gameObject);
				placedProduct = null;
				GoIdle();
				if (infoPanel != null)
				{
					UnityEngine.Object.Destroy(infoPanel);
				}
			}
			else if (CanPickSomething(worker))
			{
				worker.PickUpProduct(placedProduct);
				placedProduct = null;
				GoIdle();
				if (infoPanel != null)
				{
					UnityEngine.Object.Destroy(infoPanel);
				}
			}
		}

		protected override bool CanPlaceSomething(Worker worker)
		{
			return placedProduct == null && worker.heldProducts.Any((Product p) => workController.recipesList.CanUse(p, base.baseDevice));
		}

		protected override bool CanPickSomething(Worker worker)
		{
			return placedProduct != null && placedProduct.canBePickedUp && worker.CanPickSomething() && status != Status.COOKING;
		}

		protected override bool CanUseOnSomething(Worker worker)
		{
			return placedProduct != null && status != Status.COOKING && worker.heldProducts.Any((Product p) => workController.recipesList.CanUse(p, placedProduct));
		}

		private void Update()
		{
			switch (status)
			{
			case Status.COOKING:
				UpdateCooking();
				break;
			case Status.BURNING:
				UpdateBurning();
				break;
			}
			if (status != 0 && placedProduct == null && status != Status.ANIMATING)
			{
				GoIdle();
			}
			UpdateVisualEffects();
		}

		private void UpdateCooking()
		{
			if (timer.isDone)
			{
				placedProduct = SpawnNewProduct(placedProduct);
				ShowInfoPanel();
				if (canBurn)
				{
					StartBurning();
				}
				else
				{
					GoIdle();
				}
			}
		}

		private void StartBurning()
		{
			if (baseCookingDevice.timerEndClip != null)
			{
				MixersManager.Play(baseCookingDevice.timerEndClip);
			}
			status = Status.BURNING;
			SpawnTimer(baseCookingDevice.GetBurnTime(), burn: true);
			audioSource.clip = baseCookingDevice.burningClip;
			audioSource.Play();
		}

		private void UpdateBurning()
		{
			if (timer.isDone)
			{
				placedProduct = SpawnNewProduct(placedProduct);
				ShowInfoPanel();
				GoIdle();
				if (baseCookingDevice.burnedClip != null)
				{
					MixersManager.Play(baseCookingDevice.burnedClip);
				}
			}
		}

		private void StartWorking(Worker worker)
		{
			placedProduct = worker.heldProducts.First((Product p) => workController.recipesList.CanUse(p, base.baseDevice));
			worker.DisposeHeldItem(placedProduct);
			if (pivot != null)
			{
				placedProduct.transform.SetParent(pivot);
				placedProduct.transform.localPosition = Vector3.zero;
			}
			else
			{
				placedProduct.gameObject.SetActive(value: false);
			}
			status = Status.COOKING;
			SpawnTimer(baseCookingDevice.GetTimeRequired(), burn: false);
			audioSource.clip = baseCookingDevice.cookingClip;
			audioSource.Play();
		}

		public void GoIdle()
		{
			DestroyTimer();
			audioSource.Stop();
			status = Status.IDLE;
		}

		public void startAnimating()
		{
			audioSource.clip = baseCookingDevice.cookingClip;
			audioSource.Play();
			status = Status.ANIMATING;
		}

		private void SpawnTimer(float time, bool burn)
		{
			DestroyTimer();
			Vector3 spacePosition = (!(timerPivot == null)) ? timerPivot.position : (base.transform.position + Vector3.up);
			timer = Manager.Get<CookingManager>().SpawnTimer(spacePosition, time, burn);
			timer.destroyOnFinish = false;
		}

		private void DestroyTimer()
		{
			if (timer != null)
			{
				UnityEngine.Object.Destroy(timer.gameObject);
			}
		}

		private Product SpawnNewProduct(Product baseProduct)
		{
			Product product = base.baseDevice.SpawnNewProduct(baseProduct);
			if (pivot != null)
			{
				product.transform.SetParent(pivot);
				product.transform.localPosition = Vector3.zero;
			}
			else
			{
				product.gameObject.SetActive(value: false);
			}
			UnityEngine.Object.Destroy(baseProduct.gameObject);
			return product;
		}

		private void ShowInfoPanel()
		{
			if (infoPanel != null)
			{
				UnityEngine.Object.Destroy(infoPanel);
			}
			GameObject infoPanelPrefab = workController.cookingGameplay.infoPanelPrefab;
			infoPanel = UnityEngine.Object.Instantiate(infoPanelPrefab, Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
			workController.cookingGameplay.MoveToConnector(infoPanel);
			infoPanel.transform.localScale = Vector3.one;
			Vector3 position = workController.mainCam.WorldToScreenPoint(pivot.position);
			infoPanel.transform.position = position;
			RecipeTutorial component = infoPanel.GetComponent<RecipeTutorial>();
			component.AddItem(placedProduct.GetSprite());
			AddButton(component.parent, delegate
			{
				if (!workController.cookingGameplay.isTutorialOrMinigame && CanTap(workController.worker))
				{
					workController.worker.AddToQueue(this);
				}
			});
		}

		private void AddButton(Transform parent, Action action)
		{
			GameObject gameObject = new GameObject("Button");
			gameObject.transform.SetParent(parent);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.localScale = Vector3.one;
			rectTransform.anchorMin = new Vector2(0.2f, 0.2f);
			rectTransform.anchorMax = new Vector2(0.8f, 0.8f);
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			image.raycastTarget = true;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.onClick.AddListener(delegate
			{
				action();
			});
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			layoutElement.ignoreLayout = true;
		}

		private void UpdateVisualEffects()
		{
			if (status == Status.COOKING || status == Status.BURNING || status == Status.ANIMATING)
			{
				if (status == Status.ANIMATING && hasCustomMinigameAnimation)
				{
					animator.SetBool("minigame", value: true);
				}
				else
				{
					animator.SetBool("working", value: true);
				}
				foreach (ParticleSystem particleSystem in particleSystems)
				{
					if (!particleSystem.isPlaying)
					{
						particleSystem.Play();
					}
				}
			}
			else
			{
				animator.SetBool("working", value: false);
				animator.SetBool("minigame", value: false);
				foreach (ParticleSystem particleSystem2 in particleSystems)
				{
					if (particleSystem2.isPlaying)
					{
						particleSystem2.Stop();
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (infoPanel != null)
			{
				UnityEngine.Object.Destroy(infoPanel);
			}
			if (timer != null)
			{
				UnityEngine.Object.Destroy(timer.gameObject);
			}
		}
	}
}
