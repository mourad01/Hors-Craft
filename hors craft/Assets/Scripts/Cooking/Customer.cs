// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.Customer
using Common.Managers;
using Gameplay;
using Gameplay.Audio;
using GameUI;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Cooking
{
	public class Customer : MonoBehaviour, ITapObject
	{
		private enum Status
		{
			COMING = 0,
			WAITING = 1,
			LEAVING_WITH_MEAL = 2,
			LEAVING_WITH_ANGER = 3,
			SPECIAL = 4,
			IDLE = 999
		}

		public GameObject moneyPrefab;

		public Transform orderPivot;

		public bool randomizeSkin = true;

		public float eatingTime = 3f;

		public bool dontChangeYOnSit;

		public bool specialCustomer;

		public AudioClip clientEnterClip;

		public AudioClip clientFailedClip;

		public AudioClip getCoinsClip;

		private WorkController workController;

		private NavMeshAgent navigator;

		private Animator animator;

		private Wave wave;

		private WorkPlace.CustomerPlace customerPlace;

		private CookingOrderPanel orderPanel;

		private Vector3 startPosition;

		private Status status = Status.IDLE;

		private int money;

		public List<Product> products
		{
			get;
			private set;
		}

		public float waitTime
		{
			get;
			private set;
		}

		private Timer timer => orderPanel.timer;

		public GameObject GetGameObject()
		{
			return base.gameObject;
		}

		private void Awake()
		{
			animator = GetComponentInChildren<Animator>();
			workController = Manager.Get<CookingManager>().workController;
			if (randomizeSkin)
			{
				GetComponentInChildren<PlayerGraphic>().SetWholeBodyl(FindRandomSkin(UnityEngine.Random.value));
			}
		}

		public void Init(WorkPlace.CustomerPlace customerPlace, Wave wave, List<Product> products, float waitTime)
		{
			this.wave = wave;
			this.products = products;
			this.waitTime = waitTime;
			this.customerPlace = customerPlace;
			navigator = base.gameObject.AddComponent<NavMeshAgent>();
			navigator.radius = 0.5f;
			navigator.height = 2f;
			navigator.baseOffset = 0.85f;
			navigator.speed = 3.5f;
			navigator.angularSpeed = 240f;
			navigator.acceleration = 8f;
			navigator.stoppingDistance = 0f;
			navigator.autoBraking = true;
			navigator.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
			navigator.avoidancePriority = 50;
			navigator.autoTraverseOffMeshLink = true;
			navigator.autoRepath = true;
			status = Status.COMING;
			GoToFreeCustomerPlace(customerPlace.place.position);
		}

		private void GoToFreeCustomerPlace(Vector3 destination)
		{
			startPosition = base.transform.position;
			SetDestination(destination);
		}

		public void OnInteraction(Worker worker)
		{
			foreach (Product product in products)
			{
				Product viableProduct = worker.heldProducts.FirstOrDefault((Product p) => p.Key == product.Key);
				if (viableProduct != null)
				{
					money += CalculateMoney(viableProduct);
					viableProduct.transform.SetParent(customerPlace.moneyPlace);
					viableProduct.transform.localPosition = Vector3.zero;
					viableProduct.transform.localRotation = Quaternion.identity;
					viableProduct.transform.localScale = Vector3.one;
					products.Remove(product);
					bool served = (products.Count <= 0) ? true : false;
					status = Status.IDLE;
					animator.SetBool("eat", value: true);
					StartCoroutine(DoAfter(delegate
					{
						if (served)
						{
							LeaveHappy(money);
						}
						else
						{
							animator.SetBool("eat", value: false);
							status = Status.WAITING;
						}
						UnityEngine.Object.Destroy(viableProduct.gameObject);
					}, eatingTime));
					worker.GiveProductToCustomer(this, viableProduct);
					if (served)
					{
						SpawnMoney(money);
						UnityEngine.Object.Destroy(orderPanel.gameObject);
					}
					else
					{
						orderPanel.RemoveProduct(product);
					}
					break;
				}
			}
		}

		public Vector3 GetWorkerPlace()
		{
			return customerPlace.workerPlace.position;
		}

		public Transform GetOrderPanelPlace()
		{
			return customerPlace.orderPanelPlace;
		}

		public bool CanTap(Worker worker)
		{
			return CanTap(worker, 0);
		}

		public bool CanTap(Worker worker, int productIndex = 0)
		{
			bool flag = products.Count > 0 && worker.heldProducts.Any((Product p) => products.Any((Product otherp) => otherp.Key == p.Key));
			if (!flag && status == Status.WAITING)
			{
				workController.cookingGameplay.ShowRecipeTutorial(products[productIndex]);
			}
			return flag;
		}

		private void Update()
		{
			switch (status)
			{
			case Status.WAITING:
				UpdateWaiting();
				break;
			case Status.COMING:
				UpdateComing();
				break;
			case Status.LEAVING_WITH_MEAL:
				UpdateLeaving();
				break;
			case Status.LEAVING_WITH_ANGER:
				UpdateLeaving();
				break;
			case Status.SPECIAL:
				UpdateSpecial();
				break;
			}
			UpdateAnimator();
		}

		private void UpdateComing()
		{
			if (HasArrivedAtDestination())
			{
				if (specialCustomer)
				{
					SitDown();
					status = Status.SPECIAL;
					workController.StartMinigame();
					workController.cookingGameplay.isTutorialOrMinigame = true;
				}
				else
				{
					SitDown();
					MakeOrder();
					status = Status.WAITING;
				}
			}
		}

		private void UpdateAnimator()
		{
			animator.SetBool("sit", status == Status.WAITING || status == Status.IDLE);
			animator.SetBool("walking", status == Status.COMING || status == Status.LEAVING_WITH_ANGER || status == Status.LEAVING_WITH_MEAL);
		}

		private void SitDown()
		{
			if (dontChangeYOnSit)
			{
				Transform transform = base.transform;
				Vector3 position = customerPlace.place.position;
				float x = position.x;
				Vector3 position2 = base.transform.position;
				float y = position2.y;
				Vector3 position3 = customerPlace.place.position;
				transform.position = new Vector3(x, y, position3.z);
			}
			else
			{
				base.transform.position = customerPlace.place.position;
			}
			base.transform.rotation = customerPlace.place.rotation;
			navigator.enabled = false;
		}

		private void MakeOrder()
		{
			if (!workController.cookingGameplay.isTutorialOrMinigame)
			{
				orderPanel = workController.cookingGameplay.ShowCustomerOrder(this);
				if (clientEnterClip != null)
				{
					MixersManager.Play(clientEnterClip);
				}
			}
		}

		private void UpdateSpecial()
		{
			if (!(Manager.Get<StateMachineManager>().currentState is TappingGameState))
			{
				workController.worker.ShowGraphics(show: true);
				workController.cookingGameplay.isTutorialOrMinigame = false;
				money = Manager.Get<ModelManager>().cookingSettings.MoneyForSpecialCustomer();
				wave.moneyCollected += money;
				wave.NotifyCustomerServed(money, 1f);
			}
		}

		private void UpdateWaiting()
		{
			if (orderPanel == null)
			{
				MakeOrder();
				return;
			}
			if (timer.isDone)
			{
				LeaveAngry();
			}
			timer.pause = workController.cookingGameplay.isTutorialOrMinigame;
		}

		private void UpdateLeaving()
		{
			if (HasArrivedAtDestination())
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void LeaveAngry()
		{
			wave.NotifyCustomerLeftHungry();
			UnityEngine.Object.Destroy(orderPanel.gameObject);
			status = Status.IDLE;
			animator.SetTrigger("impatient");
			if (clientFailedClip != null)
			{
				MixersManager.Play(clientFailedClip);
			}
			StartCoroutine(DoAfter(delegate
			{
				status = Status.LEAVING_WITH_ANGER;
				LeavePlace();
			}, 1f));
		}

		private void LeaveHappy(int money)
		{
			wave.NotifyCustomerServed(money, 1f - timer.progress);
			animator.SetBool("eat", value: false);
			animator.SetTrigger("happy");
			StartCoroutine(DoAfter(delegate
			{
				status = Status.LEAVING_WITH_MEAL;
				LeavePlace();
			}, 1f));
		}

		private void LeavePlace()
		{
			customerPlace.isFree = true;
			customerPlace.customer = null;
			customerPlace = null;
			SetDestination(startPosition);
		}

		private void SetDestination(Vector3 destination)
		{
			navigator.enabled = true;
			navigator.SetDestination(destination);
		}

		private void SpawnMoney(int value)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(moneyPrefab, Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
			gameObject.transform.position = workController.mainCam.WorldToScreenPoint(orderPivot.position);
			gameObject.transform.localScale = Vector3.one;
			Text componentInChildren = gameObject.GetComponentInChildren<Text>();
			componentInChildren.text = "+" + value;
			wave.moneyCollected += value;
			if (getCoinsClip != null)
			{
				MixersManager.Play(getCoinsClip);
			}
		}

		private int CalculateMoney(Product product)
		{
			int price = product.Price;
			float num = 1f - timer.progress;
			if (num < 0.5f)
			{
				return price;
			}
			float num2 = (float)price * (num + 0.5f);
			float num3 = num2 - (float)price;
			float percentUpgradeEffectSummarized = workController.workPlace.GetPercentUpgradeEffectSummarized(Device.UpgradeEffect.TIPS);
			return Mathf.RoundToInt((float)price + num3 * percentUpgradeEffectSummarized);
		}

		private bool HasArrivedAtDestination()
		{
			if (!navigator.pathPending && navigator.remainingDistance <= navigator.stoppingDistance && (!navigator.hasPath || navigator.velocity.sqrMagnitude == 0f))
			{
				return true;
			}
			return false;
		}

		private IEnumerator DoAfter(Action action, float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			action();
		}

		private void OnDestroy()
		{
			if (orderPanel != null)
			{
				UnityEngine.Object.Destroy(orderPanel.gameObject);
			}
			if (customerPlace != null)
			{
				customerPlace.isFree = true;
				customerPlace.customer = null;
			}
		}

		private int FindRandomSkin(float random)
		{
			float num = random * SkinList.instance.sumOfWeights;
			float num2 = 0f;
			int num3 = -1;
			while (num3 < SkinList.instance.possibleSkins.Count - 1 && num2 <= num)
			{
				num3++;
				num2 += SkinList.instance.genderProbabilities[SkinList.instance.possibleSkins[num3].gender];
			}
			if (num3 < SkinList.instance.possibleSkins.Count - 1)
			{
				return num3;
			}
			return SkinList.instance.possibleSkins.Count - 1;
		}
	}
}
