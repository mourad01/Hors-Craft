// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.AchievementsManager
using Common.Managers;
using Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class AchievementsManager : Manager, IGameCallbacksListener
	{
		[Serializable]
		public struct AchievementSetting
		{
			public string description;

			public string achievementId;

			public string commonId;
		}

		[Header("THIS ONE IS DEPRECATED")]
		[Space(20f)]
		[Header("AchievementId - achievement variable name from GPGSConstants")]
		[Header("CommonId - id used in classes eg. FishingManager")]
		public AchievementSetting[] achievementSettings;

		private Dictionary<string, string> achievementDict = new Dictionary<string, string>();

		private Dictionary<string, int> achievementProgress = new Dictionary<string, int>();

		public override void Init()
		{
			AchievementSetting[] array = achievementSettings;
			for (int i = 0; i < array.Length; i++)
			{
				AchievementSetting achievementSetting = array[i];
				achievementDict.Add(achievementSetting.commonId, achievementSetting.achievementId);
			}
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
		}

		public int GetAchievementProgress(string id)
		{
			if (achievementProgress.ContainsKey(id))
			{
				return achievementProgress[id];
			}
			return 0;
		}

		public void Report(string id, int amount = 100)
		{
			achievementProgress[id] = amount;
			if (achievementDict.ContainsKey(id))
			{
				string iDFor = Singleton<GooglePlayConstants>.get.GetIDFor(achievementDict[id]);
				if ((float)amount == 100f || (float)amount == 0f)
				{
					Manager.Get<SocialPlatformManager>().social.ReportProgress(iDFor, amount);
				}
			}
		}

		public void OnGameSavedFrequent()
		{
			PlayerPrefs.SetString("achievements.progress", JSONHelper.ToJSON(achievementProgress));
		}

		public void OnGameSavedInfrequent()
		{
		}

		public void OnGameplayStarted()
		{
			if (PlayerPrefs.HasKey("achievements.progress"))
			{
				string @string = PlayerPrefs.GetString("achievements.progress");
				achievementProgress = JSONHelper.Deserialize<Dictionary<string, int>>(@string);
			}
			else
			{
				achievementProgress = new Dictionary<string, int>();
			}
		}

		public void OnGameplayRestarted()
		{
		}
	}
}
