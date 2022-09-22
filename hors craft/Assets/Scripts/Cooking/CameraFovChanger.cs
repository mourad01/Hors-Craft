// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.CameraFovChanger
using UnityEngine;

namespace Cooking
{
	[RequireComponent(typeof(Camera))]
	public class CameraFovChanger : MonoBehaviour
	{
		public float minFov = 51f;

		public float maxFov = 75f;

		private const float MAX_RATIO = 2f;

		private const float MIN_RATIO = 1.25f;

		private void Start()
		{
			float num = (float)Screen.width / (float)Screen.height;
			float num2 = 0.75f;
			float num3 = Mathf.Clamp(num - 1.25f, 0f, num2);
			float t = Mathf.Clamp(num3 / num2, 0f, 1f);
			float fieldOfView = Mathf.Lerp(maxFov, minFov, t);
			GetComponent<Camera>().fieldOfView = fieldOfView;
		}
	}
}
