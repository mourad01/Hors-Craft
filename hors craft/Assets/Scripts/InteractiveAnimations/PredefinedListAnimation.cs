// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveAnimations.PredefinedListAnimation
using Common.Utils;
using UnityEngine;

namespace InteractiveAnimations
{
	[CreateAssetMenu(fileName = "Animation-Definition", menuName = "ScriptableObjects/Animations/PredefinedListAnimation-Definition")]
	public class PredefinedListAnimation : StaticAnimation
	{
		public AnimationClip[] clips;

		public override AnimationClip GetClip()
		{
			return clips.Random();
		}
	}
}
