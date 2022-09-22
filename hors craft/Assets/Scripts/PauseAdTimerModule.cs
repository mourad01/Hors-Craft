// DecompilerFi decompiler from Assembly-CSharp.dll class: PauseAdTimerModule
using Common.Managers;
using Common.Model;

public class PauseAdTimerModule : ModelModule
{
	private string keyPauseAdTimerEnabled()
	{
		return "pause.ad.timer.enabled";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyPauseAdTimerEnabled(), defaultValue: false);
	}

	public override void OnModelDownloaded()
	{
	}

	public bool GetAdTimerEnabled()
	{
		return base.settings.GetBool(keyPauseAdTimerEnabled());
	}
}
