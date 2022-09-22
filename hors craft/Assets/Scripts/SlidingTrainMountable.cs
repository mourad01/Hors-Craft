// DecompilerFi decompiler from Assembly-CSharp.dll class: SlidingTrainMountable
using com.ootii.Cameras;
using Uniblocks;
using UnityEngine;

public class SlidingTrainMountable : TrainMountable
{
	[SerializeField]
	private float cameraRotationSpeed = 4.5f;

	[SerializeField]
	private bool hasToAutoRide;

	[Header("FOV Settings")]
	public float speedFOVTreshold = 0.8f;

	public float maxFOVChange = 20f;

	protected float currentFOV;

	protected float defaultFOV;

	protected float FOVChange;

	protected PlayerGraphic playerGraphic;

	protected Transform cameraParent;

	protected Camera cam;

	protected Vector3 cameraPositon;

	protected bool isMounted;

	public override void MountVehicle(Transform transformPlayer)
	{
		base.MountVehicle(transformPlayer);
		playerTransform = transformPlayer;
		cam = CameraController.instance.MainCamera;
		playerGraphic = playerTransform.GetComponentInChildren<PlayerGraphic>();
		playerGraphic.HideBodyAndLegs();
		playerGraphic.HideHands();
		defaultFOV = (currentFOV = cam.fieldOfView);
		FOVChange = speedMultiplier * speedFOVTreshold;
		isMounted = true;
	}

	public void Update()
	{
		if (hasToAutoRide && isMounted)
		{
			Move(null, new Vector3(1f, 0f, 1f), 1f);
		}
	}

	public override void Unmount()
	{
		base.Unmount();
		playerGraphic.ShowBodyAndLegs();
		playerGraphic.ShowHands();
		CameraController.instance.SetDefaultCameraPreset();
		cam.fieldOfView = defaultFOV;
		isMounted = false;
	}

	public override void Move(Transform pTransform, Vector3 moveDir, float vertical)
	{
		vertical = Mathf.Clamp01(vertical);
		base.Move(pTransform, moveDir, vertical);
		playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, base.transform.rotation, Time.deltaTime * cameraRotationSpeed);
		ApplyFOV();
	}

	protected void ApplyFOV()
	{
		if (FOVChange != 0f)
		{
			float t = Mathf.Clamp(lastSpeed - FOVChange, 0f, 5500f) / FOVChange;
			cam.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV + maxFOVChange, t);
		}
	}

	public override Vector3 VehicleMoveDirection(Vector3 moveDir, float vertical)
	{
		if (vertical != 0f)
		{
			lastPlayerInput = vertical;
		}
		if (vertical == 0f)
		{
			return Vector3.zero;
		}
		base.transform.rotation = vehicleInitRotation;
		vehicleForward = base.transform.forward;
		vehicleRight = base.transform.right;
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position);
		if (voxelInfo != null && vertical != 0f)
		{
			VoxelInfo forwardVoxelInfo = Engine.PositionToVoxelInfo(base.transform.position + vehicleForward);
			VoxelInfo backwardVoxelInfo = Engine.PositionToVoxelInfo(base.transform.position - vehicleForward);
			moveDir = MoveDirection(voxelInfo, forwardVoxelInfo, backwardVoxelInfo, moveDir);
		}
		return moveDir;
	}

	public override void RotateSelf()
	{
		base.gameObject.transform.Rotate(0f, 180f, 0f);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
	}
}
