// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.QuestsV2.MainQuestRewardRuner
using System;
using UnityEngine;

namespace Gameplay.QuestsV2
{
	public abstract class MainQuestRewardRuner : ScriptableObject
	{
		public abstract void Claim(Action claimed);
	}
}
