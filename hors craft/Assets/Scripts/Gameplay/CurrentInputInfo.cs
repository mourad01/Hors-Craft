// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CurrentInputInfo
using UnityEngine;

namespace Gameplay
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
