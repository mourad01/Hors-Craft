// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RewardsManager
using Common.Gameplay;
using Common.Managers;
using System.Linq;

namespace Gameplay
{
	public class RewardsManager : Manager
	{
		public RewardsList rewards;

		public override void Init()
		{
		}

		public Reward GetRewardObject(string key)
		{
			return rewards.availableRewards.FirstOrDefault((Reward r) => r.key.ToLower() == key.ToLower());
		}
	}
}
