// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CurrencyChestReward
using Common.Managers;
using UnityEngine;

namespace Gameplay
{
	public class CurrencyChestReward : CurrencyReward
	{
		public LootChestManager.Rarity chestCategory;

		public int minAmount = 1;

		public int maxAmount = 3;

		public override void ClaimReward()
		{
			minAmount = Manager.Get<ModelManager>().lootSettings.GetHardCurrencyAmountMin(chestCategory);
			maxAmount = Manager.Get<ModelManager>().lootSettings.GetHardCurrencyAmountMax(chestCategory);
			amount = Random.Range(minAmount, maxAmount + 1);
			base.ClaimReward();
		}
	}
}
