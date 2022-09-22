// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemsDropModule
using Common.Managers;
using Common.Model;
using ItemVInventory;

public class ItemsDropModule : ModelModule
{
	private string baseDropKey(int index)
	{
		return "items.drop.base." + index.ToString();
	}

	private string tierModifierKey(int tier)
	{
		return "items.drop.modifier.tier." + tier.ToString();
	}

	private string baseCurrencyDropKey(int index)
	{
		return "items.drop.currency.base." + index.ToString();
	}

	private string tierCurrencyModifierKey(int tier)
	{
		return "items.drop.currency.modifier.tier." + tier.ToString();
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
	}

	public override void OnModelDownloaded()
	{
		if (Manager.Contains<ItemsManager>())
		{
			Manager.Get<ItemsManager>().InitDropTable();
		}
	}

	public bool HasBaseIndex(int index)
	{
		return ModelSettingsHelper.HasField(base.settings, baseDropKey(index));
	}

	public int GetBaseDrop(int index, int defaultValue)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, baseDropKey(index), defaultValue);
	}

	public int GetBaseCurrencyDrop(int index, int defaultValue)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, baseCurrencyDropKey(index), defaultValue);
	}

	public bool HasTierIndex(int index)
	{
		return ModelSettingsHelper.HasField(base.settings, tierModifierKey(index));
	}

	public float GetTierModifier(int tier, float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, tierModifierKey(tier), defaultValue);
	}

	public float GetTierCurrencyModifier(int tier, float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, tierCurrencyModifierKey(tier), defaultValue);
	}
}
