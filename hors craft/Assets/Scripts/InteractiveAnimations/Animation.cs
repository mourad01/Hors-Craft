// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveAnimations.Animation
using UnityEngine;

namespace InteractiveAnimations
{
	public abstract class Animation : ScriptableObject
	{
		[Header("Opcjonalny opis animacji (tylko do deva)")]
		public string description;

		public abstract AnimationClip GetClip();
	}
}
