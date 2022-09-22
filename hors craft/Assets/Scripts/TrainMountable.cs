// DecompilerFi decompiler from Assembly-CSharp.dll class: TrainMountable
using com.ootii.Cameras;
using Common.Managers;
using CommonAttribute;
using Uniblocks;
using UnityEngine;

public class TrainMountable : Mountable
{
	[Header("Train Stuffs")]
	public CameraController.CameraPresets onActivateCameraPreset = CameraController.CameraPresets.FPP_CarDriving;

	public bool forceSnapToRails;

	protected Transform playerTransform;

	[ReadOnly]
	public Quaternion vehicleInitRotation;

	[ReadOnly]
	public Vector3 belowVoxelPosition = Vector3.zero;

	protected float lastPlayerInput;

	[ReadOnly]
	public Vector3 vehicleForward;

	[ReadOnly]
	public Vector3 vehicleRight;

	[ReadOnly]
	public TrainMountable wagonPrev;

	[ReadOnly]
	public TrainMountable wagonNext;

	public Voxel[] canMoveOnTracks;

	public bool canBeControlled = true;

	public bool followTrain;

	public float traveledDistance;

	public float maxDistanceToNextWagon = 1f;

	[ReadOnly]
	public CharacterController controller;

	private float characterRadius;

	[ReadOnly]
	public GameObject[] otherWagons;

	public BoxCollider bumperCollider;

	public float speedMultiplierByDistance = 1f;

	[Header("Acceleration settings")]
	public bool hasAcceleration;

	public float sqrtTurningTreshold = 0.1f;

	public float speedUpPower = 2f;

	public float speedDownPower = 3f;

	public float turnPower = 3f;

	public float stopTreshold = 0.2f;

	protected float lastSpeed;

	protected Vector3 lastMoveDirection = Vector3.zero;

	[Header("Camera settings")]
	public Vector3 cameraOffset = new Vector3(0f, 0.9f, 0f);

	private Train _vehicle;

	public bool isChooChoo => !followTrain;

	public Train vehicle
	{
		get
		{
			return _vehicle;
		}
		set
		{
			_vehicle = value;
		}
	}

	public override void MountVehicle(Transform transformPlayer)
	{
		if (!isChooChoo)
		{
			UnmountWagon();
		}
		if ((bool)GetComponentInChildren<TrainInterior>())
		{
			GetComponentInChildren<TrainInterior>().EnableInterior(enabled: true);
		}
		playerTransform = transformPlayer;
		PlayerGraphic.GetControlledPlayerInstance().HidePlayerGraphic();
		Vector3 position = base.transform.position;
		transformPlayer.rotation = base.transform.rotation;
		vehicleInitRotation = base.transform.rotation;
		vehicle.mounted = true;
		MountAndPosition(playerTransform);
		vehicle.connectToTrain = true;
		GetComponent<BoxCollider>().isTrigger = true;
		GetComponent<BoxCollider>().enabled = true;
		controller = playerTransform.GetComponent<CharacterController>();
		characterRadius = controller.radius;
		controller.radius = 0.1f;
		playerTransform.position = position + Vector3.up;
		playerTransform.forward = base.transform.forward;
		Vector3 vector = base.transform.localPosition;
		vector.y += GetMountHeight();
		vector -= GetComponent<BoxCollider>().center;
		base.transform.localPosition = vector;
		CameraController.instance.SetCameraPreset(onActivateCameraPreset);
	}

	public override void Unmount()
	{
		if ((bool)GetComponentInChildren<TrainInterior>())
		{
			GetComponentInChildren<TrainInterior>().EnableInterior(enabled: false);
		}
		PlayerGraphic.GetControlledPlayerInstance().ShowPlayerGraphic();
		playerTransform.GetComponentInChildren<PlayerMovement>().UpdateCurrentMode();
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position);
		belowVoxelPosition = Engine.VoxelInfoToPosition(voxelInfo);
		vehicle.connectToTrain = false;
		SaveTransform componentInChildren = GetComponentInChildren<SaveTransform>();
		if (componentInChildren != null)
		{
			base.transform.parent = componentInChildren.module.transformsParent;
		}
		else
		{
			base.transform.parent = Manager.Get<SaveTransformsManager>().transform;
		}
		vehicle.mounted = false;
		controller.radius = characterRadius;
		controller = GetComponent<CharacterController>();
		controller.enabled = true;
		CameraController.instance.SetDefaultCameraPreset();
	}

	private void Update()
	{
		if (wagonPrev != null && Vector3.Distance(base.transform.position, wagonPrev.transform.position) > 4f)
		{
			UnmountWagon();
		}
		CheckWagons();
	}

	public virtual void UnmountWagon()
	{
		if (wagonPrev != null)
		{
			wagonPrev.vehicle.connectToTrain = false;
			wagonPrev.wagonNext = null;
			wagonPrev.UnmountWagon();
			wagonPrev = null;
		}
		if (wagonNext != null)
		{
			wagonNext.vehicle.connectToTrain = false;
			wagonNext.wagonPrev = null;
			wagonNext.UnmountWagon();
			wagonNext = null;
		}
	}

	public virtual void MountWagon(TrainMountable trainMountable)
	{
	}

	public override void SetCameraPosition(Transform cameraPos)
	{
		cameraPos.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, cameraOffset.z);
	}

	public override void Move(Transform pTransform, Vector3 moveDir, float vertical)
	{
		if (!canBeControlled)
		{
			return;
		}
		speedMultiplierByDistance = AutoRepairSpeedByDistances(vertical);
		Vector3 vector = VehicleMoveDirection(moveDir, vertical);
		Vector3 zero = Vector3.zero;
		if (hasAcceleration)
		{
			zero = lastMoveDirection * AccelSpeed(vector) * speedMultiplierByDistance;
			if (!(vector == Vector3.zero) || !(lastSpeed > stopTreshold))
			{
				lastMoveDirection = vector;
			}
			else
			{
				lastMoveDirection = VehicleMoveDirection(new Vector3(1f, 0f, 1f), 1f);
			}
		}
		else
		{
			lastMoveDirection = vector;
			zero = lastMoveDirection * speedMultiplier * speedMultiplierByDistance;
		}
		controller.SimpleMove(zero);
	}

	protected virtual float AccelSpeed(Vector3 currentDirection)
	{
		float a = lastSpeed;
		return lastSpeed = ((currentDirection == Vector3.zero) ? Mathf.Lerp(a, 0f, Time.deltaTime * speedDownPower) : ((!(Vector3.Distance(currentDirection, lastMoveDirection) > sqrtTurningTreshold)) ? Mathf.Lerp(a, speedMultiplier, Time.deltaTime * speedUpPower) : Mathf.Lerp(a, 0f, Time.deltaTime * turnPower)));
	}

	public float AutoRepairSpeedByDistances(float dir)
	{
		Vector3 vector = Vector3.zero;
		if (wagonNext != null && dir > 0f)
		{
			vector = wagonNext.transform.position;
		}
		else if (wagonPrev != null && dir < 0f)
		{
			vector = wagonPrev.transform.position;
		}
		if (vector != Vector3.zero)
		{
			Vector3 position = base.transform.position;
			vector.y = position.y;
			float num = Vector3.Distance(vector, base.transform.position);
			if (num > maxDistanceToNextWagon)
			{
				return 1.2f;
			}
			return 0.8f;
		}
		return 1f;
	}

	public virtual bool MoveBy(float distance, float inputDir)
	{
		return false;
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
		if (Physics.Raycast(base.transform.position + Vector3.up / 2f, vertical * base.transform.forward * 2f, out RaycastHit hitInfo, 1f, 0, QueryTriggerInteraction.Ignore))
		{
			TrainMountable componentInParent = hitInfo.transform.GetComponentInParent<TrainMountable>();
			if (componentInParent != null && componentInParent != this)
			{
				if (componentInParent.isChooChoo)
				{
					if (vertical > 0f && wagonNext == null)
					{
						return Vector3.zero;
					}
					if (vertical < 0f && wagonPrev == null)
					{
						return Vector3.zero;
					}
				}
				else
				{
					if (!isChooChoo && wagonNext == null && wagonPrev == null)
					{
						return Vector3.zero;
					}
					if (isChooChoo && vertical > 0f)
					{
						UnityEngine.Debug.Log(hitInfo.transform.name);
						return Vector3.zero;
					}
				}
			}
		}
		else if (isChooChoo && Physics.Raycast(base.transform.position + Vector3.up * 2f, vertical * base.transform.forward, out hitInfo, 1f, 0, QueryTriggerInteraction.Ignore))
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
		if (moveDir.magnitude != 0f && !isChooChoo)
		{
			if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.z))
			{
				Vector3 position = base.transform.position;
				position.z = belowVoxelPosition.z;
				base.transform.position = position;
			}
			else
			{
				Vector3 position2 = base.transform.position;
				position2.x = belowVoxelPosition.x;
				base.transform.position = position2;
			}
		}
		return moveDir;
	}

	public virtual Vector3 MoveDirection(VoxelInfo belowVoxelInfo, VoxelInfo forwardVoxelInfo, VoxelInfo backwardVoxelInfo, Vector3 moveDir)
	{
		ushort uniqueID = belowVoxelInfo.GetVoxelType().GetUniqueID();
		float num = Mathf.Sign(lastPlayerInput);
		if (uniqueID == 0)
		{
			belowVoxelInfo = Engine.PositionToVoxelInfo(base.transform.position - base.transform.up);
			uniqueID = belowVoxelInfo.GetVoxelType().GetUniqueID();
		}
		AutoConnectionRailsVoxelEvents component = belowVoxelInfo.GetVoxelType().GetComponent<AutoConnectionRailsVoxelEvents>();
		if (GetRailsEngineIds(uniqueID))
		{
			belowVoxelPosition = Engine.VoxelInfoToPosition(belowVoxelInfo);
			Vector3[] inOutDirections = component.GetInOutDirections(belowVoxelInfo.GetVoxelRotation());
			Vector3[] array = new Vector3[2]
			{
				new Vector3(inOutDirections[0].x, 0f, inOutDirections[0].z),
				new Vector3(inOutDirections[1].x, 0f, inOutDirections[1].z)
			};
			Vector3 vector = belowVoxelPosition + inOutDirections[0] / 2f;
			Vector3 vector2 = belowVoxelPosition + inOutDirections[1] / 2f;
			if (num * -vehicleForward == array[0])
			{
				float x = vector.x;
				Vector3 position = base.transform.position;
				float num2 = x - position.x;
				float z = vector.z;
				Vector3 position2 = base.transform.position;
				traveledDistance = Mathf.Abs(num2 - (z - position2.z));
			}
			else if (num * -vehicleForward == array[1])
			{
				float x2 = vector2.x;
				Vector3 position3 = base.transform.position;
				float num3 = x2 - position3.x;
				float z2 = vector2.z;
				Vector3 position4 = base.transform.position;
				traveledDistance = Mathf.Abs(num3 - (z2 - position4.z));
			}
			if (traveledDistance >= 0.5f)
			{
				for (int i = 0; i < 2; i++)
				{
					if (array[i] == num * -vehicleForward)
					{
						if (array[(i + 1) % 2] == vehicleRight)
						{
							Turn(num * 1f);
						}
						else if (array[(i + 1) % 2] == -vehicleRight)
						{
							Turn(num * -1f);
						}
					}
				}
				moveDir = CheckOtherSides(num, backwardVoxelInfo, forwardVoxelInfo, moveDir, component);
			}
		}
		else
		{
			moveDir = CheckOtherSides(num, backwardVoxelInfo, forwardVoxelInfo, moveDir, component);
		}
		if (wagonPrev != null && moveDir != Vector3.zero && !wagonPrev.MoveBy(traveledDistance, num))
		{
			return Vector3.zero;
		}
		if (forceSnapToRails)
		{
			Transform transform = base.transform;
			Vector3 localPosition = base.transform.localPosition;
			transform.localPosition = new Vector3(0f, localPosition.y, 0f);
		}
		return DestDirection(moveDir);
	}

	private Vector3 CheckOtherSides(float inputDir, VoxelInfo backwardVoxelInfo, VoxelInfo forwardVoxelInfo, Vector3 moveDir, AutoConnectionRailsVoxelEvents railsEvents)
	{
		if (inputDir < 0f && backwardVoxelInfo != null)
		{
			return VoxelCheck(moveDir, backwardVoxelInfo, railsEvents, inputDir);
		}
		if (inputDir > 0f && forwardVoxelInfo != null)
		{
			return VoxelCheck(moveDir, forwardVoxelInfo, railsEvents, inputDir);
		}
		return moveDir;
	}

	public virtual Vector3 VoxelCheck(Vector3 dir, VoxelInfo voxelInfo, AutoConnectionRailsVoxelEvents belowRailsEvents, float inputDir)
	{
		AutoConnectionRailsVoxelEvents component = voxelInfo.GetVoxelType().GetComponent<AutoConnectionRailsVoxelEvents>();
		if (component != null)
		{
			Vector3[] inOutDirections = component.GetInOutDirections(voxelInfo.GetVoxelRotation());
			Vector3[] array = new Vector3[2]
			{
				new Vector3(inOutDirections[0].x, 0f, inOutDirections[0].z),
				new Vector3(inOutDirections[1].x, 0f, inOutDirections[1].z)
			};
			for (int i = 0; i < 2; i++)
			{
				if (array[i] != inputDir * -vehicleForward && array[(i + 1) % 2] != inputDir * -vehicleForward)
				{
					dir = Vector3.zero;
				}
			}
		}
		else if (component == null && belowRailsEvents == null)
		{
			return Vector3.zero;
		}
		return dir;
	}

	public bool GetRailsEngineIds(ushort voxelId)
	{
		Voxel[] array = canMoveOnTracks;
		foreach (Voxel voxel in array)
		{
			if (voxel.GetUniqueID() == voxelId)
			{
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
		vehicle = GetComponent<Train>();
		controller = GetComponent<CharacterController>();
	}

	private void FixedUpdate()
	{
		if (controller.enabled && !controller.isGrounded)
		{
			controller.Move(Vector3.zero + Physics.gravity);
		}
	}

	private void LateUpdate()
	{
		if (vehicle.mounted)
		{
			base.transform.rotation = vehicleInitRotation;
		}
	}

	public void Turn(float rightDir)
	{
		float num = 90f;
		if (rightDir < 0f)
		{
			num *= -1f;
		}
		if (lastPlayerInput < 0f)
		{
			rightDir *= -1f;
		}
		SnapToBelowVoxel();
		base.transform.Rotate(0f, num, 0f);
		vehicleInitRotation = base.transform.rotation;
	}

	public void SnapToBelowVoxel()
	{
		if (isChooChoo)
		{
			Transform transform = playerTransform;
			float x = belowVoxelPosition.x;
			Vector3 position = playerTransform.position;
			transform.position = new Vector3(x, position.y, belowVoxelPosition.z);
		}
		else
		{
			Transform transform2 = base.transform;
			float x2 = belowVoxelPosition.x;
			Vector3 position2 = base.transform.position;
			transform2.position = new Vector3(x2, position2.y, belowVoxelPosition.z);
		}
	}

	public Vector3 DestDirection(Vector3 moveDir)
	{
		Vector3 vector = moveDir;
		Vector3 forward = base.transform.forward;
		if (vector != Vector3.zero)
		{
			if (lastPlayerInput > 0f)
			{
				vector = forward;
			}
			else if (lastPlayerInput < 0f)
			{
				vector = forward * -1f;
			}
		}
		return vector;
	}

	public void CheckWagons()
	{
		if (!(wagonPrev == null) || (!(wagonNext != null) && !isChooChoo))
		{
			return;
		}
		otherWagons = GameObject.FindGameObjectsWithTag("TrainCollider");
		if (otherWagons == null || otherWagons.Length <= 0)
		{
			return;
		}
		Transform closestWagon = GetClosestWagon(otherWagons);
		if (!(closestWagon != null))
		{
			return;
		}
		Bounds bounds = closestWagon.GetComponent<BoxCollider>().bounds;
		Bounds bounds2 = bumperCollider.bounds;
		if (bounds.Intersects(bounds2) && wagonPrev == null)
		{
			WagonMountable component = closestWagon.GetComponent<WagonMountable>();
			if (component != null)
			{
				wagonPrev = component;
				component.MountWagon(this);
			}
		}
	}

	public Transform GetClosestWagon(GameObject[] wagons)
	{
		Transform result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		foreach (GameObject gameObject in wagons)
		{
			if (!(gameObject == base.gameObject) && !(gameObject.GetComponent<TrainMountable>().wagonNext != null) && !gameObject.GetComponent<TrainMountable>().isChooChoo)
			{
				float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = gameObject.transform;
				}
			}
		}
		return result;
	}

	public override TrainMountable GetClosestTrain()
	{
		TrainMountable[] array = (TrainMountable[])Object.FindObjectsOfType(typeof(TrainMountable));
		TrainMountable result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		Vector3 zero = Vector3.zero;
		float num2 = 0f;
		TrainMountable[] array2 = array;
		foreach (TrainMountable trainMountable in array2)
		{
			if (!(trainMountable == base.gameObject) && trainMountable.isChooChoo)
			{
				num2 = (trainMountable.transform.position - position).sqrMagnitude;
				if (num2 < num)
				{
					num = num2;
					result = trainMountable;
				}
			}
		}
		return result;
	}

	private void OnDestroy()
	{
		if (wagonPrev != null)
		{
			wagonPrev.UnmountWagon();
		}
	}

	public virtual void RotateSelf()
	{
		base.gameObject.transform.Rotate(0f, 90f, 0f);
	}
}
