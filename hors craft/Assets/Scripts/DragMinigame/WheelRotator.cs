// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.WheelRotator
using System;
using UnityEngine;

namespace DragMinigame
{
	public class WheelRotator : MonoBehaviour
	{
		private Transform[] wheels;

		private float wheelGirth;

		public void Init()
		{
			Transform transform = base.transform.FindChildRecursively("Wheels");
			wheels = new Transform[transform.childCount];
			for (int i = 0; i < transform.childCount; i++)
			{
				wheels[i] = transform.GetChild(i);
			}
			Vector3 size = wheels[0].GetComponent<MeshRenderer>().bounds.size;
			float z = size.z;
			wheelGirth = z * (float)Math.PI;
		}

		public void UpdateWheels(float currentTranslation)
		{
			float d = 360f * currentTranslation / wheelGirth / 1.5f;
			Transform[] array = wheels;
			foreach (Transform transform in array)
			{
				transform.Rotate(Vector3.right * d);
			}
		}
	}
}
