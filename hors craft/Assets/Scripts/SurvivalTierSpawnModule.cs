// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalTierSpawnModule
using Common.Managers;
using Common.Model;
using System;

public class SurvivalTierSpawnModule : ModelModule
{
	public Action onDownload;

	private string baseHPKey(int indexInTier)
	{
		return "survival.spawn.base.hp." + indexInTier.ToString();
	}

	private string baseDmgKey(int indexInTier)
	{
		return "survival.spawn.base.dmg." + indexInTier.ToString();
	}

	private string tierStartWave(int tier)
	{
		return "survival.spawn.tier.start." + tier.ToString();
	}

	private string tierModifierKey(int tier)
	{
		return "survival.spawn.modifier." + tier.ToString();
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
	}

	public override void OnModelDownloaded()
	{
		if (onDownload != null)
		{
			onDownload();
		}
	}

	public float GetBaseHP(int index, float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, baseHPKey(index), defaultValue);
	}

	public float GetBaseDmg(int index, float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, baseDmgKey(index), defaultValue);
	}

	public int GetStartWave(int tier)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, tierStartWave(tier), tier);
	}

	public float GetTierModifier(int tier)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, tierModifierKey(tier), tier);
	}
}
