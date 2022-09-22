// DecompilerFi decompiler from Assembly-CSharp.dll class: SunModule
using Common.Managers;
using Common.Model;

public class SunModule : ModelModule
{
	private string secondsInFullDayKey => "sun.second.full.day";

	private string nightComingKey => "sun.night.coming";

	private string nightStartKey => "sun.night.start";

	private string dayComingKey => "sun.day.coming";

	private string dayStartKey => "sun.day.start";

	private string gameStartTimeOfDayKey => "game.start.time.of.day";

	public override void FillModelDescription(ModelDescription descriptions)
	{
	}

	public override void OnModelDownloaded()
	{
		SunController instance = SunController.instance;
		if (instance != null)
		{
			instance.InitFromModel();
		}
	}

	public float GetSecondsInFullDay(float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, secondsInFullDayKey, defaultValue);
	}

	public float GetNightComing(float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, nightComingKey, defaultValue);
	}

	public float GetNightStart(float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, nightStartKey, defaultValue);
	}

	public float GetDayComing(float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, dayComingKey, defaultValue);
	}

	public float GetDayStart(float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, dayStartKey, defaultValue);
	}

	public float GetGameStartTime(float defaultValue)
	{
		return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, gameStartTimeOfDayKey, defaultValue);
	}
}
