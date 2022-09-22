// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldsSettingsModule
using Common.Managers;
using Common.Model;
using System;

public class WorldsSettingsModule : ModelModule
{
	public Action OnModelDownload;

	private string keyChangeWorldEnabled()
	{
		return "worlds.enabled";
	}

	private string keyFreeWorlds()
	{
		return "worlds.freeWorlds";
	}

	private string keyNewWorldCost()
	{
		return "worlds.worldsCost";
	}

	private string keyPaymentType()
	{
		return "worlds.paymentType";
	}

	private string keyNewWorldCostCurrency()
	{
		return "worlds.worldsCost.currency";
	}

	private string keyWorldHiddenCrafting()
	{
		return "worlds.crafting.hidden";
	}

	private string keyWorldsUltimateSelection()
	{
		return "worlds.ultimate.selection";
	}

	private string keyWorldsStartingCurrency()
	{
		return "worlds.starting.currency";
	}

	private string keyCostOfBlockDefault()
	{
		return "currency.ultimate.costblock";
	}

	private string keyCostOfClothesDefault()
	{
		return "currency.ultimate.costclothes";
	}

	private string keyWorldQuestEnabled()
	{
		return "worlds.quest.enabled";
	}

	private string keyWorldQuestUnlimited()
	{
		return "worlds.quest.unlimited";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyChangeWorldEnabled(), 1);
		descriptions.AddDescription(keyFreeWorlds(), 3);
		descriptions.AddDescription(keyNewWorldCost(), 1);
		descriptions.AddDescription(keyPaymentType(), 0);
		descriptions.AddDescription(keyNewWorldCostCurrency(), 100);
		descriptions.AddDescription(keyWorldHiddenCrafting(), 0);
		descriptions.AddDescription(keyWorldsUltimateSelection(), 0);
		descriptions.AddDescription(keyCostOfBlockDefault(), 10);
		descriptions.AddDescription(keyCostOfClothesDefault(), 200);
		descriptions.AddDescription(keyWorldsStartingCurrency(), 1450);
		descriptions.AddDescription(keyWorldQuestEnabled(), defaultValue: false);
		descriptions.AddDescription(keyWorldQuestUnlimited(), defaultValue: false);
	}

	public override void OnModelDownloaded()
	{
		if (IsThisGameUltimate())
		{
			Manager.Get<UltimateSoftCurrencyManager>().OnModelDownloaded(GetWorldsStartCurrency());
		}
		if (Manager.Contains<AbstractSoftCurrencyManager>())
		{
			Manager.Get<AbstractSoftCurrencyManager>().InitializeAtModelDownloaded();
		}
		if (OnModelDownload != null)
		{
			OnModelDownload();
		}
	}

	public int GetFreeWorlds()
	{
		if (base.contextIAP.isAdsFree)
		{
			return 9999;
		}
		return base.settings.GetInt(keyFreeWorlds());
	}

	public ItemsUnlockModel GetUnlockType()
	{
		return (ItemsUnlockModel)base.settings.GetInt(keyPaymentType(), 0);
	}

	public int GetNewWorldCostCurrency()
	{
		if (base.contextIAP.isAdsFree)
		{
			return 0;
		}
		return base.settings.GetInt(keyNewWorldCostCurrency(), 100);
	}

	public bool AreWorldQuestEnabled()
	{
		return base.settings.GetBool(keyWorldQuestEnabled(), defaultValue: false);
	}

	public bool AreWorldQuestUnlimited()
	{
		return base.settings.GetBool(keyWorldQuestUnlimited(), defaultValue: true);
	}

	public bool GetWorldsEnabled()
	{
		return (base.settings.GetInt(keyChangeWorldEnabled()) != 0) ? true : false;
	}

	public bool GetCraftRecipesPerWorldEnabled()
	{
		return (base.settings.GetInt(keyWorldHiddenCrafting()) != 0) ? true : false;
	}

	public bool GetWorldsUltimateSelection()
	{
		return (base.settings.GetInt(keyWorldsUltimateSelection()) != 0) ? true : false;
	}

	public int GetCostOfClothesDefault()
	{
		return base.settings.GetInt(keyCostOfClothesDefault());
	}

	public int GetCostOfBlockDefault()
	{
		return base.settings.GetInt(keyCostOfBlockDefault());
	}

	public int GetWorldsStartCurrency()
	{
		return base.settings.GetInt(keyWorldsStartingCurrency());
	}

	public bool IsThisGameUltimate()
	{
		return GetWorldsUltimateSelection();
	}

	public int GetNewWorldCost()
	{
		if (base.contextIAP.isAdsFree)
		{
			return 0;
		}
		return base.settings.GetInt(keyNewWorldCost());
	}
}
