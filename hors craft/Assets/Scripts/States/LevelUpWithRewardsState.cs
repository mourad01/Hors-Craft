// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LevelUpWithRewardsState
using Common.Managers;
using Common.Managers.States;
using System.Collections.Generic;
using System.Linq;

namespace States
{
	public class LevelUpWithRewardsState : XCraftUIState<LevelUpWithRewardsStateConnector>
	{
		public List<LevelupRewardConfig> rewardConfigs = new List<LevelupRewardConfig>();

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onOk = OnOkButton;
			base.connector.Init(GetRewards());
		}

		private List<LevelUpRewardItemData> GetRewards()
		{
			int level = Manager.Get<ProgressManager>().level;
			return (from r in rewardConfigs
				from d in r.GetRewards(level)
				select d).ToList();
		}

		private void OnOkButton()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
