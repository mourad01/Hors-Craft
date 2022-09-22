// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.RotateTowardsCamera
using UnityEngine;

namespace Common.Behaviours
{
	public class RotateTowardsCamera : MonoBehaviour
	{
		public bool realtime = true;

		public bool fixRotationX;

		public bool fixRotationY;

		public bool fixRotationZ;

		private Transform mainCameraTransform;

		private const float checkInterval = 0.02f;

		private float nextCheckTime;

		private void Awake()
		{
			mainCameraTransform = Camera.main.transform;
		}

		private void FixedUpdate()
		{
			if (!realtime)
			{
				Check();
			}
		}

		private void Update()
		{
			if (realtime && Time.realtimeSinceStartup > nextCheckTime)
			{
				nextCheckTime = Time.realtimeSinceStartup + 0.02f;
				Check();
			}
		}

		private void Check()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			base.transform.LookAt(mainCameraTransform);
			Transform transform = base.transform;
			float x;
			if (fixRotationX)
			{
				x = eulerAngles.x;
			}
			else
			{
				Vector3 eulerAngles2 = base.transform.eulerAngles;
				x = eulerAngles2.x;
			}
			float y;
			if (fixRotationY)
			{
				y = eulerAngles.y;
			}
			else
			{
				Vector3 eulerAngles3 = base.transform.eulerAngles;
				y = eulerAngles3.y;
			}
			float z;
			if (fixRotationZ)
			{
				z = eulerAngles.z;
			}
			else
			{
				Vector3 eulerAngles4 = base.transform.eulerAngles;
				z = eulerAngles4.z;
			}
			transform.eulerAngles = new Vector3(x, y, z);
		}
	}
}
