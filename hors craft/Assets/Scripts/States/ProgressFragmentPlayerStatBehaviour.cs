// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ProgressFragmentPlayerStatBehaviour
using System;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	public class ProgressFragmentPlayerStatBehaviour : ProgressFragmentBehaviour
	{
		[Serializable]
		public class StatItem
		{
			public PlayerStats stat;

			public Sprite icon;
		}

		public List<StatItem> statItems = new List<StatItem>();

		protected override void SetStats()
		{
			fragment.ClearStats();
			foreach (StatItem statItem in statItems)
			{
				fragment.AddProgressStat(statItem.icon, statItem.stat.GetStats().ToString("F0"));
			}
		}
	}
}
