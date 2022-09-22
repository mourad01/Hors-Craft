// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingBeforeStageState
using Common.Managers;
using Common.Managers.States;
using Cooking;
using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CookingBeforeStageState : XCraftUIState<CookingBeforeStageStateConnector>
	{
		public bool useAdditionalIcons = true;

		public bool useDefaultIcons = true;

		public GameObject additionalButtonIcon;

		public GameObject[] iconsReplacement;

		private WorkController workController;

		private int restockPrice;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			workController = Manager.Get<CookingManager>().workController;
			workController.GenerateWave();
			Manager.Get<CookingManager>().ShowTopBar();
			InitConnectorCooking();
			if (additionalButtonIcon == null)
			{
				useAdditionalIcons = false;
			}
			else
			{
				GameObject[] additionalButtonIcons = base.connector.additionalButtonIcons;
				foreach (GameObject gameObject in additionalButtonIcons)
				{
					GameObject gameObject2 = Object.Instantiate(additionalButtonIcon);
					gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
					gameObject2.transform.localScale = Vector3.one;
				}
			}
			if (iconsReplacement != null && iconsReplacement.Length == 3 && base.connector.buttonIcons.Length == 3)
			{
				for (int j = 0; j < 3 && iconsReplacement[j] != null && base.connector.buttonIcons[j] != null; j++)
				{
					GameObject gameObject3 = Object.Instantiate(iconsReplacement[j]);
					gameObject3.transform.SetParent(base.connector.buttonIcons[j].transform.parent, worldPositionStays: false);
					gameObject3.transform.localScale = Vector3.one;
					gameObject3.transform.SetAsFirstSibling();
				}
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			InitConnectorCooking();
			Manager.Get<CookingManager>().ShowTopBar();
		}

		private void InitConnectorCooking()
		{
			base.connector.onStartButton = OnStart;
			base.connector.onRankingButton = OnRankings;
			base.connector.onRestockButton = OnRestock;
			base.connector.onUpgradesButton = OnUpgrades;
			base.connector.onLeaveButton = OnLeave;
			base.connector.todaysGoal.text = workController.wave.bonusGoal.moneyRequired.ToString();
			base.connector.levelNumber.GetComponent<TranslateText>().AddVisitor((string t) => t.Replace("{0}", workController.choosenLevel.levelNumber.ToString()));
			UpdateRestockButton();
			SpawnFoodPreviews();
			base.connector.leaveButton.gameObject.SetActive(Manager.Get<ModelManager>().cookingSettings.IsLeaveButtonEnabled());
			GameObject[] buttonIcons = base.connector.buttonIcons;
			foreach (GameObject gameObject in buttonIcons)
			{
				gameObject.SetActive(useDefaultIcons);
			}
		}

		private void SpawnFoodPreviews()
		{
			base.connector.ClearFoodPreviewList();
			List<BaseIngredientDevice> devicesOfType = workController.workPlace.GetDevicesOfType<BaseIngredientDevice>();
			devicesOfType.Sort((BaseIngredientDevice wendy, BaseIngredientDevice hubert) => wendy.GetPrestigeRequired() - hubert.GetPrestigeRequired());
			for (int i = 0; i < devicesOfType.Count; i++)
			{
				BaseIngredientDevice baseIngredientDevice = devicesOfType[i];
				if (baseIngredientDevice.Unlocked())
				{
					base.connector.AddUnlockedFoodPreview(baseIngredientDevice.sprite, baseIngredientDevice.portionsLeft, baseIngredientDevice.portionsMax);
					continue;
				}
				GameObject item = base.connector.AddLockedFoodPreview(baseIngredientDevice.sprite, baseIngredientDevice.GetPrestigeRequired());
				if (workController.workData.prestigeLevel >= baseIngredientDevice.GetPrestigeRequired())
				{
					ShowExclamationMark(baseIngredientDevice, item);
				}
			}
		}

		private void ShowExclamationMark(BaseIngredientDevice device, GameObject item)
		{
			GameObject x = ExclamationMarkController.ShowExclamationMark("cooking.before.stage." + device.GetKey(), base.connector.upgradesButton.gameObject);
			if (x != null)
			{
				ExclamationMarkController.CreateDefaultMarkPlacement(item)();
				base.connector.upgradesButton.onClick.AddListener(delegate
				{
					ExclamationMarkController.ClearMarksWithKey("cooking.before.stage." + device.GetKey());
				});
			}
		}

		private void UpdateRestockButton()
		{
			restockPrice = CalculateRestockPrice();
			base.connector.restockFoodPrice.text = restockPrice.ToString();
			base.connector.fullyRestockedGO.SetActive(restockPrice == 0);
			base.connector.notFullyRestockedGO.SetActive(restockPrice != 0);
			base.connector.restockFoodPrice.GetComponentInParent<Button>().interactable = (restockPrice > 0);
		}

		private int CalculateRestockPrice()
		{
			int num = Manager.Get<ModelManager>().cookingSettings.GetRestockPrice();
			List<BaseIngredientDevice> devicesOfType = workController.workPlace.GetDevicesOfType<BaseIngredientDevice>();
			return devicesOfType.Count((BaseIngredientDevice bd) => bd.portionsLeft < bd.portionsMax) * num;
		}

		private void OnRestock()
		{
			if (workController.workData.money >= restockPrice)
			{
				workController.workData.money -= restockPrice;
				List<BaseIngredientDevice> devicesOfType = workController.workPlace.GetDevicesOfType<BaseIngredientDevice>();
				devicesOfType.ForEach(delegate(BaseIngredientDevice d)
				{
					d.Refill();
				});
				SpawnFoodPreviews();
				UpdateRestockButton();
			}
			else
			{
				string text = Manager.Get<TranslationsManager>().GetText("cooking.watch.ad", "Watch ad to get {0} coins").ToUpper();
				text = text.Replace("{0}", Manager.Get<ModelManager>().cookingSettings.MoneyPerAd().ToString());
				Manager.Get<CookingManager>().HideTopBar();
				Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
				{
					numberOfAdsNeeded = 1,
					translationKey = string.Empty,
					description = text,
					reason = StatsManager.AdReason.COOKING_CURRENCY,
					immediatelyAd = false,
					type = AdsCounters.Currency,
					onSuccess = delegate(bool b)
					{
						if (b)
						{
							Manager.Get<CookingManager>().workController.workData.money += Manager.Get<ModelManager>().cookingSettings.MoneyPerAd();
						}
					},
					configWatchButton = delegate(GameObject go)
					{
						TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
						componentInChildren.translationKey = "menu.watch";
						componentInChildren.defaultText = "watch";
						componentInChildren.ForceRefresh();
					}
				});
			}
		}

		private void OnStart()
		{
			workController.GenerateWave();
			Manager.Get<StateMachineManager>().SetState<CookingGameplayState>();
			workController.StartWave();
		}

		private void OnRankings()
		{
			Manager.Get<StateMachineManager>().PushState<RankingsState>();
		}

		private void OnUpgrades()
		{
			Manager.Get<StateMachineManager>().PushState<CookingUpgradesState>();
		}

		private void OnLeave()
		{
			Manager.Get<StateMachineManager>().SetState<CookingChooseLevelState>();
		}
	}
}
