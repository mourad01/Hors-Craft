// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingAfterStageState
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Cooking;
using Gameplay;
using UnityEngine;

namespace States
{
	public class CookingAfterStageState : XCraftUIState<CookingAfterStageStateConnector>
	{
		private const string leaderboardPrestige = "leaderboardPrestige";

		private WorkController workController;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = false;
				autoAdsConfig.properAdReason = StatsManager.AdReason.COOKING_BETWEEN_STAGE;
				return autoAdsConfig;
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			workController = Manager.Get<CookingManager>().workController;
			InitConnectorCooking();
			Manager.Get<CookingManager>().ShowTopBar();
			TimeScaleHelper.value = 0f;
			Manager.Get<StatsManager>().QuestCompleted(workController.workData.wave);
			ShowRateUsOrAd();
			if (Manager.Contains<SocialPlatformManager>())
			{
				Manager.Get<SocialPlatformManager>().social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor("leaderboardPrestige"), workController.workData.prestigeLevel);
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			Manager.Get<CookingManager>().ShowTopBar();
		}

		private void ShowRateUsOrAd()
		{
			int @int = PlayerPrefs.GetInt("timeSinceStartup", 0);
			if (Manager.Get<ModelManager>().timeBasedRateUs.HasToShowRateReminder(@int))
			{
				Manager.Get<CookingManager>().HideTopBar();
				Manager.Get<StateMachineManager>().PushState<RateUsState>();
			}
			else
			{
				//TryToShowAd();
			}
		}

		private void InitConnectorCooking()
		{
			base.connector.onOkButton = OnContinue;
			Wave wave = workController.wave;
			base.connector.SetStars(workController.choosenLevel.numberOfStars);
			TranslateText component = base.connector.level.GetComponent<TranslateText>();
			if (workController.choosenLevel.numberOfStars > 0)
			{
				component.translationKey = "cooking.level.completed";
				component.defaultText = "level {0} completed!";
			}
			else
			{
				component.translationKey = "cooking.level.failed";
				component.defaultText = "level {0} failed!";
			}
			component.AddVisitor((string t) => t.Replace("{0}", workController.choosenLevel.levelNumber.ToString()).ToString());
			base.connector.todaysGoal.text = wave.bonusGoal.moneyRequired.ToString();
			bool flag = wave.moneyCollected >= wave.bonusGoal.moneyRequired;
			base.connector.goalCompletedGO.SetActive(flag);
			base.connector.goalFailedGO.SetActive(!flag);
			base.connector.earnings.text = wave.moneyCollected.ToString();
			base.connector.goalBonusGold.text = ((!flag) ? "0" : wave.bonusGoal.bonusMoney.ToString());
			base.connector.goalBonusPrestige.text = ((!flag) ? "0" : wave.bonusGoal.bonusPrestige.ToString());
		}

		private void OnContinue()
		{
			Manager.Get<StateMachineManager>().SetState<CookingChooseLevelState>();
		}
	}
}
