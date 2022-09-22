// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.ConstantStepRotation
using UnityEngine;

namespace Common.Behaviours
{
	public class ConstantStepRotation : MonoBehaviour
	{
		public bool realtime;

		public float stepDuration = 0.1f;

		public float stepAngleOffset = 45f;

		public Vector3 rotationDirection = new Vector3(0f, 0f, 1f);

		private float nextCheck;

		private void OnValidate()
		{
			rotationDirection.x = Mathf.Clamp(rotationDirection.x, -1f, 1f);
			rotationDirection.y = Mathf.Clamp(rotationDirection.y, -1f, 1f);
			rotationDirection.z = Mathf.Clamp(rotationDirection.z, -1f, 1f);
		}

		private void Update()
		{
			if (!realtime)
			{
				Check(Time.time);
			}
			else
			{
				Check(Time.realtimeSinceStartup);
			}
		}

		private void Check(float currentTime)
		{
			if (currentTime > nextCheck)
			{
				nextCheck = currentTime + stepDuration;
				base.transform.Rotate(rotationDirection * stepAngleOffset);
			}
		}
	}
}
