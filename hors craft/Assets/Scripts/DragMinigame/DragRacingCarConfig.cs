// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragRacingCarConfig
using UnityEngine;

namespace DragMinigame
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Drag Game/Drag Racing/Car Config")]
	public class DragRacingCarConfig : DragPlayerConfig
	{
		public float MaxSpeed;

		public float Acceleration;

		public float GearSpeedRetention;

		public float Braking;
	}
}
