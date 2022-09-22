// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.RotationEffects.ShakeRotationModifier
using System;
using UnityEngine;

namespace Common.Behaviours.RotationEffects
{
	[Serializable]
	public class ShakeRotationModifier : RotationEffect
	{
		[Range(0f, 3f)]
		public float shakeDuration = 0.5f;

		private float startTime;

		public override OverrideMode mode => OverrideMode.RELATIVE;

		public void StartShake()
		{
			startTime = base.currentTime;
		}

		public override Quaternion GetCurrentRotation()
		{
			float num = CalculateCurrentFactor();
			if (num == 0f)
			{
				return Quaternion.identity;
			}
			if (num == 1f)
			{
				return Quaternion.identity;
			}
			float t = 1f - num;
			return Quaternion.Slerp(Quaternion.identity, UnityEngine.Random.rotation, t);
		}

		private float CalculateCurrentFactor()
		{
			if (shakeDuration == 0f)
			{
				return 1f;
			}
			return Mathf.Clamp((base.currentTime - startTime) / shakeDuration, 0f, 1f);
		}
	}
}
