// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LevelSkippingConfigModule
using Common.Model;

namespace Common.Managers
{
	public class LevelSkippingConfigModule : ModelModule
	{
		private string keySkipRequirementMinLevels()
		{
			return "rewardedad.minfailures";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keySkipRequirementMinLevels(), 0);
		}

		public override void OnModelDownloaded()
		{
		}

		public bool IsLevelSkippable(int levelNo)
		{
			int @int = base.settings.GetInt(keySkipRequirementMinLevels());
			return levelNo >= @int;
		}

		public int GetFailuresCountToSeeRewardedAd()
		{
			return base.settings.GetInt(keySkipRequirementMinLevels());
		}
	}
}
