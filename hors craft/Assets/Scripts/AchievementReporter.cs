// DecompilerFi decompiler from Assembly-CSharp.dll class: AchievementReporter
using Common.Managers;
using Gameplay;
using System;
using UnityEngine;

public class AchievementReporter : MonoBehaviour
{
	[Serializable]
	public class AchievementConfig
	{
		public string commonAchievementID;

		public int achievementProgress = 100;
	}

	public AchievementConfig[] configs;

	public void Report()
	{
		AchievementConfig[] array = configs;
		foreach (AchievementConfig achievementConfig in array)
		{
			if (achievementConfig.achievementProgress == 0)
			{
				achievementConfig.achievementProgress = 100;
			}
			Report(achievementConfig.commonAchievementID, achievementConfig.achievementProgress);
		}
	}

	public static void Report(string id, int amount = 100)
	{
		if (Manager.Contains<AchievementsManager>())
		{
			Manager.Get<AchievementsManager>().Report(id, amount);
		}
	}
}
