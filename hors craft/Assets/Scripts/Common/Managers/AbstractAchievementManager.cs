// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AbstractAchievementManager
using Common.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.Managers
{
	public abstract class AbstractAchievementManager : Manager
	{
		public AchievementsList achievementsList;

		[HideInInspector]
		public List<Achievement> configs = new List<Achievement>();

		public Dictionary<Achievement, double> achievementsKeeper;

		public float achievementToastWaitTime = 4f;

		protected double maxPoints;

		protected double achievedPoints;

		private Achievement currentConfig;

		public abstract void RaiseAchievement(List<AchievementStep> steps);

		public override void Init()
		{
			configs.Clear();
		}

		private void SetMaxPoints()
		{
			maxPoints = 0.0;
			for (int i = 0; i < configs.Count; i++)
			{
				for (int j = 0; j < configs[i].steps.Count; j++)
				{
					maxPoints += configs[i].steps[j].rewardAmount;
				}
			}
		}

		private void SetAchievedPoints()
		{
		}

		public void AddToAchievedPoints(double points)
		{
			achievedPoints += points;
		}

		public double GetMaxPoints()
		{
			SetMaxPoints();
			return maxPoints;
		}

		public double GetAchievedPoints()
		{
			CalculateAchievedPoints();
			return achievedPoints;
		}

		public void SetupAchievements()
		{
			AchievementsModule achievementsSettings = Manager.Get<AbstractModelManager>().achievementsSettings;
			achievementsKeeper = new Dictionary<Achievement, double>();
			foreach (Achievement availableAchievement in achievementsList.availableAchievements)
			{
				if (achievementsSettings.IsAchievementEnabled(availableAchievement.modelSettingsKey))
				{
					configs.Add(availableAchievement);
				}
			}
			foreach (Achievement config in configs)
			{
				config.group = achievementsSettings.GetAchievementGroup(config.modelSettingsKey);
				int achievementGroupSwap = achievementsSettings.GetAchievementGroupSwap(config.group);
				if (achievementGroupSwap >= 0)
				{
					config.group = achievementGroupSwap;
				}
				int i = 0;
				string modelSettingsKey = config.modelSettingsKey;
				config.steps = new List<AchievementStep>();
				for (; achievementsSettings.HasStep(modelSettingsKey, i); i++)
				{
					AchievementStep achievementStep = new AchievementStep();
					achievementStep.rewardAmount = achievementsSettings.GetAchievementRewardAmount(config.group, i);
					achievementStep.reward = GetReward(achievementsSettings.GetAchievementRewardType(config.group, i));
					achievementStep.countToUnlock = achievementsSettings.GetAchievementNeededToUnlock(modelSettingsKey, i, i);
					achievementStep.achievement = config;
					bool flag = achievementStep.isClaimed = PlayerPrefs.GetString("{0}.{1}.isClaimed".Formatted(config.modelSettingsKey, i), "false").ToBool(defaultValue: false);
					bool flag2 = achievementStep.isUnlocked = PlayerPrefs.GetString("{0}.{1}.isUnlocked".Formatted(config.modelSettingsKey, i), "false").ToBool(defaultValue: false);
					config.steps.Add(achievementStep);
				}
				double value = PlayerPrefs.GetString("{0}.progress".Formatted(config.modelSettingsKey), "0").ToDouble(0.0);
				achievementsKeeper.Add(config, value);
			}
			SetMaxPoints();
			SetAchievedPoints();
			achievementToastWaitTime = achievementsSettings.GetToastWaitTime();
		}

		protected abstract Reward GetReward(int rewardIndex);

		private void ParseAchievements(Achievement config, Action action = null)
		{
			List<AchievementStep> list = new List<AchievementStep>();
			foreach (AchievementStep item in from a in config.steps
				where !a.isUnlocked
				select a)
			{
				if (!(achievementsKeeper[config] < item.countToUnlock))
				{
					config.UnlockAchievementStep(item);
					item.claimAction = action;
					list.Add(item);
				}
			}
			if (list.Count >= 1)
			{
				RaiseAchievement(list);
			}
		}

		public Vector2 GetScrollViewPosition()
		{
			if (currentConfig == null)
			{
				return Vector2.up;
			}
			int num = configs.IndexOf(currentConfig);
			float y = (float)(configs.Count - num) / (float)configs.Count;
			return new Vector2(0f, y);
		}

		public void CalculateAchievedPoints()
		{
			achievedPoints = 0.0;
			for (int i = 0; i < configs.Count; i++)
			{
				for (int j = 0; j < configs[i].steps.Count; j++)
				{
					if (configs[i].steps[j].isClaimed)
					{
						achievedPoints += configs[i].steps[j].rewardAmount;
					}
				}
			}
		}

		public void RegisterEvent(string key, double amount = 1.0, Action action = null)
		{
			Achievement achievement = configs.FirstOrDefault((Achievement a) => a.modelSettingsKey == key);
			if (!(achievement == null))
			{
				RegisterEvent(achievement, amount, action);
			}
		}

		public void RegisterEvent(Achievement config, double amount = 1.0, Action action = null)
		{
			if (Manager.Get<AbstractModelManager>().achievementsSettings.IsAchievementsEnabled() && achievementsKeeper.ContainsKey(config) && !IsAchievementCompleted(config))
			{
				Dictionary<Achievement, double> dictionary;
				Achievement key;
				(dictionary = achievementsKeeper)[key = config] = dictionary[key] + amount;
				currentConfig = config;
				ParseAchievements(config, action);
			}
		}

		public void ResetCurrentConfig()
		{
			currentConfig = null;
		}

		public void SetEvent(Achievement type, double amount)
		{
			if (achievementsKeeper.ContainsKey(type) && !IsAchievementCompleted(type))
			{
				achievementsKeeper[type] = amount;
				ParseAchievements(type);
			}
		}

		private bool IsAchievementCompleted(Achievement achievement)
		{
			List<AchievementStep> steps = achievement.steps;
			if (steps.Count <= 0)
			{
				return false;
			}
			if (!(achievementsKeeper[achievement] >= steps[steps.Count - 1].countToUnlock))
			{
				return false;
			}
			achievementsKeeper[achievement] = steps[steps.Count - 1].countToUnlock;
			return true;
		}

		public void SavePrefs()
		{
			if (achievementsKeeper != null)
			{
				foreach (KeyValuePair<Achievement, double> item in achievementsKeeper)
				{
					PlayerPrefs.SetString("{0}.progress".Formatted(item.Key.modelSettingsKey), item.Value.ToString());
				}
			}
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				SavePrefs();
			}
		}

		private void OnApplicationQuit()
		{
			SavePrefs();
		}
	}
}
