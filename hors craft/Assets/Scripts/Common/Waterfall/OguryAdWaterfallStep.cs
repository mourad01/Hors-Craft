// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.OguryAdWaterfallStep
using Common.Managers;

//namespace Common.Waterfall
//{
	/*public class OguryAdWaterfallStep : IAdWaterfallStep
	{
		private OguryManager oguryManager => Manager.Get<OguryManager>();

		private StatsManager statsManager => Manager.Get<StatsManager>();

		public void ShowInterstitialAd(string adTag, OnShowAdFailed onShowAdFailed)
		{
			if (oguryManager.IsInterstitialAvailable)
			{
				oguryManager.ShowInterstitial();
				statsManager.AdShownSuccessBasedOnCallbacks("ogury");
			}
			else
			{
				oguryManager.LoadInterstitial(0f);
				onShowAdFailed();
			}
		}

		public void ShowRewardedAd(string adTag, OnShowRewardedAdCompleted onShowRewardedAdCompleted, OnShowAdFailed onShowAdFailed)
		{
			if (oguryManager.IsRewardedAvailable)
			{
				oguryManager.ShowRewarded();
				statsManager.AdShownSuccessBasedOnCallbacks("ogury");
				onShowRewardedAdCompleted(watchedAdTillTheEnd: true);
			}
			else
			{
				oguryManager.LoadRewarded(0f);
				onShowAdFailed();
			}
		}

		public void ShowBanner(string adTag, OnShowAdFailed onShowAdFailed)
		{
			onShowAdFailed();
		}

		public void HideBanner()
		{
		}
	}
}
	*/