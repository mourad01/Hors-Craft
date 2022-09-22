// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Gameplay.CurrentInputInfo
using UnityEngine;

namespace Common.Gameplay
{
	public class CurrentInputInfo
	{
		public TouchPhase phase;

		public Vector3 position;

		public bool TouchDetected()
		{
			return phase == TouchPhase.Moved || phase == TouchPhase.Began || phase == TouchPhase.Stationary;
		}
	}
}
