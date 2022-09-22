// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.RotationEffects.RotationEffect
using System;
using UnityEngine;

namespace Common.Behaviours.RotationEffects
{
	[Serializable]
	public abstract class RotationEffect
	{
		public enum OverrideMode
		{
			ABSOLUTE,
			RELATIVE
		}

		[HideInInspector]
		public bool timeScaleIndependent;

		[Range(0f, 1f)]
		[Tooltip("In absolute modifiers intensity=1 can competely replace rotation. In relative modifiers it's just a 'power factor'.")]
		public float intensity = 1f;

		protected float currentTime
		{
			get
			{
				if (timeScaleIndependent)
				{
					return Time.realtimeSinceStartup;
				}
				return Time.time;
			}
		}

		public abstract OverrideMode mode
		{
			get;
		}

		public abstract Quaternion GetCurrentRotation();
	}
}
