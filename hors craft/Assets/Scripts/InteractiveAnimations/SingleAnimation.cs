// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveAnimations.SingleAnimation
using UnityEngine;

namespace InteractiveAnimations
{
	[CreateAssetMenu(fileName = "Animation-Definition", menuName = "ScriptableObjects/Animations/SingleAnimation-Definition")]
	public class SingleAnimation : StaticAnimation
	{
		public AnimationClip clip;

		public override AnimationClip GetClip()
		{
			return clip;
		}
	}
}
