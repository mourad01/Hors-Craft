// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.GlobalBannerModule
using Common.Model;
using Common.Utils;
using Gameplay;
using UnityEngine;

//namespace Common.Managers
//{
	/*public class GlobalBannerModule : ModelModule
	{
		private string keyGlobalBannerRequirementMinSessionsId()
		{
			return "global.banner.minsessions";
		}

		private string keyGlobalBannerRequirementProbabilityID()
		{
			return "global.banner.probability";
		}

		private string keyBannerEnabledFor(string state)
		{
			return "banner.enabled." + state;
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyGlobalBannerRequirementMinSessionsId(), 0);
			descriptions.AddDescription(keyGlobalBannerRequirementProbabilityID(), 1f);
		}

		public override void OnModelDownloaded()
		{
			if (base.settings.GetFloat(keyGlobalBannerRequirementProbabilityID()) == 0f)
			{
				MoveIfBannerEnabled.BannerDisabled();
			}
		}

		public bool HasToShowGlobalBanner()
		{
			int @int = base.settings.GetInt(keyGlobalBannerRequirementMinSessionsId());
			float @float = base.settings.GetFloat(keyGlobalBannerRequirementProbabilityID());
			if (base.context.isAdsFree)
			{
				return false;
			}
			return RequirementsFulfilled(@int, @float);
		}

		public bool BannerEnabled(string state)
		{
			string key = keyBannerEnabledFor(state);
			if (base.settings.HasBool(key))
			{
				return base.settings.GetBool(key);
			}
			return true;
		}

		private bool RequirementsFulfilled(int minSessions, float probability)
		{
			int sessionNo = PlayerSession.GetSessionNo();
			if (sessionNo <= minSessions)
			{
				return false;
			}
			if (Random.value > probability)
			{
				return false;
			}
			return true;
		}
	}
}
	*/