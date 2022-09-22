// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.LootChestManager
using Common.Gameplay;
using Common.Managers;
using Common.Utils;
using ItemVInventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class LootChestManager : Manager
	{
		[Serializable]
		public class RarityTier
		{
			public Rarity rarity;

			public List<ItemDefinition> items;

			public List<Reward> rewards;
		}

		public enum Rarity
		{
			COMMON = 0,
			RARE = 1,
			EPIC = 2,
			LEGENDARY = 3,
			MONEY_BAG = 999
		}

		public List<RarityTier> raritiesDropConfig = new List<RarityTier>();

		public override void Init()
		{
		}

		public void InitFreeChest()
		{
			AutoRefreshingStock.InitStockItem("free.chest", Manager.Get<ModelManager>().lootSettings.GetFreeChestEveryHours() * 60 * 60, 1, 1);
		}

		public LootChest GenerateChest(Rarity rarity)
		{
			LootChest lootChest = new LootChest();
			lootChest.rewards = new List<Reward>();
			lootChest.rarity = rarity;
			LootModule.LootConfig lootConfig = GetLootConfig(rarity);
			HashSet<ItemDefinition> hashSet = new HashSet<ItemDefinition>(raritiesDropConfig.FirstOrDefault((RarityTier r) => r.rarity == rarity).items);
			HashSet<Reward> hashSet2 = new HashSet<Reward>(raritiesDropConfig.FirstOrDefault((RarityTier r) => r.rarity == rarity).rewards);
			for (int i = 0; i < lootConfig.itemCount; i++)
			{
				ItemDefinition itemDefinition = hashSet.Random();
				hashSet.Remove(itemDefinition);
				ItemReward itemReward = ScriptableObject.CreateInstance<ItemReward>();
				itemReward.item = itemDefinition;
				itemReward.amount = lootConfig.itemAmount;
				itemReward.dropOnWorld = false;
				itemReward.baseSprite = itemDefinition.itemSprite;
				lootChest.rewards.Add(itemReward);
			}
			foreach (Reward item in hashSet2)
			{
				lootChest.rewards.Add(item);
			}
			if (Manager.Contains<CraftSoftCurrencyManager>() && Manager.Get<ShopManager>().currencies.FirstOrDefault((ShopManager.CurrencyItem c) => c.id == "soft") != null)
			{
				CurrencyReward currencyReward = ScriptableObject.CreateInstance<CurrencyReward>();
				currencyReward.amount = lootConfig.goldAmount;
				currencyReward.key = "soft";
				currencyReward.currency = (Manager.Get<ShopManager>().GetCurrency("soft") as CurrencyScriptableObject);
				lootChest.rewards.Add(currencyReward);
			}
			return lootChest;
		}

		public void OpenChest(LootChest chest)
		{
			AbstractLootDropBehaviour componentInChildren = GetComponentInChildren<AbstractLootDropBehaviour>();
			componentInChildren.Drop(chest);
		}

		private LootModule.LootConfig GetLootConfig(Rarity rarity)
		{
			string key = "chest." + rarity.ToString().ToLower();
			return Manager.Get<ModelManager>().lootSettings.GetLootConfig(key);
		}
	}
}
