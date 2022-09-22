// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.IronSourceAdWaterfallStep
using Common.Managers;

/*namespace Common.Waterfall
{
	public class IronSourceAdWaterfallStep : IAdWaterfallStep
	{
		private IronSourceManager ironSourceManager => Manager.Get<IronSourceManager>();

		private StatsManager statsManager => Manager.Get<StatsManager>();

		public void HideBanner()
		{
			ironSourceManager.HideBanner();
		}

		public void ShowBanner(string adTag, OnShowAdFailed onShowAdFailed)
		{
			ironSourceManager.ShowBanner();
		}

		public void ShowInterstitialAd(string adTag, OnShowAdFailed onShowAdFailed)
		{
			if (ironSourceManager.isInterstitialReady)
			{
				ironSourceManager.ShowInterstitial();
			}
			else
			{
				onShowAdFailed();
			}
		}

		public void ShowRewardedAd(string adTag, OnShowRewardedAdCompleted onShowRewardedAdCompleted, OnShowAdFailed onShowAdFailed)
		{
			if (ironSourceManager.isRewardedReady)
			{
				ironSourceManager.ShowRewarded(delegate(bool completed)
				{
					onShowRewardedAdCompleted(completed);
					statsManager.AdShownSuccessBasedOnCallbacks("ironsource");
				});
			}
			else
			{
				onShowAdFailed();
			}
		}
	}
}*/
