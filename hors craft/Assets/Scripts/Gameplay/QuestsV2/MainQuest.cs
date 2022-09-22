// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.QuestsV2.MainQuest
using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;

namespace Gameplay.QuestsV2
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Quests/MainQuest")]
	public class MainQuest : ScriptableObject
	{
		public MainQuestDataProvider dataProvider;

		public float amountToCompleted;

		[Reorderable]
		public List<MainQuestRewardRuner> rewards = new List<MainQuestRewardRuner>();

		public string key;

		public string label;

		public string description;

		private int nextClaim;

		public bool IsDone()
		{
			return dataProvider.GetProgress() >= amountToCompleted;
		}

		public void Claim()
		{
			if (nextClaim < rewards.Count)
			{
				rewards[nextClaim++].Claim(Claim);
			}
		}

		public override string ToString()
		{
			return $"Quest {label} with requirement {amountToCompleted}/{dataProvider.GetProgress()}; desc: {description}";
		}
	}
}
