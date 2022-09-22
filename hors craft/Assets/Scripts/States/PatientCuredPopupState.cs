// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PatientCuredPopupState
using Common.Managers;
using Common.Managers.States;

namespace States
{
	public class PatientCuredPopupState : XCraftUIState<PatientCuredPopupStateConnector>
	{
		protected PatientCuredPopupStateStartParameter startParameter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			startParameter = (parameter as PatientCuredPopupStateStartParameter);
			base.connector.prestigeEarnedText.text = startParameter.prestigeEarnedValue;
			base.connector.moneyEarnedText.text = startParameter.moneyEarnedValue;
			base.connector.onOkButtonClicked = OnReturn;
			base.connector.onRewardButtonClicked = OnReward;
		}

		private void OnReturn()
		{
			Manager.Get<HospitalManager>().CleanUp();
			Manager.Get<StateMachineManager>().PopState();
		}

		private void OnReward()
		{
			Manager.Get<RewardedAdsManager>().ShowRewardedAd(StatsManager.AdReason.XCRAFT_DOUBLE_REWARD, delegate(bool success)
			{
				if (success)
				{
					WatchedDesiredAd();
				}
			});
		}

		private void WatchedDesiredAd()
		{
			Manager.Get<HospitalManager>().WatchedRewardedAd();
			Manager.Get<HospitalManager>().CleanUp();
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
