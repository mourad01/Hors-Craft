// DecompilerFi decompiler from Assembly-CSharp.dll class: Cannon
using com.ootii.Cameras;
using Gameplay;
using Uniblocks;
using UnityEngine;

public class Cannon : VehicleController
{
	[SerializeField]
	private GameObject playerPosition;

	[SerializeField]
	private float rotationSpeed;

	private Rigidbody rigidBody;

	private UniversalAnalogInput analogInput;

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		InitLayerMask();
	}

	private void Update()
	{
		if (isInUse)
		{
			Transform transform = base.transform;
			Vector3 up = Vector3.up;
			Vector2 vector = analogInput.CalculatePosition();
			transform.Rotate(up, vector.x * rotationSpeed * Time.deltaTime);
		}
	}

	public override void VehicleActivate(GameObject player)
	{
		base.VehicleActivate(player);
		isInUse = true;
		base.enabled = true;
		rigidBody.useGravity = false;
		rigidBody.isKinematic = false;
		EnablePlayerScripts(player, enable: false);
		SetCamera();
		analogInput = CameraEventsSender.MainInput;
		colliderStatic.SetActive(value: false);
		CarMob componentInParent = GetComponentInParent<CarMob>();
		if (componentInParent != null && spawnedByBlock < 0)
		{
			componentInParent.HijackCar();
		}
	}

	public override void Dispose()
	{
		UnityEngine.Object.Destroy(base.transform.parent.gameObject);
	}

	public override void OnVehicleDestroy()
	{
		if (isInUse)
		{
			StopUsing();
		}
		if (spawnedByBlock > 0)
		{
			Singleton<PlayerData>.get.playerItems.AddToBlocks(spawnedByBlock, 1);
		}
		CarMob componentInParent = GetComponentInParent<CarMob>();
		if (componentInParent != null)
		{
			componentInParent.OnCarPickup(spawnedByBlock < 0);
		}
		UnityEngine.Object.Destroy(base.transform.parent.gameObject);
	}

	public override void StopUsing()
	{
		base.StopUsing();
		isInUse = false;
		base.enabled = false;
		rigidBody.useGravity = true;
		if (!(player == null))
		{
			player.transform.localPosition = new Vector3(-3f, 4f);
			player.transform.SetParent(null);
			EnablePlayerScripts(player, enable: true);
			RestoreVehicleFov();
			player.transform.localScale = Vector3.one;
			player.transform.localRotation = Quaternion.identity;
			colliderStatic.SetActive(value: true);
		}
	}

	private void SetCamera()
	{
		SetParent(playerPosition.transform);
		vehicleCamera = CameraController.instance.MainCamera;
		SetVehicleFov();
	}
}
