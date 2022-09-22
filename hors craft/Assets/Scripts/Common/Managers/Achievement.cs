// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.Achievement
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	[Serializable]
	[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/Achievements/SingleAchievement", order = 1)]
	public class Achievement : ScriptableObject
	{
		private const string titleBaseKey = "achievement.title.";

		private const string descriptionBaseKey = "achievement.description.";

		public Sprite achivementIcon;

		public string modelSettingsKey;

		[HideInInspector]
		public int group;

		public string titleTranslationDefaultText;

		public string descriptionTranslationDefaultText;

		public List<AchievementStep> steps = new List<AchievementStep>();

		public string titleTranslationKey => "achievement.title." + modelSettingsKey;

		public string descriptionTranslationKey => "achievement.description." + modelSettingsKey;

		public void ReportProgress(int amount = 1)
		{
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<AbstractAchievementManager>().RegisterEvent(modelSettingsKey, amount);
			}
		}

		public AchievementStep GetCurrentAchievementStep()
		{
			foreach (AchievementStep step in steps)
			{
				if (!step.isClaimed)
				{
					return step;
				}
			}
			return steps[steps.Count - 1];
		}

		public void ClaimAchievementStep(AchievementStep stepToClaim)
		{
			int num = 0;
			foreach (AchievementStep step in steps)
			{
				if (stepToClaim == step)
				{
					if (step.claimAction != null)
					{
						step.claimAction();
					}
					step.isClaimed = true;
					PlayerPrefs.SetString("{0}.{1}.isClaimed".Formatted(modelSettingsKey, num), "true");
				}
				num++;
			}
		}

		public void UnlockAchievementStep(AchievementStep stepToUnlock)
		{
			int num = 0;
			foreach (AchievementStep step in steps)
			{
				if (stepToUnlock == step)
				{
					step.isUnlocked = true;
					PlayerPrefs.SetString("{0}.{1}.isUnlocked".Formatted(modelSettingsKey, num), "true");
				}
				num++;
			}
		}

		public int GetStepsCount()
		{
			return steps.Count;
		}

		public int GetCurrentStepIndex()
		{
			for (int i = 0; i < steps.Count; i++)
			{
				if (!steps[i].isClaimed)
				{
					return i;
				}
			}
			return steps.Count;
		}

		public bool IsAllClaimed()
		{
			return steps[steps.Count - 1].isClaimed;
		}
	}
}
