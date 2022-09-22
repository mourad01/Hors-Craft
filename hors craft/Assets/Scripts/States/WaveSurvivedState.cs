// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WaveSurvivedState
using Common.Managers;
using Common.Managers.States;
using System;

namespace States
{
	public class WaveSurvivedState : XCraftUIState<WaveSurvivedStateConnector>
	{
		public class WaveSurvivedStartParameter : StartParameter
		{
			public int earnedMoney;
		}

		public Action onStart;

		private WaveSurvivedStartParameter startParam;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			if (onStart != null)
			{
				onStart();
			}
			startParam = (WaveSurvivedStartParameter)parameter;
			base.connector.earnedMoney.text = startParam.earnedMoney.ToString();
			base.connector.okButton.onClick.RemoveAllListeners();
			base.connector.okButton.onClick.AddListener(OnOkClicked);
			base.connector.rewardedButton.onClick.RemoveAllListeners();
			base.connector.rewardedButton.onClick.AddListener(OnDoubleReward);
		}

		private void OnOkClicked()
		{
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(startParam.earnedMoney);
		}

		private void OnDoubleReward()
		{
			Manager.Get<RewardedAdsManager>().ShowRewardedAd(StatsManager.AdReason.XCRAFT_DOUBLE_REWARD, delegate(bool success)
			{
				if (success)
				{
					WatchedAd();
				}
			});
		}

		private void WatchedAd()
		{
			Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(startParam.earnedMoney);
			OnOkClicked();
		}
	}
}
