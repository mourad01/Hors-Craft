// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.QuestsV2.MainQuestDataProvider
using System;
using UnityEngine;

namespace Gameplay.QuestsV2
{
	public abstract class MainQuestDataProvider : ScriptableObject
	{
		public abstract bool supportEvents
		{
			get;
		}

		public abstract float GetProgress();

		public abstract void RegisterCallback(Action<int> callback);
	}
}
