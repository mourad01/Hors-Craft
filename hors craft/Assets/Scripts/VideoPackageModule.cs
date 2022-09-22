// DecompilerFi decompiler from Assembly-CSharp.dll class: VideoPackageModule
using Common.Managers;
using Common.Model;

public class VideoPackageModule : ModelModule
{
	private string keyMaxNumberOfAds()
	{
		return "ads.video.currency.maxnumber";
	}

	private string keyTimeToResetCounter()
	{
		return "ads.video.currency.time";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyMaxNumberOfAds(), 10);
		descriptions.AddDescription(keyTimeToResetCounter(), 60);
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetMaxNumberOfAds()
	{
		return base.settings.GetInt(keyMaxNumberOfAds(), 10);
	}

	public int TimeToResetCounter()
	{
		return base.settings.GetInt(keyTimeToResetCounter(), 30);
	}
}
