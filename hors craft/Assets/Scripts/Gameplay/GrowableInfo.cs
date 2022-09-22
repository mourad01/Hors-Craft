// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GrowableInfo
using Common.Managers;
using System;
using UnityEngine;

namespace Gameplay
{
	[Serializable]
	public class GrowableInfo
	{
		public string settingsKey;

		public GrowthStage[] growStages;

		protected Action grow;

		protected Func<string> getUniqueId;

		public GrowableInfo(string settingsKey, Action grow, Func<string> getId)
		{
			this.grow = grow;
			getUniqueId = getId;
			this.settingsKey = settingsKey;
		}

		public GrowableInfo()
		{
		}

		public virtual void Grow()
		{
			if (grow != null)
			{
				grow();
			}
		}

		public virtual string GetUniqueId()
		{
			if (getUniqueId == null)
			{
				UnityEngine.Debug.LogError("Get Unique ID is NULL");
				return "NULL";
			}
			return getUniqueId();
		}

		public GrowthData GetGrowthData()
		{
			return Manager.Get<GrowthManager>().GetGrowthData(GetUniqueId());
		}

		public GrowthStage CurrentStage()
		{
			int num = CurrentStageNumber();
			return (num >= growStages.Length) ? null : growStages[CurrentStageNumber()];
		}

		public int CurrentStageNumber()
		{
			return Manager.Get<GrowthManager>().GetGrowthStage(GetUniqueId());
		}

		public int StageIndex(GrowthStage stage)
		{
			return Array.IndexOf(growStages, stage);
		}

		public virtual string GetDescription()
		{
			return string.Empty;
		}
	}
}
