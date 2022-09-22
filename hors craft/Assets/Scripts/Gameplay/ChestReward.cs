// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ChestReward
using Common.Gameplay;
using Common.Managers;
using States;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class ChestReward : Reward
	{
		public LootChestManager.Rarity rarity;

		public override void ClaimReward()
		{
			if (Manager.Get<StateMachineManager>().IsCurrentStateA<DailyRewardsState>())
			{
				DailyRewardsState stateInstance = Manager.Get<StateMachineManager>().GetStateInstance<DailyRewardsState>();
				stateInstance.OnFinish = (Action)Delegate.Combine(stateInstance.OnFinish, new Action(Open));
			}
			else
			{
				Open();
			}
		}

		public void Open()
		{
			LootChest chest = Manager.Get<LootChestManager>().GenerateChest(rarity);
			Manager.Get<LootChestManager>().OpenChest(chest);
		}

		public override List<Sprite> GetSprites()
		{
			return baseSprite.AsList();
		}
	}
}
