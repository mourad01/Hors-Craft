// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.IronSourceManager
using Common.Ksero;
using Common.Model;
using System;
using UnityEngine;

/*{
	public class IronSourceManager : Manager
	{
		[SerializeField]
		private string appKey = string.Empty;

		private Action<bool> onShowRewardedAdCompleted;

		private bool isShowingBanner;

		private bool bannerReady;

		//public bool isInterstitialReady => IronSource.Agent.isInterstitialReady();

		//public bool isRewardedReady => IronSource.Agent.isRewardedVideoAvailable();

		public bool isBannerReady
		{
			get
			{
				return bannerReady;
			}
			private set
			{
				bannerReady = value;
			}
		}

		public bool IsBannerShown()
		{
			return isShowingBanner && isBannerReady;
		}

		/*public void LoadBanner()
		{
			IronSource.Agent.loadBanner(IronSourceBannerSize.SMART_BANNER, IronSourceBannerPosition.TOP);
		}
		
		public void DisplayBanner()
		{
			IronSource.Agent.displayBanner();
			UnityEngine.Debug.Log("IronSource: banner shown");
		}

		public void ShowInterstitial()
		{
			IronSource.Agent.showInterstitial();
			UnityEngine.Debug.Log("IronSource: interstitial shown");
		}

		public void ShowRewarded(Action<bool> onShowRewardedAdCompleted)
		{
			UnityEngine.Debug.Log("IronSource: rewarded shown");
			this.onShowRewardedAdCompleted = onShowRewardedAdCompleted;
			IronSource.Agent.showRewardedVideo();
		}

		public void ShowBanner()
		{
			isShowingBanner = true;
			if (isBannerReady)
			{
				DisplayBanner();
			}
			else
			{
				LoadBanner();
			}
		}

		public void HideBanner()
		{
			isShowingBanner = false;
			IronSource.Agent.hideBanner();
		}
		
		public override void Init()
		{
		}

		public override void OnConsentSpecified(bool consentAquired)
		{
			Settings settingsForGame = KseroFiles.GetSettingsForGame(Manager.Get<ConnectionInfoManager>().gameName);
			string value = (!settingsForGame.HasString("ironsource.api.key")) ? string.Empty : settingsForGame.GetString("ironsource.api.key").Trim();
			if (!string.IsNullOrEmpty(value))
			{
				appKey = value;
			}
			//IronSource.Agent.setConsent(consentAquired);
			//IronSource.Agent.init(appKey);
			//IronSource.Agent.setAdaptersDebug(enabled: true);
			//IronSource.Agent.validateIntegration();
			InitInterstitialEvents();
			InitRewardedEvents();
			InitBannerEvents();
			LoadInterstitial();
			UnityEngine.Debug.Log("IronSource: Init ended succesfully");
		}

		private void OnApplicationPause(bool pause)
		{
			//IronSource.Agent.onApplicationPause(pause);
		}

		private void LoadInterstitial()
		{
			//IronSource.Agent.loadInterstitial();
		}

		private void InitInterstitialEvents()
		{
			IronSourceEvents.onInterstitialAdReadyEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: InterstitialAdReady");
			};
			IronSourceEvents.onInterstitialAdShowSucceededEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: InterstitialAdSucceded");
			};
			IronSourceEvents.onInterstitialAdClickedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: InterstitialAdClicked");
			};
			IronSourceEvents.onInterstitialAdOpenedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: InterstitialAdOpened");
			};
			IronSourceEvents.onInterstitialAdLoadFailedEvent += delegate(IronSourceError ironSourceError)
			{
				UnityEngine.Debug.Log("IronSource: InterstitialAdLoadFailed " + ironSourceError.ToString());
				LoadInterstitial();
				UnityEngine.Debug.Log("IronSource: Attempting to load next interstitial after error");
			};
			IronSourceEvents.onInterstitialAdClosedEvent += delegate
			{
				LoadInterstitial();
				UnityEngine.Debug.Log("IronSource: InterstitialAdClosed and asked to load next ad");
			};
			IronSourceEvents.onInterstitialAdShowFailedEvent += delegate(IronSourceError ironSourceError)
			{
				UnityEngine.Debug.LogError("IronSource: InterstitialAdShowFailed " + ironSourceError.ToString());
				LoadInterstitial();
				UnityEngine.Debug.Log("IronSource: Attempting to load next interstitial after error");
			};
		}

		private void InitRewardedEvents()
		{
			IronSourceEvents.onRewardedVideoAdOpenedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: RewardedVideoAdOpened");
			};
			IronSourceEvents.onRewardedVideoAdClosedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: RewardedVideoAdClosed");
			};
			IronSourceEvents.onRewardedVideoAdStartedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: RewardedVideoAdStarted");
			};
			IronSourceEvents.onRewardedVideoAdEndedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: RewardedVideoAdEnded");
			};
			IronSourceEvents.onRewardedVideoAdShowFailedEvent += delegate(IronSourceError ironSourceError)
			{
				UnityEngine.Debug.LogError("IronSource: RewardedVideoAdShowFailed " + ironSourceError.ToString());
			};
			IronSourceEvents.onRewardedVideoAdRewardedEvent += delegate(IronSourcePlacement ironSourcePlacement)
			{
				UnityEngine.Debug.Log("IronSource: onRewardedVideoAdRewardedEvent for placement " + ironSourcePlacement.ToString());
				onShowRewardedAdCompleted(obj: true);
			};
			IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += delegate(bool isAdAvilable)
			{
				UnityEngine.Debug.Log("IronSource: RewardedVideoAvailabilityChanged to " + isAdAvilable);
			};
		}

		private void InitBannerEvents()
		{
			IronSourceEvents.onBannerAdLoadedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: onBannerAdLoadedEvent");
				isBannerReady = true;
				if (isShowingBanner)
				{
					//DisplayBanner();
				}
			};
			IronSourceEvents.onBannerAdClickedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: onBannerAdScreenPresentedEvent");
			};
			IronSourceEvents.onBannerAdScreenPresentedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: onBannerAdScreenPresentedEvent");
			};
			IronSourceEvents.onBannerAdScreenDismissedEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: onBannerAdScreenDismissedEvent");
				IronSource.Agent.destroyBanner();
				isBannerReady = false;
				LoadBanner();
			};
			IronSourceEvents.onBannerAdLeftApplicationEvent += delegate
			{
				UnityEngine.Debug.Log("IronSource: onBannerAdLeftApplicationEvent");
			};
			IronSourceEvents.onBannerAdLoadFailedEvent += delegate(IronSourceError ironSourceError)
			{
				UnityEngine.Debug.LogError("IronSource: onBannerAdLoadFailedEvent " + ironSourceError.ToString());
				LoadBanner();
			};
		}
	}
}
*/