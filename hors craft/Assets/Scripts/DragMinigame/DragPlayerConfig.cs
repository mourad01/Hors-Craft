// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragPlayerConfig
using UnityEngine;

namespace DragMinigame
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Drag Game/Base Player Config")]
	public class DragPlayerConfig : MinigameSingleConfig
	{
		public float Power;

		public float BoostValue;

		public float BoostTime;

		public float[] shiftValues;

		public GameObject VisualRepresentation;
	}
}
