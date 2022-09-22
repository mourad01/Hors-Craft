// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.PermanentCameraGizmo
using UnityEngine;

namespace Common.Behaviours
{
	[RequireComponent(typeof(Camera))]
	public class PermanentCameraGizmo : MonoBehaviour
	{
		public Color color = Color.white;

		public float maxFar = 1000f;

		public float maxAspect = 2f;

		private Camera cam;

		public Camera GetCamera()
		{
			if (cam == null)
			{
				cam = GetComponent<Camera>();
			}
			return cam;
		}
	}
}
