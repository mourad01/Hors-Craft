// DecompilerFi decompiler from Assembly-CSharp.dll class: WagonMountable
using Uniblocks;
using UnityEngine;

public class WagonMountable : TrainMountable
{
	public bool isGrounded = true;

	public override void MountWagon(TrainMountable trainMountable)
	{
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position);
		Vector3 a = Engine.VoxelInfoToPosition(voxelInfo);
		AutoConnectionRailsVoxelEvents component = voxelInfo.GetVoxelType().GetComponent<AutoConnectionRailsVoxelEvents>();
		if (component == null)
		{
			voxelInfo = Engine.PositionToVoxelInfo(base.transform.position - Vector3.up);
			a = Engine.VoxelInfoToPosition(voxelInfo);
			component = voxelInfo.GetVoxelType().GetComponent<AutoConnectionRailsVoxelEvents>();
		}
		Vector3[] inOutDirections = component.GetInOutDirections(voxelInfo.GetVoxelRotation());
		Vector3 vector = a + inOutDirections[0];
		Vector3 vector2 = a + inOutDirections[1];
		float num = 0.0001f;
		Vector3 vector3 = Engine.VoxelInfoToPosition(Engine.PositionToVoxelInfo(trainMountable.transform.position));
		if (Mathf.Abs(vector.x - vector3.x) < num && Mathf.Abs(vector.z - vector3.z) < num)
		{
			base.transform.forward = new Vector3(inOutDirections[0].x, 0f, inOutDirections[0].z);
		}
		else if (Mathf.Abs(vector2.x - vector3.x) < num && Mathf.Abs(vector2.z - vector3.z) < num)
		{
			base.transform.forward = new Vector3(inOutDirections[1].x, 0f, inOutDirections[1].z);
		}
		else
		{
			base.transform.forward = trainMountable.transform.forward;
		}
		vehicleInitRotation = base.transform.rotation;
		base.vehicle.mounted = false;
		base.vehicle.connectToTrain = true;
		isGrounded = false;
		wagonNext = trainMountable;
		controller.radius = 0.2f;
	}

	public override bool MoveBy(float distance, float inputDir)
	{
		speedMultiplierByDistance = AutoRepairSpeedByDistances(inputDir);
		Vector3 vector = VehicleMoveDirection(inputDir * base.transform.forward, inputDir) * speedMultiplier * speedMultiplierByDistance;
		if (controller.isGrounded)
		{
			controller.Move(vector / 50f + Physics.gravity);
		}
		if (vector != Vector3.zero)
		{
			return true;
		}
		return false;
	}

	private void Awake()
	{
		base.vehicle = GetComponent<Train>();
		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		CheckWagons();
	}

	private void FixedUpdate()
	{
		isGrounded = controller.isGrounded;
		if (!controller.isGrounded || (wagonPrev == null && wagonNext == null))
		{
			controller.Move(Vector3.zero + Physics.gravity);
		}
	}

	private void LateUpdate()
	{
		if (base.vehicle.mounted || wagonNext != null || wagonPrev != null)
		{
			base.transform.rotation = vehicleInitRotation;
		}
	}
}
