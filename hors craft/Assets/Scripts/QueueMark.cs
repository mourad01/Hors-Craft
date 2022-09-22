// DecompilerFi decompiler from Assembly-CSharp.dll class: QueueMark
using Common.Utils;
using UnityEngine;

public class QueueMark : MonoBehaviour
{
	private GameObject pivot;

	private Vector3 offset;

	private Camera cam;

	public void Init(GameObject pivot, Camera cam)
	{
		this.cam = cam;
		this.pivot = pivot;
		offset = RenderersBounds.MiddlePoint(pivot);
	}

	private void Update()
	{
		Vector3 position = cam.WorldToViewportPoint(pivot.transform.position + offset);
		position.x *= Screen.width;
		position.y *= Screen.height;
		position.z = 0f;
		base.transform.position = position;
	}
}
