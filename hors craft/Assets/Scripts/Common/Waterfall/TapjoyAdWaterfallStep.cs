// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.TapjoyAdWaterfallStep
using Common.Managers;

namespace Common.Waterfall
{
	public class TapjoyAdWaterfallStep : IAdWaterfallStep
	{
		private string placementSuffix;

		private TapjoyManager tapjoyManager => Manager.Get<TapjoyManager>();

		public TapjoyAdWaterfallStep(string placementSuffix = "")
		{
			this.placementSuffix = placementSuffix;
		}

		public void ShowInterstitialAd(string adTag, OnShowAdFailed onShowAdFailed)
		{
			if (Manager.Get<AbstractModelManager>().commonAdSettings.UsesSimplifiedTapjoyTags())
			{
				adTag = "Common";
			}
			if (!TryToShowPlacement(adTag))
			{
				onShowAdFailed();
			}
		}

		public void ShowRewardedAd(string adTag, OnShowRewardedAdCompleted onShowRewardedAdCompleted, OnShowAdFailed onShowAdFailed)
		{
			if (Manager.Get<AbstractModelManager>().commonAdSettings.UsesSimplifiedTapjoyTags())
			{
				adTag = "Rewarded";
			}
			if (!TryToShowPlacement(adTag))
			{
				onShowAdFailed();
			}
			else
			{
				onShowRewardedAdCompleted(watchedAdTillTheEnd: true);
			}
		}

		public void ShowBanner(string adTag, OnShowAdFailed onShowAdFailed)
		{
			onShowAdFailed();
		}

		public void HideBanner()
		{
		}

		private bool TryToShowPlacement(string placementId)
		{
			placementId += placementSuffix;
			return tapjoyManager.ShowPlacement(placementId);
		}

		public override string ToString()
		{
			return "TapjoyAdWaterfallStep with suffix: " + placementSuffix + ((!Manager.Get<AbstractModelManager>().commonAdSettings.UsesSimplifiedTapjoyTags()) ? string.Empty : " [simplified]");
		}
	}
}
