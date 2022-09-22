// DecompilerFi decompiler from Assembly-CSharp.dll class: GrowthModule
using Common.Managers;
using Common.Model;

public class GrowthModule : ModelModule
{
	private string keyGrowthDuration(string key, int stage)
	{
		return "growth.duration." + key + "." + stage;
	}

	private string keyFastForwardTime()
	{
		return "growth.skip.time.amount";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyFastForwardTime(), 10800f);
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetGrowthDuration(string key, int stage, int defaultDuration)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyGrowthDuration(key, stage), defaultDuration);
	}

	public float SkipTimeAmount()
	{
		return base.settings.GetFloat(keyFastForwardTime());
	}
}
