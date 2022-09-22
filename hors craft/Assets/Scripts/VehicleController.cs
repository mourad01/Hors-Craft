// DecompilerFi decompiler from Assembly-CSharp.dll class: VehicleController
using com.ootii.Cameras;
using Common.Utils;
using States;
using System;
using Uniblocks;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
	[Header("Reference")]
	public GameObject colliderStatic;

	[Header("Params")]
	public CameraController.CameraPresets onActivateCameraPreset = CameraController.CameraPresets.FPP_CarDriving;

	public bool renderPlayer = true;

	public GameplayState.Substates substateToPush = GameplayState.Substates.VEHICLE;

	public float friction = 0.95f;

	public AnimationCurve fovChangeType;

	protected bool isInUse;

	protected int spawnedByBlock = -1;

	protected float remeberedFov;

	protected int layerMask;

	private Vector3 lastPosition;

	protected GameObject player;

	protected Camera vehicleCamera;

	public Action<GameObject> OnVehicleActivated;

	public Action OnVehicleUnactivated;

	public bool IsInUse => isInUse;

	public int SpawnedByBlock => spawnedByBlock;

	public virtual bool isWaterVehicle => false;

	public virtual void VehicleActivate(GameObject player)
	{
		this.player = player;
		player.GetComponent<PlayerController>().isInVehicle = true;
		if (OnVehicleActivated != null)
		{
			OnVehicleActivated(player);
		}
	}

	public virtual void StopUsing()
	{
		PlayerController component = player.GetComponent<PlayerController>();
		component.isInVehicle = false;
		component.isInFlyingVehicle = false;
		if (OnVehicleUnactivated != null)
		{
			OnVehicleUnactivated();
		}
	}

	public void InitLayerMask()
	{
		layerMask = 1 << LayerMask.NameToLayer("Vehicle");
		layerMask = ~layerMask;
	}

	public virtual void InitAfterSpawnByPlayer(int blockSpawn)
	{
		spawnedByBlock = blockSpawn;
	}

	public void InformOnMovement()
	{
		float num = Vector3.Distance(lastPosition, base.transform.position);
		if (num > 0f)
		{
			UpdatePlayerQuests(num);
		}
		lastPosition = base.gameObject.transform.position;
	}

	protected virtual void UpdatePlayerQuests(float distance)
	{
		Singleton<PlayerData>.get.playerQuests.InformOnPlayerWalk(base.transform.position, distance, PlayerMovement.Mode.MOUNTED_FLYING, inWater: false);
	}

	public void SetVehicleFov()
	{
		if (!(vehicleCamera == null))
		{
			remeberedFov = vehicleCamera.fieldOfView;
			vehicleCamera.fieldOfView = fovChangeType.Evaluate(0f);
		}
	}

	protected virtual void UpdateFov(float factor)
	{
		if (!(vehicleCamera == null))
		{
			vehicleCamera.fieldOfView = Mathf.Lerp(vehicleCamera.fieldOfView, Easing.Ease(EaseType.OutCubic, 70f, 90f, factor), 0.1f);
			if (float.IsNaN(vehicleCamera.fieldOfView))
			{
				vehicleCamera.fieldOfView = 70f;
			}
		}
	}

	public void RestoreVehicleFov()
	{
		if (!(vehicleCamera == null))
		{
			vehicleCamera.fieldOfView = remeberedFov;
		}
	}

	protected void EnablePlayerScripts(GameObject player, bool enable)
	{
		if (!enable)
		{
			CameraController.instance.SetCameraPreset(onActivateCameraPreset);
		}
		else
		{
			CameraController.instance.SetDefaultCameraPreset();
		}
		player.GetComponent<PlayerController>().EnablePlayerVehicleScripts(enable);
	}

	public virtual void OnVehicleDestroy()
	{
		if (isInUse)
		{
			StopUsing();
		}
		if (spawnedByBlock > 0)
		{
			Singleton<PlayerData>.get.playerItems.AddToBlocks(spawnedByBlock, 1);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public virtual void Dispose()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected void SetParent(Transform parent)
	{
		CameraController.instance.Anchor.transform.SetParent(parent);
		CameraController.instance.Anchor.transform.localPosition = Vector3.zero;
		CameraController.instance.Anchor.transform.localEulerAngles = Vector3.zero;
		CameraController.instance.Anchor.transform.localScale = Vector3.one;
	}

	protected bool DestroyElement<TVal>() where TVal : Component
	{
		Component component = GetComponent<TVal>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
			return true;
		}
		return false;
	}
}
