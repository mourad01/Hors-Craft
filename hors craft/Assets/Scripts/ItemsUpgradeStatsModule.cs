// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemsUpgradeStatsModule
using Common.Managers;
using Common.Model;

public class ItemsUpgradeStatsModule : ModelModule
{
	private string doubleDpsKey => "double.dps.time";

	private string levelStatsUpgradeKey(string id, int level)
	{
		return "items.upgrade.level." + id + "." + level.ToString();
	}

	private string levelOverUpgradeBaseKey(string id)
	{
		return "items.upgrade.over." + id;
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
	}

	public override void OnModelDownloaded()
	{
	}

	public float GetUpgradeStats(string id, int level, int defaultValue)
	{
		if (ModelSettingsHelper.HasField(base.settings, levelStatsUpgradeKey(id, level)))
		{
			return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, levelStatsUpgradeKey(id, level), defaultValue);
		}
		if (ModelSettingsHelper.HasField(base.settings, levelOverUpgradeBaseKey(id)))
		{
			return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, levelOverUpgradeBaseKey(id), defaultValue);
		}
		return defaultValue;
	}

	public int GetDoubleDPSTime(int defaultValue)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, doubleDpsKey, defaultValue);
	}
}
