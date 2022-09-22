// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AchievementsModule
using Common.Model;

namespace Common.Managers
{
	public class AchievementsModule : ModelModule
	{
		private string keyAchievementsEnabled()
		{
			return "achievements.enabled";
		}

		private string GetKeyAchievementRewardAmount(int group, int step)
		{
			return "achievement.reward.amount.{0}.{1}".Formatted(group, step);
		}

		private string GetKeyAchievementRewardType(int group, int step)
		{
			return "achievement.reward.type.{0}.{1}".Formatted(group, step);
		}

		private string GetKeyAchievementNeededToUnlock(string key, int index)
		{
			return "achievement.needed.to.unlock.{0}.{1}".Formatted(key, index);
		}

		private string GetKeyAchievementToastWaitTime()
		{
			return "achievement.toast.wait.time";
		}

		private string keyAchievementEnabled(string key)
		{
			return "achievement.enabled." + key;
		}

		private string keyAchievementGroup(string key)
		{
			return "achievement.group." + key;
		}

		private string keyAchievementGroupSwap(int old)
		{
			return "achievement.group.swap." + old;
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(GetKeyAchievementToastWaitTime(), 5f);
			descriptions.AddDescription(keyAchievementsEnabled(), 1);
		}

		public override void OnModelDownloaded()
		{
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<AbstractAchievementManager>().SetupAchievements();
			}
		}

		public bool IsAchievementsEnabled()
		{
			return base.settings.GetInt(keyAchievementsEnabled()) == 1;
		}

		public int GetAchievementRewardAmount(int group, int step)
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, GetKeyAchievementRewardAmount(group, step), 1);
		}

		public int GetAchievementRewardType(int group, int step)
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, GetKeyAchievementRewardType(group, step), 1);
		}

		public float GetToastWaitTime()
		{
			return base.settings.GetFloat(GetKeyAchievementToastWaitTime());
		}

		public float GetAchievementNeededToUnlock(string key, int index, float def)
		{
			return ModelSettingsHelper.GetFloatFromStringSettings(base.settings, GetKeyAchievementNeededToUnlock(key, index), def);
		}

		public bool HasStep(string key, int index)
		{
			return ModelSettingsHelper.HasField(base.settings, GetKeyAchievementNeededToUnlock(key, index));
		}

		public bool IsAchievementEnabled(string key)
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAchievementEnabled(key)) == 1;
		}

		public int GetAchievementGroup(string key)
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAchievementGroup(key));
		}

		public int GetAchievementGroupSwap(int group)
		{
			if (ModelSettingsHelper.HasField(base.settings, keyAchievementGroupSwap(group)))
			{
				return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAchievementGroupSwap(group));
			}
			return -1;
		}
	}
}
