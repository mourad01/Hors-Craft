// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemsUpgradeRequirementsModule
using Common.Managers;
using Common.Model;

public class ItemsUpgradeRequirementsModule : ModelModule
{
	private string levelStatsUpgradeKey(int level)
	{
		return "items.upgrade.requirement." + level.ToString();
	}

	private string levelOverUpgradeBaseKey()
	{
		return "items.upgrade.requirement.over";
	}

	private string currencyNeedKey(int level)
	{
		return "items.upgrade.requirement.currency." + level.ToString();
	}

	private string currencyOverNeedKey()
	{
		return "items.upgrade.requirement.currency.over";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetUpgradeStats(int level, int defaultValue)
	{
		if (ModelSettingsHelper.HasField(base.settings, levelStatsUpgradeKey(level)))
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, levelStatsUpgradeKey(level), defaultValue);
		}
		if (ModelSettingsHelper.HasField(base.settings, levelOverUpgradeBaseKey()))
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, levelOverUpgradeBaseKey(), defaultValue);
		}
		return defaultValue;
	}

	public int GetCurrencyRequired(int level, int defaultValue)
	{
		if (ModelSettingsHelper.HasField(base.settings, currencyNeedKey(level)))
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, currencyNeedKey(level), defaultValue);
		}
		if (ModelSettingsHelper.HasField(base.settings, currencyOverNeedKey()))
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, currencyOverNeedKey(), defaultValue);
		}
		return defaultValue;
	}
}
