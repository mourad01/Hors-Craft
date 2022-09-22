// DecompilerFi decompiler from Assembly-CSharp.dll class: StorySystem.CameraFitter
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StorySystem
{
	public class CameraFitter : MonoBehaviour
	{
		[SerializeField]
		private Vector2 center;

		[SerializeField]
		private Vector2 size;

		private Camera mainCamera
		{
			[CompilerGenerated]
			get
			{
				return Camera.main;
			}
		}

		private void Awake()
		{
			FitCamera();
		}

		[ContextMenu("FitCamera")]
		public void FitCamera()
		{
			mainCamera.transform.position = new Vector3(center.x, center.y, -5f);
			float num = (float)Screen.width / (float)Screen.height;
			float num2 = size.x / size.y;
			if (num >= num2)
			{
				mainCamera.orthographicSize = size.y / 2f;
				return;
			}
			float num3 = num2 / num;
			mainCamera.orthographicSize = size.y / 2f * num3;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(new Vector3(center.x, center.y, 0f), new Vector3(size.x, size.y, 0f));
		}
	}
}
