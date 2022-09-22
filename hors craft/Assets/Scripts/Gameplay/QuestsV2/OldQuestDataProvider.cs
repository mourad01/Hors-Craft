// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.QuestsV2.OldQuestDataProvider
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Gameplay.QuestsV2
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Quests/DataProviders/OldQuestDataProvider")]
	public class OldQuestDataProvider : MainQuestDataProvider
	{
		public QuestType questType;

		private string _questName;

		private string questName
		{
			[CompilerGenerated]
			get
			{
				return string.IsNullOrEmpty(_questName) ? (_questName = questType.ToString()) : _questName;
			}
		}

		public override bool supportEvents
		{
			[CompilerGenerated]
			get
			{
				return true;
			}
		}

		public override float GetProgress()
		{
			return MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor(questName);
		}

		public override void RegisterCallback(Action<int> callback)
		{
			MonoBehaviourSingleton<ProgressCounter>.get.AddCallback(questName, callback);
		}
	}
}
