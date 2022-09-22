// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.LootChest
using Common.Gameplay;
using Common.Managers;
using System.Collections.Generic;

namespace Gameplay
{
	public class LootChest
	{
		public List<Reward> rewards = new List<Reward>();

		public LootChestManager.Rarity rarity;

		public void Claim()
		{
			Manager.Get<StatsManager>().ChestOpened(rarity.ToString().ToLower());
			foreach (Reward reward in rewards)
			{
				reward.ClaimReward();
			}
		}
	}
}
