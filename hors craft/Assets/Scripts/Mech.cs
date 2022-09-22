// DecompilerFi decompiler from Assembly-CSharp.dll class: Mech
using Common.Managers;
using UnityEngine;

public class Mech : Mountable
{
	public GameObject mainMesh;

	public GameObject interiorMesh;

	public bool hasToHideBody;

	[Header("Camera settings")]
	public Vector3 cameraOffset;

	protected Transform cameraParentTransform;

	protected Vector3 cameraSavedPositon;

	private Transform player;

	public override bool simulateMovement
	{
		set
		{
		}
	}

	public override void MountMob(Transform playerTransform)
	{
		mainMesh.SetActive(value: false);
		interiorMesh.SetActive(value: true);
		player = playerTransform;
		MountAndPosition(player);
		cameraParentTransform = player.GetComponentInChildren<Camera>().transform.parent;
		cameraSavedPositon = cameraParentTransform.localPosition;
		cameraParentTransform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, cameraOffset.z);
		Invoke("SetupCamera", 0.1f);
		if (hasToHideBody)
		{
			HideBody();
		}
	}

	private void HideBody()
	{
		PlayerGraphic componentInChildren = player.GetComponentInChildren<PlayerGraphic>();
		componentInChildren.HideBodyAndLegs();
	}

	private void ShowBody()
	{
		PlayerGraphic componentInChildren = player.GetComponentInChildren<PlayerGraphic>();
		componentInChildren.ShowBodyAndLegs();
	}

	private void SetupCamera()
	{
		cameraParentTransform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, cameraOffset.z);
	}

	public override void Unmount()
	{
		SaveTransform componentInChildren = GetComponentInChildren<SaveTransform>();
		if (componentInChildren != null)
		{
			base.transform.parent = componentInChildren.module.transformsParent;
		}
		else
		{
			base.transform.parent = Manager.Get<SaveTransformsManager>().transform;
		}
		cameraParentTransform.localPosition = cameraSavedPositon;
		player.transform.position += Vector3.left;
		mainMesh.SetActive(value: true);
		interiorMesh.SetActive(value: false);
		ShowBody();
	}
}
