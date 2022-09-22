// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SurvivalRankManager
using Gameplay;
using Gameplay.Configs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class SurvivalRankManager : Manager
	{
		[Serializable]
		public class Rank
		{
			public string name;

			public float pointsRequirements;
		}

		private const string BASE_SETTINGS_ID = "survival.rank";

		private const string RANK_KEY = ".current";

		public string leaderboardID = "leaderboardTopPlayers";

		public bool hasPauseUI;

		[SerializeField]
		private SurvivalRankConfig[] rankConfigs;

		[SerializeField]
		private Rank[] _ranks;

		[SerializeField]
		private float leaderboardMultiplier = 1f;

		private Dictionary<string, SurvivalRankConfig> nameToConfig = new Dictionary<string, SurvivalRankConfig>();

		private float fullRankPoints;

		public bool retainScoreInPrefs = true;

		protected int lastPoints;

		private ModelManager modelManager;

		public Rank[] ranks => _ranks;

		public int currentRankIndex
		{
			get;
			private set;
		}

		public int LastPoints => lastPoints;

		public float currentRankMinPoints => ranks[currentRankIndex].pointsRequirements;

		public float currentPoints
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < rankConfigs.Length; i++)
				{
					num += rankConfigs[i].RankPoints;
				}
				return num;
			}
		}

		public float pointsToNextRank
		{
			get
			{
				int num = currentRankIndex + 1;
				if (num >= ranks.Length)
				{
					return -1f;
				}
				return ranks[num].pointsRequirements;
			}
		}

		public string rankName => ranks[currentRankIndex].name;

		public float leaderboardPoints
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < rankConfigs.Length; i++)
				{
					num += rankConfigs[i].LeaderboardPoints;
				}
				return num * leaderboardMultiplier;
			}
		}

		public override void Init()
		{
			if (retainScoreInPrefs)
			{
				LoadPrefs();
			}
			InitVariables();
		}

		public bool Increase(string name, float value = 1f)
		{
			if (nameToConfig.ContainsKey(name))
			{
				Increase(nameToConfig[name]);
				return true;
			}
			return false;
		}

		public void Increase(SurvivalRankConfig config, float value = 1f)
		{
			config.IncreaseProgress(value);
			PlayerPrefs.SetFloat(GetConfigKey(config), config.Progress);
			PlayerPrefs.Save();
		}

		public void PlayerDie()
		{
			lastPoints = Mathf.FloorToInt(leaderboardPoints);
			for (int i = 0; i < rankConfigs.Length; i++)
			{
				rankConfigs[i].PlayerDie();
			}
		}

		public void LoadFromModel()
		{
			modelManager = Manager.Get<ModelManager>();
			for (int i = 0; i < ranks.Length; i++)
			{
				ranks[i].pointsRequirements = modelManager.survivalSettings.GetRankPointsRequirement(i);
			}
		}

		private void LoadPrefs()
		{
			currentRankIndex = PlayerPrefs.GetInt("survival.rank.current", 0);
			for (int i = 0; i < rankConfigs.Length; i++)
			{
				rankConfigs[i].Init(PlayerPrefs.GetFloat(GetConfigKey(rankConfigs[i]), 0f));
				SurvivalRankConfig obj = rankConfigs[i];
				obj.onStepChange = (Action)Delegate.Combine(obj.onStepChange, new Action(PointsChange));
			}
		}

		private void InitVariables()
		{
			fullRankPoints = 0f;
			for (int i = 0; i < rankConfigs.Length; i++)
			{
				if (!nameToConfig.ContainsKey(rankConfigs[i].Name))
				{
					nameToConfig.Add(rankConfigs[i].Name, rankConfigs[i]);
					fullRankPoints += rankConfigs[i].MaxPoints;
				}
				else
				{
					UnityEngine.Debug.LogError($"SURVIVAL RANK INIT VARAIBLES: Rank config with name: {rankConfigs[i].Name} is doubled or some thing");
				}
			}
			CalcRank(doToast: false);
		}

		private void CalcRank(bool doToast = true)
		{
			float currentPoints = this.currentPoints;
			int num = ranks.Length - 1;
			while (true)
			{
				if (num >= 0)
				{
					if (ranks[num].pointsRequirements <= currentPoints && currentRankIndex < num)
					{
						break;
					}
					num--;
					continue;
				}
				return;
			}
			currentRankIndex = num;
			if (doToast)
			{
				string text = Manager.Get<TranslationsManager>().GetText("rank.base.toast", "Congratulations you are now {0}");
				string arg = Manager.Get<TranslationsManager>().GetText("rank.name." + num, ranks[num].name).ToUpper();
				Manager.Get<ToastManager>().ShowToast(string.Format(text, arg));
				Manager.Get<StatsManager>().LevelUp(num);
			}
		}

		private string GetConfigKey(SurvivalRankConfig config)
		{
			return "survival.rank." + config.Name;
		}

		private void PointsChange()
		{
			CalcRank();
		}
	}
}
