// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.RotationEffects.LerpRotationEffect
using System;
using UnityEngine;

namespace Common.Behaviours.RotationEffects
{
	[Serializable]
	public class LerpRotationEffect : RotationEffect
	{
		[Range(0f, 3f)]
		public float lerpDuration = 0.2f;

		private Quaternion fromRotation = Quaternion.identity;

		private Quaternion toRotation = Quaternion.identity;

		private Quaternion lastRotation = Quaternion.identity;

		private float startTime;

		public override OverrideMode mode => OverrideMode.ABSOLUTE;

		public void StartLerpFromLastTargetTo(Quaternion rotation)
		{
			fromRotation = lastRotation;
			startTime = base.currentTime;
			toRotation = rotation;
		}

		public override Quaternion GetCurrentRotation()
		{
			float num = CalculateCurrentFactor();
			if (num == 0f)
			{
				return fromRotation;
			}
			if (num == 1f)
			{
				return toRotation;
			}
			lastRotation = Quaternion.Lerp(fromRotation, toRotation, num);
			return lastRotation;
		}

		public bool HasEnded()
		{
			return CalculateCurrentFactor() >= 1f;
		}

		private float CalculateCurrentFactor()
		{
			if (lerpDuration == 0f)
			{
				return 1f;
			}
			return Mathf.Clamp((base.currentTime - startTime) / lerpDuration, 0f, 1f);
		}
	}
}
