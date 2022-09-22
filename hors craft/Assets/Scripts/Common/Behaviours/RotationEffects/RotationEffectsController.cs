// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.RotationEffects.RotationEffectsController
using System.Collections.Generic;
using UnityEngine;

namespace Common.Behaviours.RotationEffects
{
	public class RotationEffectsController
	{
		private const float MODIFIERS_PER_PRIORITY = 10f;

		private const float MODIFIER_WEIGHT = 0.1f;

		private SortedList<float, RotationEffect> sortedModifiers = new SortedList<float, RotationEffect>();

		private bool timeScaleIndependent;

		public RotationEffectsController(bool timeScaleIndependent)
		{
			this.timeScaleIndependent = timeScaleIndependent;
		}

		public void AddModifier(int priority, RotationEffect modifier)
		{
			modifier.timeScaleIndependent = timeScaleIndependent;
			float num = priority;
			if (sortedModifiers.ContainsKey(num))
			{
				do
				{
					num += 0.1f;
				}
				while (sortedModifiers.ContainsKey(num) && num < (float)(priority + 1));
				if (num >= (float)priority)
				{
					UnityEngine.Debug.LogWarning("Cannot have more than " + 10f + " modifiers per priority!");
					return;
				}
			}
			sortedModifiers.Add(num, modifier);
		}

		public void RemoveModifier(int priority, RotationEffect modifier)
		{
			float num = priority;
			if (!sortedModifiers.ContainsKey(num))
			{
				do
				{
					num += 0.1f;
				}
				while (!sortedModifiers.ContainsKey(num) && num < (float)(priority + 1));
				if (num >= (float)(priority + 1))
				{
					UnityEngine.Debug.LogWarning("Couldn't find given modifier with given priority!");
					return;
				}
			}
			sortedModifiers.Remove(num);
		}

		public Quaternion CalculateCurrentRotation()
		{
			Quaternion quaternion = Quaternion.identity;
			foreach (KeyValuePair<float, RotationEffect> sortedModifier in sortedModifiers)
			{
				RotationEffect value = sortedModifier.Value;
				Quaternion currentRotation = value.GetCurrentRotation();
				quaternion = ((value.mode != 0) ? (quaternion * Quaternion.Lerp(Quaternion.identity, currentRotation, value.intensity)) : Quaternion.Lerp(quaternion, currentRotation, value.intensity));
			}
			return quaternion;
		}
	}
}
