// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragGameConfig
using System;
using UnityEngine;

namespace DragMinigame
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Drag Game/Game Config")]
	public class DragGameConfig : MinigameSingleConfig
	{
		[Serializable]
		public class ZoneValues
		{
			public float minPerfectValue;

			public float maxPerfectValue;

			public float minAcceptableValue;

			public float maxAcceptableValue;

			public float maxPenalty;

			public float GetPenaltyAndRating(float value, ref DragPlayerController.ShiftRating gearSwitch)
			{
				if (value < maxPerfectValue && value > minPerfectValue)
				{
					gearSwitch = DragPlayerController.ShiftRating.PERFECT;
					return 0f;
				}
				if (value <= minPerfectValue && value >= minAcceptableValue)
				{
					gearSwitch = DragPlayerController.ShiftRating.GOOD;
					float num = minPerfectValue - value;
					return num * maxPenalty / (minPerfectValue - minAcceptableValue);
				}
				if (value >= maxPerfectValue && value <= maxAcceptableValue)
				{
					gearSwitch = DragPlayerController.ShiftRating.GOOD;
					float num2 = value - maxPerfectValue;
					return 1.5f * num2 * maxPenalty / (maxAcceptableValue - maxPerfectValue);
				}
				gearSwitch = DragPlayerController.ShiftRating.BAD;
				if (value > maxAcceptableValue)
				{
					return maxPenalty * 1.5f;
				}
				return maxPenalty;
			}
		}

		public ZoneValues startZones;

		public ZoneValues changeGearZones;

		[Tooltip("Used to match max value on UI (useful when working with Images with 'Hard Coded' values")]
		public float maxProgressShiftValue;

		public bool resetProgressOnShift;

		public bool resetProgressOnStart;

		public float gameTime;
	}
}
