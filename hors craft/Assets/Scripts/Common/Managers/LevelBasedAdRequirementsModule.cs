// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LevelBasedAdRequirementsModule
using Common.Model;
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class LevelBasedAdRequirementsModule : ModelModule
	{
		public LevelBasedAdRequirementsModule(List<string> contextIds)
		{
		}

		public LevelBasedAdRequirementsModule()
		{
		}

		private string keyAdRequirementMinSessionsId(string contextId)
		{
			return "ad." + contextId + ".minsessions";
		}

		private string keyAdRequirementMinLevelsID(string contextId)
		{
			return "ad." + contextId + ".minlevels";
		}

		private string keyAdRequirementLevelsDividerID(string contextId)
		{
			return "ad." + contextId + ".levelsdivider";
		}

		private string keyAdRequirementProbabilityID(string contextId)
		{
			return "ad." + contextId + ".probability";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
		}

		public override void OnModelDownloaded()
		{
		}

		public bool HasToShowAdForContext(string contextId, int levelsFinished, int lastPlayedLevelNo)
		{
			if (base.context.isAdsFree)
			{
				return false;
			}
			int intFromStringSettings = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAdRequirementMinSessionsId(contextId));
			int intFromStringSettings2 = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAdRequirementMinLevelsID(contextId));
			int intFromStringSettings3 = ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyAdRequirementLevelsDividerID(contextId));
			float floatFromStringSettings = ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyAdRequirementProbabilityID(contextId));
			int sessionNo = PlayerSession.GetSessionNo();
			if (sessionNo <= intFromStringSettings)
			{
				return false;
			}
			if (levelsFinished < intFromStringSettings2)
			{
				return false;
			}
			if (intFromStringSettings3 > 1 && lastPlayedLevelNo % intFromStringSettings3 != 0)
			{
				return false;
			}
			if (Random.value > floatFromStringSettings)
			{
				return false;
			}
			return true;
		}
	}
}
