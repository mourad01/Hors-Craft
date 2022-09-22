// DecompilerFi decompiler from Assembly-CSharp.dll class: Ship
using com.ootii.Cameras;
using UnityEngine;

public class Ship : Boat
{
	[HideInInspector]
	public bool dropCraftableAfterDestroy = true;

	public override bool isWaterVehicle => true;

	public override void VehicleActivate(GameObject player)
	{
		base.VehicleActivate(player);
		body.constraints = (RigidbodyConstraints)80;
		CameraController.instance.MainCamera.GetComponent<Animator>().enabled = false;
		CameraController.instance.IsCollisionsEnabled = false;
		EnablePlayerScripts(player, enable: false);
	}

	public override void StopUsing()
	{
		base.StopUsing();
		body.constraints = (RigidbodyConstraints)84;
		CameraController.instance.MainCamera.GetComponent<Animator>().enabled = true;
		CameraController.instance.IsCollisionsEnabled = true;
		EnablePlayerScripts(player, enable: true);
		player.SetActive(value: true);
	}
}
