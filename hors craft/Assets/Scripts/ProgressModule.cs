// DecompilerFi decompiler from Assembly-CSharp.dll class: ProgressModule
using Common.Managers;
using Common.Model;

public class ProgressModule : ModelModule
{
	private string keyExperienceMultiplier()
	{
		return "player.experience.multiplier";
	}

	private string keyExperienceBase()
	{
		return "player.experience.base";
	}

	private string keyExperienceAdditive()
	{
		return "player.experience.additive";
	}

	private string keyUseAdditive()
	{
		return "player.experience.use.additive";
	}

	private string KeyExpNeededForLevel(int level)
	{
		return $"player.experience.level.{level}";
	}

	private string keyLevelRequired(string key)
	{
		return "player.level.required." + key;
	}

	private string keyExperiencePer(string key)
	{
		return "player.experience.per." + key;
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyExperienceMultiplier(), 1.15f);
		descriptions.AddDescription(keyExperienceBase(), 300);
		descriptions.AddDescription(keyExperienceAdditive(), 0);
		descriptions.AddDescription(keyUseAdditive(), defaultValue: false);
	}

	public override void OnModelDownloaded()
	{
		if (Manager.Contains<ProgressManager>())
		{
			Manager.Get<ProgressManager>().InitVariables();
		}
	}

	public bool GetUseAdditive()
	{
		return base.settings.GetBool(keyUseAdditive(), defaultValue: false);
	}

	public int GetExperienceAdditive()
	{
		return base.settings.GetInt(keyExperienceAdditive(), 0);
	}

	public float GetExperienceMultiplier()
	{
		return base.settings.GetFloat(keyExperienceMultiplier(), 1.15f);
	}

	public int GetExperienceBase()
	{
		return base.settings.GetInt(keyExperienceBase(), 15);
	}

	public bool TryGetExperienceNeededForLevel(int level, out int expNeeded)
	{
		expNeeded = ModelSettingsHelper.GetIntFromStringSettings(base.settings, KeyExpNeededForLevel(level), -1);
		return expNeeded != -1;
	}

	public int GetRequiredLevel(string key, int defaultLevel = 1)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyLevelRequired(key), defaultLevel);
	}

	public int GetExperiencePer(string key, int defaultExp = 1)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyExperiencePer(key), defaultExp);
	}
}
