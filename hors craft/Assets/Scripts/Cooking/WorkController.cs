// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.WorkController
using Common.Managers;
using States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public class WorkController : MonoBehaviour
	{
		[HideInInspector]
		public CookingLevel choosenLevel;

		public Transform minigameCameraPivot;

		public TimeDeviceSlot device;

		public GameObject successPrefab;

		public GameObject minigameGraphicsPrefab;

		public float tapStrength = 3f;

		public float defaultProgress;

		public float opponentCD = 0.2f;

		public float opponentTapStrength = 0.05f;

		public Wave wave
		{
			get;
			private set;
		}

		public WorkPlace workPlace
		{
			get;
			private set;
		}

		public WorkData workData
		{
			get;
			private set;
		}

		public Worker worker
		{
			get;
			private set;
		}

		public CustomerSpawner customerSpawner
		{
			get;
			private set;
		}

		public RecipesList recipesList
		{
			get;
			private set;
		}

		public CookingGameplayState cookingGameplay
		{
			get;
			private set;
		}

		public Camera mainCam
		{
			get;
			private set;
		}

		public void Init(CookingGameplayState cooking)
		{
			cookingGameplay = cooking;
		}

		private void Awake()
		{
			workPlace = GetComponentInChildren<WorkPlace>();
			workData = GetComponentInChildren<WorkData>();
			worker = GetComponentInChildren<Worker>();
			customerSpawner = GetComponentInChildren<CustomerSpawner>();
			recipesList = GetComponentInChildren<RecipesList>();
			mainCam = GetComponentInChildren<Camera>();
			customerSpawner.Init(this);
		}

		private void Update()
		{
			if (cookingGameplay != null && !cookingGameplay.isTutorialOrMinigame)
			{
				wave.timer += Time.deltaTime;
			}
			if (wave != null && wave.isDone && Manager.Get<StateMachineManager>().currentState is CookingGameplayState)
			{
				FinishWave();
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Semicolon))
			{
				AddGoldAndPrestige();
			}
		}

		public void StartMinigame()
		{
			worker.ShowGraphics(show: false);
			MashToFillBarCookingGraphics component = minigameGraphicsPrefab.GetComponent<MashToFillBarCookingGraphics>();
			component.cameraPivot = minigameCameraPivot;
			component.device = device;
			component.successPrefab = successPrefab;
			Manager.Get<StateMachineManager>().PushState<TappingGameState>(new TappingGameStateStartParameter
			{
				graphicPrefab = minigameGraphicsPrefab,
				gameBehaviour = new MashToFillBarCookingGame(tapStrength, defaultProgress, opponentCD, opponentTapStrength)
			});
		}

		public void GenerateWave()
		{
			wave = WaveGenerator.GenerateWave(choosenLevel.levelNumber);
		}

		public void StartWave()
		{
			customerSpawner.InitWave(wave);
			ResetKitchen();
		}

		public void ResetKitchen()
		{
			List<StorageDevice> devicesOfType = workPlace.GetDevicesOfType<StorageDevice>();
			foreach (StorageDevice item in devicesOfType)
			{
				StorageDeviceSlot[] slots = item.slots;
				foreach (StorageDeviceSlot storageDeviceSlot in slots)
				{
					storageDeviceSlot.Reset();
				}
			}
			while (worker.heldItems.Count > 0)
			{
				IPickable pickable = worker.DisposeHeldItem(worker.heldItems[0]);
				UnityEngine.Object.Destroy(pickable.GetGameObject());
			}
			worker.ResetQueue();
			workPlace.InitDevices();
		}

		public void DestroyCustomers()
		{
			List<Customer> list = UnityEngine.Object.FindObjectsOfType<Customer>().ToList();
			foreach (Customer item in list)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}

		private void FinishWave()
		{
			workData.wave++;
			workData.money += wave.moneyCollected;
			wave.prestigeCollected = Mathf.RoundToInt(wave.prestigeCollected);
			workData.prestige += wave.prestigeCollected;
			if (wave.moneyCollected >= wave.bonusGoal.moneyRequired)
			{
				workData.money += wave.bonusGoal.bonusMoney;
				workData.prestige += wave.bonusGoal.bonusPrestige;
			}
			int num = wave.customers.Count;
			foreach (CustomerServedInfo servedCustomer in wave.servedCustomers)
			{
				if (servedCustomer.positive)
				{
					num--;
				}
			}
			int num2 = 0;
			if (num == 0)
			{
				num2 = 3;
			}
			else if (num <= 2)
			{
				num2 = 2;
			}
			else if (num < wave.customers.Count / 2)
			{
				num2 = 1;
			}
			if (num2 > choosenLevel.numberOfStars)
			{
				workData.SetLevelStars(choosenLevel.levelNumber, num2);
				workData.UnlockLevel(choosenLevel.levelNumber + 1);
			}
			choosenLevel.numberOfStars = num2;
			ResetKitchen();
			DestroyCustomers();
			cookingGameplay.EndWave();
			customerSpawner.EndWave();
		}

		public void LeaveWave()
		{
			ResetKitchen();
			DestroyCustomers();
			customerSpawner.EndWave();
		}

		public void AddGoldAndPrestige()
		{
			workData.prestigeLevel++;
			workData.money += 1000;
			for (int i = 1; i <= 50; i++)
			{
				workData.UnlockLevel(i);
			}
		}
	}
}
