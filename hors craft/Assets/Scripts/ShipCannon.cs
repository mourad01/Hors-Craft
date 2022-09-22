// DecompilerFi decompiler from Assembly-CSharp.dll class: ShipCannon
using com.ootii.Cameras;
using UnityEngine;

public class ShipCannon : TankCannon
{
	private Vector2 screenCenter;

	private static LayerMask notPlayerCar;

	protected override void Start()
	{
		base.Start();
		screenCenter = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
		notPlayerCar = -2049;
	}

	public override void UpdateActiveWeapon()
	{
		Ray ray = CameraController.instance.MainCamera.ScreenPointToRay(screenCenter);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, distance * 50f, notPlayerCar, QueryTriggerInteraction.Ignore))
		{
			Vector3 normalized = (hitInfo.point - horizontalRotationObject.transform.position).normalized;
			horizontalRotationObject.transform.forward = new Vector3(normalized.x, 0f, normalized.z).normalized;
			verticalRotationObject.transform.forward = normalized;
		}
		else
		{
			base.UpdateActiveWeapon();
		}
	}

	protected override void UpdateCrosshair()
	{
		if (!(base.crosshairToMove == null))
		{
			base.crosshairToMove.transform.position = screenCenter;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			Ray ray = CameraController.instance.MainCamera.ScreenPointToRay(screenCenter);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, distance * 50f, notPlayerCar, QueryTriggerInteraction.Ignore))
			{
				Gizmos.color = Color.black;
				Gizmos.DrawRay(horizontalRotationObject.transform.position, (hitInfo.point - horizontalRotationObject.transform.position).normalized * distance);
			}
			else
			{
				Gizmos.color = Color.red;
				Gizmos.DrawRay(horizontalRotationObject.transform.position, horizontalRotationObject.transform.forward * 50f);
			}
		}
	}
}
