// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.IAdWaterfallStep
namespace Common.Waterfall
{
	public interface IAdWaterfallStep
	{
		void ShowInterstitialAd(string adTag, OnShowAdFailed onShowAdFailed);

		void ShowRewardedAd(string adTag, OnShowRewardedAdCompleted onShowRewardedAdCompleted, OnShowAdFailed onShowAdFailed);

		void ShowBanner(string adTag, OnShowAdFailed onShowAdFailed);

		void HideBanner();
	}
}
