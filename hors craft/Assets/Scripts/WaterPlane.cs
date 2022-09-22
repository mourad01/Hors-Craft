// DecompilerFi decompiler from Assembly-CSharp.dll class: WaterPlane
using UnityEngine;

public class WaterPlane : MonoBehaviour
{
	[SerializeField]
	private float planePosY = 10.45f;

	private Transform followCamera;

	private Transform waterplaneTransform;

	private void Awake()
	{
		followCamera = Camera.main.transform;
		waterplaneTransform = base.transform;
	}

	private void Update()
	{
		waterplaneTransform.position = followCamera.position.WithY(planePosY);
	}
}
