// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectsRequirement
using System;
using UnityEngine;

namespace Common.ImageEffects
{
	[Serializable]
	public abstract class ImageEffectsRequirement : ScriptableObject
	{
		public float requirement;

		public abstract bool MeetRequirement();
	}
}
