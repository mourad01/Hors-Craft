// DecompilerFi decompiler from Assembly-CSharp.dll class: CameraFacing
using UnityEngine;

public class CameraFacing : MonoBehaviour
{
	public enum Axis
	{
		Up,
		Down,
		Left,
		Right,
		Forward,
		Back
	}

	private Camera _referenceCamera;

	public bool reverseFace;

	public Axis axis;

	private void Awake()
	{
		if (_referenceCamera == null)
		{
			_referenceCamera = Camera.main;
		}
		if (!(_referenceCamera == null))
		{
			Vector3 worldPosition = base.transform.position + _referenceCamera.transform.rotation * ((!reverseFace) ? Vector3.back : Vector3.forward);
			Vector3 worldUp = _referenceCamera.transform.rotation * GetAxis(axis);
			base.transform.LookAt(worldPosition, worldUp);
		}
	}

	private Vector3 GetAxis(Axis refAxis)
	{
		switch (refAxis)
		{
		case Axis.Down:
			return Vector3.down;
		case Axis.Forward:
			return Vector3.forward;
		case Axis.Back:
			return Vector3.back;
		case Axis.Left:
			return Vector3.left;
		case Axis.Right:
			return Vector3.right;
		default:
			return Vector3.up;
		}
	}
}
