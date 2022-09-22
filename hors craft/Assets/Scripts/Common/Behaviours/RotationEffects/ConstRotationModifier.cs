// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.RotationEffects.ConstRotationModifier
using System;
using UnityEngine;

namespace Common.Behaviours.RotationEffects
{
	[Serializable]
	public class ConstRotationModifier : RotationEffect
	{
		[Range(0f, 359f)]
		public float constAngle = 30f;

		private Quaternion currentRotation = Quaternion.identity;

		public override OverrideMode mode => OverrideMode.ABSOLUTE;

		public void Enable()
		{
			currentRotation = Quaternion.AngleAxis(constAngle, Vector3.forward);
			intensity = 1f;
		}

		public void Disable()
		{
			currentRotation = Quaternion.identity;
			intensity = 0f;
		}

		public override Quaternion GetCurrentRotation()
		{
			return currentRotation;
		}
	}
}
