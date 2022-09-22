// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AllInOneAdRequirementsModule
using Common.Model;
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class AllInOneAdRequirementsModule : ModelModule
	{
		private class RequirementsConfig
		{
			public string name;

			public string mode;

			public int minSessions;

			public float probability;

			public int firstTime;

			public int nextTimeDelayAddition;
		}

		public class HasToShowAdOutput
		{
			public bool enabled;

			public string disabledReason;

			public HasToShowAdOutput(bool enabled, string reason = "")
			{
				this.enabled = enabled;
				disabledReason = reason;
			}
		}

		private Dictionary<string, RequirementsConfig> configs = new Dictionary<string, RequirementsConfig>();

		protected string keyAdRequirementTimingFirstTime(string contextId)
		{
			return "ad." + contextId + ".firsttime";
		}

		protected string keyAdRequirementTimingNextTime(string contextId)
		{
			return "ad." + contextId + ".nexttimedelayaddition";
		}

		protected string keyAdEnabled(string contextId)
		{
			return "ad." + contextId + ".enabled";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			for (int i = 0; i < 25; i++)
			{
				descriptions.AddDescription("adreq." + i + ".enabled", defaultValue: false);
				descriptions.AddDescription("adreq." + i + ".contextname", string.Empty);
				descriptions.AddDescription("adreq." + i + ".mode", "probability");
				descriptions.AddDescription("adreq." + i + ".minsessions", 0);
				descriptions.AddDescription("adreq." + i + ".probability", 1f);
				descriptions.AddDescription("adreq." + i + ".firsttime", 0);
				descriptions.AddDescription("adreq." + i + ".nexttimedelayaddition", 0);
			}
		}

		public override void AssignSettings(Settings settings)
		{
			base.AssignSettings(settings);
			SetConfig();
		}

		public override void OnModelDownloaded()
		{
			SetConfig();
		}

		public void SetConfig()
		{
			configs.Clear();
			for (int i = 0; i < 25; i++)
			{
				if (base.settings.GetBool("adreq." + i + ".enabled"))
				{
					string @string = base.settings.GetString("adreq." + i + ".contextname");
					if (configs.ContainsKey(@string))
					{
						UnityEngine.Debug.LogError("Duplicated ad contextname found in downloaded settings! It's: " + @string);
						continue;
					}
					RequirementsConfig requirementsConfig = new RequirementsConfig();
					requirementsConfig.name = @string;
					requirementsConfig.mode = base.settings.GetString("adreq." + i + ".mode");
					requirementsConfig.minSessions = base.settings.GetInt("adreq." + i + ".minsessions");
					requirementsConfig.probability = base.settings.GetFloat("adreq." + i + ".probability");
					requirementsConfig.firstTime = base.settings.GetInt("adreq." + i + ".firsttime");
					requirementsConfig.nextTimeDelayAddition = base.settings.GetInt("adreq." + i + ".nexttimedelayaddition");
					RequirementsConfig value = requirementsConfig;
					configs[@string] = value;
				}
			}
		}

		private string adsShownForContextKey(string contextId)
		{
			return contextId + "AdsShown";
		}

		private string lastShownAdTimeForContextKey(string contextId)
		{
			return contextId + "AdShownTime";
		}

		public HasToShowAdOutput HasToShowAd(string contextName)
		{
			if (!configs.TryGetValue(contextName, out RequirementsConfig value))
			{
				return new HasToShowAdOutput(enabled: false, "No config named " + contextName + " found in settings OR it's not enabled.");
			}
			if (base.context.isAdsFree)
			{
				return new HasToShowAdOutput(enabled: false, "The game/instance is ads-free.");
			}
			int sessionNo = PlayerSession.GetSessionNo();
			if (sessionNo <= value.minSessions)
			{
				return new HasToShowAdOutput(enabled: false, "Minsessions: " + sessionNo + " / config says it's minimum " + value.minSessions);
			}
			switch (value.mode)
			{
			default:
				return HasToShowBasedOnProbability(value);
			case "timeInGame":
				return HasToShowBasedOnTimeInGame(value);
			}
		}

		private HasToShowAdOutput HasToShowBasedOnProbability(RequirementsConfig config)
		{
			if (Random.value < config.probability)
			{
				return new HasToShowAdOutput(enabled: true, string.Empty);
			}
			return new HasToShowAdOutput(enabled: false, "Random evaluation of " + config.probability * 100f + "% insuccessfull");
		}

		private HasToShowAdOutput HasToShowBasedOnTimeInGame(RequirementsConfig config)
		{
			int firstTime = config.firstTime;
			int nextTimeDelayAddition = config.nextTimeDelayAddition;
			if (firstTime <= 0)
			{
				return new HasToShowAdOutput(enabled: false, "Have to wait for new ad. Waiting for 'showFirstAdAfterTime': " + firstTime);
			}
			int num = firstTime;
			int @int = PlayerPrefs.GetInt(adsShownForContextKey(config.name), 0);
			int int2 = PlayerPrefs.GetInt(lastShownAdTimeForContextKey(config.name), 0);
			if (@int > 0)
			{
				num = int2 + firstTime + @int * nextTimeDelayAddition;
			}
			if (!MonoBehaviourSingleton<ManagersContainer>.get.ContainsManager<TotalPlaytimeManager>())
			{
				return new HasToShowAdOutput(enabled: false, "Wants to wait for the ad, but there's no TotalPlaytimeManager found!");
			}
			int num2 = Mathf.RoundToInt(Manager.Get<TotalPlaytimeManager>().GetTotalPlaytime());
			if (num2 > num)
			{
				PlayerPrefs.SetInt(adsShownForContextKey(config.name), @int + 1);
				PlayerPrefs.SetInt(lastShownAdTimeForContextKey(config.name), num2);
				return new HasToShowAdOutput(enabled: true, string.Empty);
			}
			return new HasToShowAdOutput(enabled: false, "Have to wait for new ad. Next ad in: " + (num - num2) + " seconds.");
		}
	}
}
