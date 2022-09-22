// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.CommonAdSettingsModule
using Common.Model;

namespace Common.Managers
{
	public class CommonAdSettingsModule : ModelModule
	{
		private string keyAdMobBannerFormat()
		{
			return "common.ad.settings.admob.smart.banner.size";
		}

		private string keyCrosspromoHeadersCount()
		{
			return "common.crosspromo.headers.count";
		}

		private string keyCrosspromoXButtonEnabled()
		{
			return "common.crosspromo.xbutton.enabled";
		}

		private string keySimplifiedTapjoyTags()
		{
			return "common.ad.settings.simplified.tapjoy";
		}

		private string keyCrosspromoAnimatorIndex()
		{
			return "common.crosspromo.animator.index";
		}

		private string keyShowInterstitialInsteadOfRewarded()
		{
			return "common.ad.show.interstitial.instead.of.rewarded";
		}

		private string keyRewardedWithInterstitial()
		{
			return "common.ad.rewarded.with.interstitial";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyAdMobBannerFormat(), defaultValue: true);
			descriptions.AddDescription(keyCrosspromoHeadersCount(), 10);
			descriptions.AddDescription(keyCrosspromoXButtonEnabled(), defaultValue: false);
			descriptions.AddDescription(keySimplifiedTapjoyTags(), defaultValue: false);
			descriptions.AddDescription(keyCrosspromoAnimatorIndex(), 0);
			descriptions.AddDescription(keyShowInterstitialInsteadOfRewarded(), defaultValue: false);
			descriptions.AddDescription(keyRewardedWithInterstitial(), defaultValue: false);
		}

		public override void OnModelDownloaded()
		{
		}

		public bool CanShowSmartBanners()
		{
			return base.settings.GetBool(keyAdMobBannerFormat());
		}

		public bool IsCrosspromoXButtonEnabled()
		{
			return base.settings.GetBool(keyCrosspromoXButtonEnabled());
		}

		public int GetCrosspromoHeadersCountLimit()
		{
			return base.settings.GetInt(keyCrosspromoHeadersCount());
		}

		public bool UsesSimplifiedTapjoyTags()
		{
			return base.settings.GetBool(keySimplifiedTapjoyTags());
		}

		public int GetCrosspromoAnimatorIndex()
		{
			return base.settings.GetInt(keyCrosspromoAnimatorIndex());
		}

		public bool ShowInterstitialInsteadOfRewarded()
		{
			return base.settings.GetBool(keyShowInterstitialInsteadOfRewarded());
		}

		public bool IsRewardedWithInterstitial()
		{
			return base.settings.GetBool(keyRewardedWithInterstitial());
		}
	}
}
