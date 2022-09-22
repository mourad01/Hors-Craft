// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingBeforeStageStateConnector
using Common.Behaviours;
using Common.Managers.States.UI;
using Cooking;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CookingBeforeStageStateConnector : UIConnector
	{
		public Transform foodPreviewParent;

		public GameObject foodPreviewPrefab;

		public Text todaysGoal;

		public Text levelNumber;

		public Text restockFoodPrice;

		public GameObject[] buttonIcons;

		public GameObject[] additionalButtonIcons;

		public GameObject fullyRestockedGO;

		public GameObject notFullyRestockedGO;

		public Button restockButton;

		public Button startButton;

		public Button rankingButton;

		public Button upgradesButton;

		public Button leaveButton;

		public Button devAddGoldAndPrestige;

		public Action onRestockButton;

		public Action onStartButton;

		public Action onRankingButton;

		public Action onUpgradesButton;

		public Action onLeaveButton;

		private void Awake()
		{
			restockButton.onClick.AddListener(delegate
			{
				onRestockButton();
			});
			startButton.onClick.AddListener(delegate
			{
				onStartButton();
			});
			rankingButton.onClick.AddListener(delegate
			{
				onRankingButton();
			});
			upgradesButton.onClick.AddListener(delegate
			{
				onUpgradesButton();
			});
			leaveButton.onClick.AddListener(delegate
			{
				onLeaveButton();
			});
			if (MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper)
			{
				devAddGoldAndPrestige.gameObject.SetActive(value: true);
				devAddGoldAndPrestige.onClick.AddListener(delegate
				{
					UnityEngine.Object.FindObjectOfType<WorkController>().AddGoldAndPrestige();
				});
			}
		}

		public void ClearFoodPreviewList()
		{
			while (foodPreviewParent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(foodPreviewParent.GetChild(0).gameObject);
			}
		}

		public GameObject AddUnlockedFoodPreview(Sprite sprite, int count, int max)
		{
			FoodPreviewItem foodPreviewItem = SpawnFoodPreviewItem();
			foodPreviewItem.InitUnlocked(sprite, count, max);
			return foodPreviewItem.gameObject;
		}

		public GameObject AddLockedFoodPreview(Sprite sprite, int prestige)
		{
			FoodPreviewItem foodPreviewItem = SpawnFoodPreviewItem();
			foodPreviewItem.InitLocked(sprite, prestige);
			return foodPreviewItem.gameObject;
		}

		private FoodPreviewItem SpawnFoodPreviewItem()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(foodPreviewPrefab, foodPreviewParent, worldPositionStays: false);
			return gameObject.GetComponent<FoodPreviewItem>();
		}
	}
}
