// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.TimeBasedAdRequirementsModule
using Common.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class TimeBasedAdRequirementsModule : ModelModule
	{
		public TimeBasedAdRequirementsModule(List<string> contextIds)
		{
		}

		public TimeBasedAdRequirementsModule()
		{
		}

		protected string keyAdRequirementTimingFirstTime(string contextId)
		{
			return "ad." + contextId + ".firsttime";
		}

		protected string keyAdRequirementTimingNextTime(string contextId)
		{
			return "ad." + contextId + ".nexttimedelay";
		}

		protected string keyAdRequirementTimingNextTimeAddition(string contextId)
		{
			return "ad." + contextId + ".nexttimedelayaddition";
		}

		protected string keyAdEnabled(string contextId)
		{
			return "ad." + contextId + ".enabled";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
		}

		public override void OnModelDownloaded()
		{
		}

		private string adsShownForContextKey(string contextId)
		{
			return contextId + "AdsShown";
		}

		private string lastShownAdTimeForContextKey(string contextId)
		{
			return contextId + "AdShownTime";
		}

		public bool IsEnabled(string contextId)
		{
			return ModelSettingsHelper.GetBoolFromStringSettings(base.settings, keyAdEnabled(contextId), def: true);
		}

		public bool HasToShowAdForContext(string contextId, int timeInGame)
		{
			int num = TimeRemainingForContext(contextId, timeInGame, useLogs: true, showAdAfter: true);
			if (num == 0)
			{
				int @int = PlayerPrefs.GetInt(adsShownForContextKey(contextId), 0);
				PlayerPrefs.SetInt(adsShownForContextKey(contextId), @int + 1);
				PlayerPrefs.SetInt(lastShownAdTimeForContextKey(contextId), timeInGame);
				Log($"Can show ad");
				return true;
			}
			if (num > 0)
			{
				Log($"Have to wait for new ad. Next ad in: {num} seconds");
				return false;
			}
			return false;
		}

		private void Log(string message)
		{
			UnityEngine.Debug.Log($"TimeBasedAdRequirements.HasToShowAdForContext() | {message}");
		}

		public int TimeRemainingForContext(string contextId, int timeInGame, bool useLogs = false, bool showAdAfter = false)
		{
			if (base.context.isAdsFree || !IsEnabled(contextId))
			{
				if (useLogs)
				{
					Log("Is ads free or disabled for context " + contextId);
				}
				return -1;
			}
			int intFromStringSettings = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAdRequirementTimingFirstTime(contextId), 120);
			int intFromStringSettings2 = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAdRequirementTimingNextTime(contextId));
			int intFromStringSettings3 = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAdRequirementTimingNextTimeAddition(contextId));
			if (intFromStringSettings < 0)
			{
				if (useLogs)
				{
					Log($"Have to wait for new ad. Waiting for 'showFirstAdAfterTime': {intFromStringSettings}");
				}
				return -1;
			}
			int @int = PlayerPrefs.GetInt(adsShownForContextKey(contextId), 0);
			int int2 = PlayerPrefs.GetInt(lastShownAdTimeForContextKey(contextId), 0);
			int num = (@int != 0) ? (int2 + intFromStringSettings2 + @int * intFromStringSettings3) : intFromStringSettings;
			if (timeInGame >= num)
			{
				return 0;
			}
			return num - timeInGame;
		}
	}
}
