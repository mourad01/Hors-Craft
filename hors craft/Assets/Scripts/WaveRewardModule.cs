// DecompilerFi decompiler from Assembly-CSharp.dll class: WaveRewardModule
using Common.Managers;
using Common.Model;

public class WaveRewardModule : ModelModule
{
	private string waveCurrencyKey(int wave)
	{
		return "survival.wave.survived.currency." + wave.ToString();
	}

	private string waveCurrencyOverKey()
	{
		return "survival.wave.survived.currency.over";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetWaveEarn(int wave)
	{
		if (ModelSettingsHelper.HasField(base.settings, waveCurrencyKey(wave)))
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, waveCurrencyKey(wave), wave);
		}
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, waveCurrencyOverKey(), wave);
	}
}
