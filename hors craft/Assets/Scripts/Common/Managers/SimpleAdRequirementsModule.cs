// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SimpleAdRequirementsModule
using Common.Model;
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class SimpleAdRequirementsModule : ModelModule
	{
		private List<string> contextIds = new List<string>();

		public SimpleAdRequirementsModule(List<string> contextIds)
		{
			this.contextIds.AddRange(contextIds);
		}

		private string keyAdRequirementMinSessionsId(string contextId)
		{
			return "ad." + contextId + ".minsessions";
		}

		private string keyAdRequirementProbabilityID(string contextId)
		{
			return "ad." + contextId + ".probability";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			foreach (string contextId in contextIds)
			{
				descriptions.AddDescription(keyAdRequirementMinSessionsId(contextId), 0);
				descriptions.AddDescription(keyAdRequirementProbabilityID(contextId), 0f);
			}
		}

		public override void OnModelDownloaded()
		{
		}

		public bool HasToShowAdForContext(string contextId)
		{
			if (!contextIds.Contains(contextId))
			{
				return false;
			}
			int @int = base.settings.GetInt(keyAdRequirementMinSessionsId(contextId));
			float @float = base.settings.GetFloat(keyAdRequirementProbabilityID(contextId));
			int sessionNo = PlayerSession.GetSessionNo();
			if (sessionNo <= @int)
			{
				return false;
			}
			if (Random.value > @float)
			{
				return false;
			}
			return true;
		}
	}
}
