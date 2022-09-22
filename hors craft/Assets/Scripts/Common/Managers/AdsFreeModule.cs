// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AdsFreeModule
using Common.Model;
using System;

namespace Common.Managers
{
	public class AdsFreeModule : ModelModule
	{
		public Action onModelDownloaded;

		protected string keyAndroidAdsFreeAppURL()
		{
			return "android.adsfree.app.url";
		}

		protected string keyiOSAdsFreeAppURL()
		{
			return "ios.adsfree.app.url";
		}

		protected string keyAdsFreeEnabled()
		{
			return "adsfree.enabled";
		}

		protected string keyForceUnlockByWatchAds()
		{
			return "adsfree.force.unlock.by.watch.ads";
		}

		protected string keyWatchAdsToRemoveAds()
		{
			return "adsfree.adstowatch";
		}

		protected string keyShowButtonInWatchXAdsPopUp()
		{
			return "adsfree.show.in.watchxadspopup";
		}

		protected string keyInstertitialInsteadOfRewarded()
		{
			return "ads.interstitial.instead.rewarded";
		}

		protected string keyRemoveAdsAtPayment()
		{
			return "ads.remove.payment";
		}

		protected string keySoomlaEnabled()
		{
			return "soomla.enabled";
		}

		protected string keySoomlaAppKey()
		{
			return "soomla.app.key";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyAndroidAdsFreeAppURL(), string.Empty);
			descriptions.AddDescription(keyiOSAdsFreeAppURL(), string.Empty);
			descriptions.AddDescription(keyAdsFreeEnabled(), defaultValue: false);
			descriptions.AddDescription(keyForceUnlockByWatchAds(), defaultValue: false);
			descriptions.AddDescription(keyWatchAdsToRemoveAds(), 0);
			descriptions.AddDescription(keyInstertitialInsteadOfRewarded(), defaultValue: true);
			descriptions.AddDescription(keyRemoveAdsAtPayment(), defaultValue: true);
			descriptions.AddDescription(keyShowButtonInWatchXAdsPopUp(), defaultValue: false);
			descriptions.AddDescription(keySoomlaEnabled(), defaultValue: false);
			descriptions.AddDescription(keySoomlaAppKey(), string.Empty);
		}

		public override void OnModelDownloaded()
		{
			if (onModelDownloaded != null)
			{
				onModelDownloaded();
			}
		}

		public string GetAndroidAdsFreeAppURL()
		{
			return base.settings.GetString(keyAndroidAdsFreeAppURL());
		}

		public string GetiOSAdsFreeAppURL()
		{
			return base.settings.GetString(keyiOSAdsFreeAppURL());
		}

		public bool IsAdsFreeButtonEnabled()
		{
			return base.settings.GetBool(keyAdsFreeEnabled());
		}

		public bool IsForceUnlockByWatchAdsEnabled()
		{
			return base.settings.GetBool(keyForceUnlockByWatchAds());
		}

		public bool IsRemoveAdsButtonInWatchXAdsEnabled()
		{
			return base.settings.GetBool(keyShowButtonInWatchXAdsPopUp());
		}

		public bool IsWatchAdsToRemoveAdsEnabled()
		{
			return base.settings.GetInt(keyWatchAdsToRemoveAds()) > 0;
		}

		public int GetAdsToWatchToRemoveAds()
		{
			return base.settings.GetInt(keyWatchAdsToRemoveAds());
		}

		public bool IsInterstitialInsteadOfRewarded()
		{
			return base.settings.GetBool(keyInstertitialInsteadOfRewarded());
		}

		public bool GetRemoveAdsAfterPayment()
		{
			return base.settings.GetBool(keyRemoveAdsAtPayment());
		}

		public bool GetSoomlaEnabled()
		{
			return base.settings.GetBool(keySoomlaEnabled());
		}

		public string GetSoomlaAppKey()
		{
			return base.settings.GetString(keySoomlaAppKey());
		}
	}
}
