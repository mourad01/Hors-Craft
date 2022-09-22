// DecompilerFi decompiler from Assembly-CSharp.dll class: MeshFallerBehaviour
using com.ootii.Cameras;
using UnityEngine;

public class MeshFallerBehaviour : AbstractInteractiveDestroyBehaviour
{
	public GameObject prefab;

	public override void Destroy(GameObject toDestroy)
	{
		GameObject gameObject = Object.Instantiate(prefab, toDestroy.transform.position, toDestroy.transform.rotation);
		Vector3 forward = PlayerGraphic.GetControlledPlayerInstance().transform.forward;
		gameObject.GetComponent<Fall>().Init(toDestroy, forward);
		CameraController.instance.Shake(0.1f, 0.1f);
	}
}
