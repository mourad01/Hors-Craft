// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveAnimations.WalkAnimation
using UnityEngine;

namespace InteractiveAnimations
{
	[CreateAssetMenu(fileName = "Animation-Definition", menuName = "ScriptableObjects/Animations/WalkAnimation-Definition")]
	public class WalkAnimation : Animation
	{
		public AnimationClip clip;

		public Skin.Gender gender;

		public override AnimationClip GetClip()
		{
			return clip;
		}
	}
}
