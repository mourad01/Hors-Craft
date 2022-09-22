// DecompilerFi decompiler from Assembly-CSharp.dll class: AdsHandler
using Common.Managers;
using Common.Waterfall;
using System;
using UnityEngine;

//public static class AdsHandler
//{
	/*public class ShowAdResult
	{
		public bool adWasShown;
	}

	public static bool shownRewarded;

	public static ShowAdResult TryToShowVideo(string contextName, bool verbose = false)
	{
		AllInOneAdRequirementsModule.HasToShowAdOutput hasToShowAdOutput = Manager.Get<AbstractModelManager>().allInOneAdRequirements.HasToShowAd(contextName);
		ShowAdResult showAdResult;
		if (hasToShowAdOutput.enabled)
		{
			AdWaterfall.get.ShowInterstitialAd(contextName);
			Manager.Get<StatsManager>().AdShown();
			showAdResult = new ShowAdResult();
			showAdResult.adWasShown = true;
			return showAdResult;
		}
		if (verbose)
		{
			UnityEngine.Debug.Log("Show Video Fail. Reason: " + hasToShowAdOutput.disabledReason);
		}
		showAdResult = new ShowAdResult();
		showAdResult.adWasShown = false;
		return showAdResult;
	}

	public static ShowAdResult TryToShowRewardedVideo(OnShowRewardedAdCompleted onRewardedCompleted = null)
	{
		ShowAdResult showAdResult;
		if (Manager.Get<AbstractModelManager>().commonAdSettings.ShowInterstitialInsteadOfRewarded())
		{
			AdWaterfall.get.ShowInterstitialAd("Rewarded");
			Manager.Get<StatsManager>().AdShown();
			showAdResult = new ShowAdResult();
			showAdResult.adWasShown = true;
			return showAdResult;
		}
		OnShowRewardedAdCompleted thatDelegate = delegate(bool result)
		{
			if (result)
			{
				Manager.Get<StatsManager>().AdShown();
			}
			onRewardedCompleted(result);
		};
		Action onFail = delegate
		{
			thatDelegate(watchedAdTillTheEnd: false);
		};
		AdWaterfall.get.ShowRewardedAd("Rewarded", onRewardedCompleted, onFail);
		showAdResult = new ShowAdResult();
		showAdResult.adWasShown = true;
		return showAdResult;
	}

	public static ShowAdResult TryToShowBanner(bool verbose = false)
	{
		AllInOneAdRequirementsModule.HasToShowAdOutput hasToShowAdOutput = Manager.Get<AbstractModelManager>().allInOneAdRequirements.HasToShowAd("banner");
		ShowAdResult showAdResult;
		if (hasToShowAdOutput.enabled)
		{
			AdWaterfall.get.ShowBanner("banner");
			showAdResult = new ShowAdResult();
			showAdResult.adWasShown = true;
			return showAdResult;
		}
		if (verbose)
		{
			UnityEngine.Debug.Log("Show Banner Fail. Reason: " + hasToShowAdOutput.disabledReason);
		}
		showAdResult = new ShowAdResult();
		showAdResult.adWasShown = false;
		return showAdResult;
	}

	public static void HideBanner()
	{
		AdWaterfall.get.HideBanner();
	}
}
	*/