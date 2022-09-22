// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.QuestsV2.ProgressCounterDataProvider
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Gameplay.QuestsV2
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Quests/DataProviders/ProgressCounterDataProvider")]
	public class ProgressCounterDataProvider : MainQuestDataProvider
	{
		public new string name;

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
			return MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor(name);
		}

		public override void RegisterCallback(Action<int> callback)
		{
			MonoBehaviourSingleton<ProgressCounter>.get.AddCallback(name, callback);
		}
	}
}
