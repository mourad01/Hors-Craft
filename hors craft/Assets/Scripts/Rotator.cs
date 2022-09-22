// DecompilerFi decompiler from Assembly-CSharp.dll class: Rotator
using UnityEngine;

public class Rotator : MonoBehaviour
{
	[SerializeField]
	private float rotataionSpeed;

	[SerializeField]
	private Vector3 rotationAxis;

	private Transform myTransform;

	private void Awake()
	{
		myTransform = GetComponent<Transform>();
	}

	private void Update()
	{
		myTransform.Rotate(rotationAxis, rotataionSpeed * Time.unscaledDeltaTime);
	}
}
