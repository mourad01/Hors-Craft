// DecompilerFi decompiler from Assembly-CSharp.dll class: LootModule
using Common.Managers;
using Common.Model;
using Gameplay;
using Uniblocks;
using UnityEngine;

public class LootModule : ModelModule
{
	public class LootConfig
	{
		public int itemCount;

		public int goldAmount;

		public int itemAmountMin;

		public int itemAmountMax;

		public int itemAmount => Random.Range(itemAmountMin, itemAmountMax + 1);
	}

	private const string PREFIX = "loot.";

	private string keyShopEnabled()
	{
		return "shop.enabled";
	}

	private string keyItemCount(string key)
	{
		return "loot.count." + key;
	}

	private string keyItemAmountMin(string key)
	{
		return "loot.item.amount.min." + key;
	}

	private string keyItemAmountMax(string key)
	{
		return "loot.item.amount.max." + key;
	}

	private string keyGoldAmountMin(string key)
	{
		return "loot.gold.min." + key;
	}

	private string keyGoldAmountMax(string key)
	{
		return "loot.gold.max." + key;
	}

	private string keyBlocksAmountMin(Voxel.RarityCategory voxelCategory, LootChestManager.Rarity chestCategory)
	{
		return "loot.blocks.min." + voxelCategory.ToString().ToLower() + "." + chestCategory.ToString().ToLower();
	}

	private string keyBlocksAmountMax(Voxel.RarityCategory voxelCategory, LootChestManager.Rarity chestCategory)
	{
		return "loot.blocks.max." + voxelCategory.ToString().ToLower() + "." + chestCategory.ToString().ToLower();
	}

	private string keyBlocksPercentChance(Voxel.RarityCategory voxelCategory, LootChestManager.Rarity chestCategory)
	{
		return "loot.blocks.percent.chance." + voxelCategory.ToString().ToLower() + "." + chestCategory.ToString().ToLower();
	}

	private string keyResourcesAmountMin(LootChestManager.Rarity chestCategory)
	{
		return "loot.resources.min." + chestCategory.ToString().ToLower();
	}

	private string keyResourcesAmountMax(LootChestManager.Rarity chestCategory)
	{
		return "loot.resources.max." + chestCategory.ToString().ToLower();
	}

	private string keyResourcesDifferentAmountMin(LootChestManager.Rarity chestCategory)
	{
		return "loot.resources.different.min." + chestCategory.ToString().ToLower();
	}

	private string keyResourcesDifferentAmountMax(LootChestManager.Rarity chestCategory)
	{
		return "loot.resources.different.max." + chestCategory.ToString().ToLower();
	}

	private string keyHardCurrencyAmountMin(LootChestManager.Rarity chestCategory)
	{
		return "loot.hard.currency.min." + chestCategory.ToString().ToLower();
	}

	private string keyHardCurrencyAmountMax(LootChestManager.Rarity chestCategory)
	{
		return "loot.hard.currency.max." + chestCategory.ToString().ToLower();
	}

	private string keyFreeChestEveryHours()
	{
		return "free.chest.every.hours";
	}

	private string keyFreeHcEnabled()
	{
		return "free.hc.enabled";
	}

	private string keyFreeHcMaxCount()
	{
		return "free.hc.max.count";
	}

	private string keyFreeHcCooldown()
	{
		return "free.hc.cooldown";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyShopEnabled(), 0);
		descriptions.AddDescription(keyFreeChestEveryHours(), 8);
		descriptions.AddDescription(keyFreeHcEnabled(), 0);
		descriptions.AddDescription(keyFreeHcMaxCount(), 3);
		descriptions.AddDescription(keyFreeHcCooldown(), 8);
	}

	public override void OnModelDownloaded()
	{
		if (Manager.Contains<LootChestManager>())
		{
			Manager.Get<LootChestManager>().InitFreeChest();
		}
	}

	public bool IsShopEnabled()
	{
		return base.settings.GetInt(keyShopEnabled()) == 1;
	}

	public int GetBlocksAmountMin(Voxel.RarityCategory voxelCategory, LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyBlocksAmountMin(voxelCategory, chestRarity));
	}

	public int GetBlocksAmountMax(Voxel.RarityCategory voxelCategory, LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyBlocksAmountMax(voxelCategory, chestRarity));
	}

	public float GetBlocksPercentChance(Voxel.RarityCategory voxelCategory, LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyBlocksPercentChance(voxelCategory, chestRarity));
	}

	public int GetResourcesAmountMin(LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyResourcesAmountMin(chestRarity));
	}

	public int GetResourcesAmountMax(LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyResourcesAmountMax(chestRarity));
	}

	public int GetResourcesDifferentAmountMin(LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyResourcesDifferentAmountMin(chestRarity));
	}

	public int GetResourcesDifferentAmountMax(LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyResourcesDifferentAmountMax(chestRarity));
	}

	public int GetHardCurrencyAmountMin(LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyHardCurrencyAmountMin(chestRarity));
	}

	public int GetHardCurrencyAmountMax(LootChestManager.Rarity chestRarity)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyHardCurrencyAmountMax(chestRarity));
	}

	public int GetFreeChestEveryHours()
	{
		return base.settings.GetInt(keyFreeChestEveryHours());
	}

	public bool IsFreeHcEnabled()
	{
		return false;
	}

	public int GetFreeHcMaxCount()
	{
		return base.settings.GetInt(keyFreeHcMaxCount());
	}

	public int GetFreeHcCooldown()
	{
		return base.settings.GetInt(keyFreeHcCooldown()) * 60 * 60;
	}

	public LootConfig GetLootConfig(string key)
	{
		LootConfig lootConfig = new LootConfig();
		lootConfig.itemCount = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyItemCount(key));
		lootConfig.itemAmountMin = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyItemAmountMin(key));
		lootConfig.itemAmountMax = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyItemAmountMax(key));
		int intFromStringSettings = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyGoldAmountMin(key));
		int intFromStringSettings2 = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyGoldAmountMax(key));
		lootConfig.goldAmount = Random.Range(intFromStringSettings, intFromStringSettings2 + 1);
		return lootConfig;
	}
}
