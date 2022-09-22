// DecompilerFi decompiler from Assembly-CSharp.dll class: CameraCeilingCorrection
using UnityEngine;

public class CameraCeilingCorrection : MonoBehaviour
{
	public Transform cameraParent;

	public float offset = 0.2f;

	private float change;

	private Vector3 defaultPosition;

	private bool insideCeiling;

	private void Update()
	{
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		float y = position2.y - offset;
		Vector3 position3 = base.transform.position;
		if (Physics.Raycast(new Vector3(x, y, position3.z), Vector3.up, out RaycastHit hitInfo, offset * 2f - change, 1 << LayerMask.NameToLayer("Terrain")))
		{
			if (!insideCeiling)
			{
				defaultPosition = cameraParent.localPosition;
				insideCeiling = true;
			}
			float num = change;
			Vector3 point = hitInfo.point;
			float num2 = point.y - offset;
			Vector3 position4 = cameraParent.position;
			change = num + (num2 - position4.y);
			Transform transform = cameraParent;
			Vector3 position5 = cameraParent.position;
			float x2 = position5.x;
			Vector3 point2 = hitInfo.point;
			float y2 = point2.y - offset;
			Vector3 position6 = cameraParent.position;
			transform.position = new Vector3(x2, y2, position6.z);
		}
		else if (insideCeiling)
		{
			insideCeiling = false;
			change = 0f;
			cameraParent.localPosition = defaultPosition;
		}
	}
}
