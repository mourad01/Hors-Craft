// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveAnimations.StaticAnimation
using System;
using UnityEngine;

namespace InteractiveAnimations
{
	public class StaticAnimation : Animation
	{
		[HideInInspector]
		public int animationIndex;

		[Space]
		public float chanceToPlay;

		public float timePlaying = 8f;

		public float timeDelay = 5f;

		public override AnimationClip GetClip()
		{
			throw new NotImplementedException();
		}
	}
}
