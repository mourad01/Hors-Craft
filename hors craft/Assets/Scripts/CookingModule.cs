// DecompilerFi decompiler from Assembly-CSharp.dll class: CookingModule
using Common.Managers;
using Common.Model;
using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingModule : ModelModule
{
	private string keyCookingEnabled()
	{
		return "cooking.enabled";
	}

	private string keyStartWithCooking()
	{
		return "cooking.on.start";
	}

	private string keyLeaveButtonEnabled()
	{
		return "cooking.leave.button.enabled";
	}

	private string keyBaseXPFactor()
	{
		return "cooking.xp.factor";
	}

	private string keyDevicePrestigeRequired(string device)
	{
		return "cooking.device.prestige.req." + device;
	}

	private string keyDeviceUnlockPrice(string device)
	{
		return "cooking.device.unlock.price." + device;
	}

	private string keyDeviceBaseUpgradePrice(string device)
	{
		return "cooking.device.base.upgrade.price." + device;
	}

	private string keyDeviceUpgradePriceIncrease(string device)
	{
		return "cooking.device.upgrade.price.increase." + device;
	}

	private string keyDeviceUpgradeBaseValue(string device, string upgrade)
	{
		return "cooking.device.upgrade.base." + device + "." + upgrade;
	}

	private string keyDeviceUpgradeValueIncrease(string device, string upgrade)
	{
		return "cooking.device.upgrade.increase." + device + "." + upgrade;
	}

	private string keyDeviceMaxUpgrade(string device)
	{
		return "cooking.device.upgrade.max." + device;
	}

	private string keyRestockPrice()
	{
		return "cooking.restock.price";
	}

	private string keyMoneyPerAd()
	{
		return "cooking.money.per.ad";
	}

	private string keyBaseCustomersNumber()
	{
		return "cooking.customers.base";
	}

	private string keyMaxCustomersNumber()
	{
		return "cooking.customers.max";
	}

	private string keyCustomersPerLevel()
	{
		return "cooking.customers.per.level";
	}

	private string keyBonusGoalMoneyPerCustomer()
	{
		return "cooking.bonus.money.per.customer";
	}

	private string keyBonusGoalMoneyPerLevel()
	{
		return "cooking.bonus.money.per.level";
	}

	private string keyBonusGoalPrestigePercent()
	{
		return "cooking.bonus.prestige.percent";
	}

	private string keyBonusGoalMoneyPercent()
	{
		return "cooking.bonus.money.percent";
	}

	private string keyMoneyForSpecialCustomer()
	{
		return "cooking.money.for.special.customer";
	}

	private string keyWaveTimePerCustomer()
	{
		return "cooking.wave.time.per.customer";
	}

	private string keyWaveTimeMultiplierPerLevel()
	{
		return "cooking.wave.time.multi.per.level";
	}

	private string keySpawnTimePerCustomer()
	{
		return "cooking.spawn.time.per.customer";
	}

	private string keySpawnTimeMultiModifierPerLevel()
	{
		return "cooking.spawn.time.modifier.per.level";
	}

	private string keySpawnTimeDeviation()
	{
		return "cooking.spawn.time.deviation";
	}

	private string keyTwoOrdersAvailableAfter()
	{
		return "cooking.two.orders.available.after";
	}

	private string keytThreeOrdersAvailableAfter()
	{
		return "cooking.three.orders.available.after";
	}

	private string keyTwoOrdersChance()
	{
		return "cooking.two.orders.chance";
	}

	private string keyThreeOrdersChance()
	{
		return "cooking.three.orders.chance";
	}

	private string keyEnableMultipleOrders()
	{
		return "cooking.enable.multiple.orders";
	}

	private string keyNumberOfChapters()
	{
		return "cooking.number.of.chapters";
	}

	private string keyChapterLevels(int id)
	{
		return "cooking.chapter.levels." + id.ToString();
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyCookingEnabled(), defaultValue: true);
		descriptions.AddDescription(keyStartWithCooking(), defaultValue: false);
		descriptions.AddDescription(keyLeaveButtonEnabled(), !SymbolsHelper.isIOS);
		descriptions.AddDescription(keyBaseXPFactor(), 0.1f);
		descriptions.AddDescription(keyRestockPrice(), 50);
		descriptions.AddDescription(keyMoneyPerAd(), 500);
		descriptions.AddDescription(keyBaseCustomersNumber(), 3);
		descriptions.AddDescription(keyMaxCustomersNumber(), 10);
		descriptions.AddDescription(keyCustomersPerLevel(), 0.15f);
		descriptions.AddDescription(keyBonusGoalMoneyPerCustomer(), 20);
		descriptions.AddDescription(keyBonusGoalMoneyPerLevel(), 0.01f);
		descriptions.AddDescription(keyBonusGoalPrestigePercent(), 0.25f);
		descriptions.AddDescription(keyBonusGoalMoneyPercent(), 0.5f);
		descriptions.AddDescription(keyMoneyForSpecialCustomer(), 50);
		descriptions.AddDescription(keyWaveTimePerCustomer(), 60f);
		descriptions.AddDescription(keyWaveTimeMultiplierPerLevel(), -0.05f);
		descriptions.AddDescription(keySpawnTimePerCustomer(), 10f);
		descriptions.AddDescription(keySpawnTimeMultiModifierPerLevel(), -0.05f);
		descriptions.AddDescription(keySpawnTimeDeviation(), 2f);
		descriptions.AddDescription(keyTwoOrdersAvailableAfter(), 5);
		descriptions.AddDescription(keytThreeOrdersAvailableAfter(), 10);
		descriptions.AddDescription(keyTwoOrdersChance(), 0.2f);
		descriptions.AddDescription(keyThreeOrdersChance(), 0.1f);
		descriptions.AddDescription(keyEnableMultipleOrders(), defaultValue: true);
		descriptions.AddDescription(keyNumberOfChapters(), 3);
		descriptions.AddDescription(keyChapterLevels(1), 10);
		descriptions.AddDescription(keyChapterLevels(2), 25);
		descriptions.AddDescription(keyChapterLevels(3), 15);
		if (Manager.Contains<CookingManager>())
		{
			List<string> allDevicesKeys = Manager.Get<CookingManager>().GetAllDevicesKeys();
			foreach (string item in allDevicesKeys)
			{
				switch (item)
				{
				case "pizza.oven":
				case "pizza.base":
				case "tomato":
				case "chopping.block":
					descriptions.AddDescription(keyDevicePrestigeRequired(item), 0);
					descriptions.AddDescription(keyDeviceUnlockPrice(item), 0);
					break;
				default:
					descriptions.AddDescription(keyDevicePrestigeRequired(item), 2);
					descriptions.AddDescription(keyDeviceUnlockPrice(item), 50);
					break;
				}
				descriptions.AddDescription(keyDeviceBaseUpgradePrice(item), 100);
				descriptions.AddDescription(keyDeviceUpgradePriceIncrease(item), 10f);
				descriptions.AddDescription(keyDeviceMaxUpgrade(item), 50);
				IEnumerator enumerator2 = Enum.GetValues(typeof(Device.UpgradeEffect)).GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string upgrade = Misc.CustomNameToKey(((Device.UpgradeEffect)enumerator2.Current).ToString());
						descriptions.AddDescription(keyDeviceUpgradeBaseValue(item, upgrade), 10f);
						descriptions.AddDescription(keyDeviceUpgradeValueIncrease(item, upgrade), 1f);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator2 as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
	}

	public override void OnModelDownloaded()
	{
		if (Manager.Contains<CookingManager>())
		{
			Manager.Get<CookingManager>().UpdateSettings();
		}
	}

	public float SpawnTimeDeviation()
	{
		return base.settings.GetFloat(keySpawnTimeDeviation());
	}

	public float SpawnTimeMultiModifierPerLevel()
	{
		return base.settings.GetFloat(keySpawnTimeMultiModifierPerLevel());
	}

	public float SpawnTimePerCustomer()
	{
		return base.settings.GetFloat(keySpawnTimePerCustomer());
	}

	public float WaveTimeMultiplierPerLevel()
	{
		return base.settings.GetFloat(keyWaveTimeMultiplierPerLevel());
	}

	public float WaveTimePerCustomer()
	{
		return base.settings.GetFloat(keyWaveTimePerCustomer());
	}

	public float BonusGoalMoneyPercent()
	{
		return base.settings.GetFloat(keyBonusGoalMoneyPercent());
	}

	public float BonusGoalPrestigePercent()
	{
		return base.settings.GetFloat(keyBonusGoalPrestigePercent());
	}

	public int BonusGoalMoneyPerCustomer()
	{
		return base.settings.GetInt(keyBonusGoalMoneyPerCustomer());
	}

	public float BonusGoalMoneyPerLevel()
	{
		return base.settings.GetFloat(keyBonusGoalMoneyPerLevel());
	}

	public int MoneyForSpecialCustomer()
	{
		return base.settings.GetInt(keyMoneyForSpecialCustomer());
	}

	public float CustomersPerLevel()
	{
		return base.settings.GetFloat(keyCustomersPerLevel());
	}

	public int MaxCustomersNumber()
	{
		return base.settings.GetInt(keyMaxCustomersNumber());
	}

	public int BaseCustomersNumber()
	{
		return base.settings.GetInt(keyBaseCustomersNumber());
	}

	public int MoneyPerAd()
	{
		return base.settings.GetInt(keyMoneyPerAd());
	}

	public int GetRestockPrice()
	{
		return base.settings.GetInt(keyRestockPrice());
	}

	public int GetDevicePrestigeRequirement(string device)
	{
		return base.settings.GetInt(keyDevicePrestigeRequired(device));
	}

	public int GetDeviceUnlockPrice(string device)
	{
		return base.settings.GetInt(keyDeviceUnlockPrice(device));
	}

	public int GetDeviceMaxUpgradeLevel(string device)
	{
		return base.settings.GetInt(keyDeviceMaxUpgrade(device)) - 1;
	}

	public int GetUpgradePrice(string device, int upgrade)
	{
		int @int = base.settings.GetInt(keyDeviceBaseUpgradePrice(device));
		float @float = base.settings.GetFloat(keyDeviceUpgradePriceIncrease(device));
		float num = 1f + @float * (float)upgrade / 100f;
		return Mathf.RoundToInt((float)@int * num);
	}

	public float GetUpgradeValue(string device, string upgrade, int level)
	{
		float @float = base.settings.GetFloat(keyDeviceUpgradeBaseValue(device, upgrade));
		float float2 = base.settings.GetFloat(keyDeviceUpgradeValueIncrease(device, upgrade));
		float num = 1f + float2 * (float)level / 100f;
		return @float * num;
	}

	public float GetBaseExpFactor()
	{
		return base.settings.GetFloat(keyBaseXPFactor());
	}

	public bool IsCookingEnabled()
	{
		return base.settings.GetBool(keyCookingEnabled());
	}

	public bool IsCookingOnStart()
	{
		return base.settings.GetBool(keyStartWithCooking());
	}

	public bool IsLeaveButtonEnabled()
	{
		return base.settings.GetBool(keyLeaveButtonEnabled());
	}

	public int GetTwoOrdersAvailableAfter()
	{
		return base.settings.GetInt(keyTwoOrdersAvailableAfter());
	}

	public int GetThreeOrdersAvailableAfter()
	{
		return base.settings.GetInt(keytThreeOrdersAvailableAfter());
	}

	public float GetTwoOrdersChance()
	{
		return base.settings.GetFloat(keyTwoOrdersChance());
	}

	public float GetThreeOrdersChance()
	{
		return base.settings.GetFloat(keyThreeOrdersChance());
	}

	public bool IsMultipleOrdersEnabled()
	{
		return base.settings.GetBool(keyEnableMultipleOrders());
	}

	public int GetNumberOfChapters()
	{
		return base.settings.GetInt(keyNumberOfChapters());
	}

	public int GetNumberOfLevelsForChapter(int chapterNumber)
	{
		return base.settings.GetInt(keyChapterLevels(chapterNumber));
	}
}
