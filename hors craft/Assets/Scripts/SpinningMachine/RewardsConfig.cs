// DecompilerFi decompiler from Assembly-CSharp.dll class: SpinningMachine.RewardsConfig
using UnityEngine;

namespace SpinningMachine
{
	public class RewardsConfig : ScriptableObject
	{
		[SerializeField]
		private RewardSettings[] _resourcesId;

		[SerializeField]
		private RewardsSteps _rewardsStepsAlwaysMax;

		[SerializeField]
		private RewardsSteps[] _rewardsStepsOther;

		public int minToWin => _rewardsStepsOther[0].minCount;

		public int slotsCount => _resourcesId.Length;

		public RewardSettings RewardSettings(int winValue)
		{
			return _resourcesId[winValue];
		}

		public int WinCount(SpinableMachineController.WinType winType, int inRowCount = 0)
		{
			if (winType == SpinableMachineController.WinType.AlwaysMax)
			{
				return _rewardsStepsAlwaysMax.rewardCount;
			}
			return GetRewardsCount(inRowCount);
		}

		private int GetRewardsCount(int inRow)
		{
			for (int num = _rewardsStepsOther.Length - 1; num >= 0; num--)
			{
				if (_rewardsStepsOther[num].minCount <= inRow)
				{
					return _rewardsStepsOther[num].rewardCount;
				}
			}
			return 0;
		}
	}
}
