// DecompilerFi decompiler from Assembly-CSharp.dll class: HoverCar
using com.ootii.Cameras;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class HoverCar : VehicleController
{
	[Serializable]
	public class WheelsRotationAxes
	{
		public Vector3 steering = Vector3.forward;

		public Vector3 wheelForward = Vector3.right;

		public Vector3 wheelTurning = Vector3.up;
	}

	[Header("Vehicle Config")]
	[HoverVehicleConfigsDropdown]
	public HoverVehicleConfig config;

	[HoverVehicleSkeletonsDropdown]
	public HoverVehicleSkeleton vehicleSkeletonPrefab;

	public WheelsRotationAxes rotationAxes = new WheelsRotationAxes();

	[Header("References")]
	public Rigidbody body;

	public Transform vehicleSkeletonRoot;

	public GameObject insideCameraPosition;

	[Header("References gfx")]
	public GameObject steeringWheel;

	public GameObject carInterior;

	public GameObject mainMesh;

	public GameObject[] wheels;

	public int[] rotatableWheelIndex = new int[2]
	{
		0,
		1
	};

	protected float thrust;

	protected float turnValue;

	protected float deadZone = 0.1f;

	protected Quaternion initialSteeringWheelRotation;

	private Quaternion[] initialWheelRotations;

	protected HoverVehicleSkeleton skeleton;

	protected UniversalAnalogInput analogInput;

	private float PIx2 = (float)Math.PI * 2f;

	private float traveledDistance;

	[Header("Stop using")]
	public List<Vector3> exitOffsets = new List<Vector3>();

	public float velocity
	{
		get;
		private set;
	}

	protected virtual void Start()
	{
		SetupWheelsInitialRotation();
		SetupRigidbodyFromConfig();
		InitLayerMask();
	}

	protected virtual void Awake()
	{
		if (skeleton == null)
		{
			SpawnSkeleton();
		}
		EnableCarInterior(enable: false);
	}

	protected virtual void Update()
	{
		if (analogInput == null)
		{
			analogInput = CameraEventsSender.MainInput;
			return;
		}
		velocity = body.velocity.magnitude;
		thrust = 0f;
		Vector2 vector = analogInput.CalculatePosition();
		float num = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y) * Mathf.Sign(vector.y);
		num = vector.y;
		if (num > deadZone)
		{
			thrust = num * config.forwardAcceleration;
		}
		else if (num < 0f - deadZone)
		{
			thrust = num * config.reverseAcceleration;
		}
		turnValue = 0f;
		float forwardVelocity = GetForwardVelocity();
		float x = vector.x;
		if (Mathf.Abs(forwardVelocity) > config.turringMinimum)
		{
			turnValue = x * Mathf.Sign(forwardVelocity);
		}
		MoveCarWheel(vector.x);
		CheckPlayerYPosition();
		float factor = Mathf.Clamp01(velocity / 10f);
		UpdateFov(factor);
		InformOnMovement();
	}

	protected virtual void FixedUpdate()
	{
		AddHoverForces();
		AddForces();
		Realign();
	}

	private void OnDrawGizmos()
	{
		if (!(player == null))
		{
			Vector3 a = new Vector3(1f, 1.8f, 1f) / 2f;
			Gizmos.color = Color.blue;
			Vector3 position = default(Vector3);
			foreach (Vector3 exitOffset in exitOffsets)
			{
				position = exitOffset;
				Vector3 center = base.transform.TransformPoint(position);
				Gizmos.DrawWireCube(center, a * 2f);
			}
			if (!(mainMesh == null))
			{
				Vector3 size = mainMesh.GetComponent<MeshRenderer>().bounds.size;
				position = new Vector3(0f - (size.x / 2f + 0.2f), 1f, 0f);
				Vector3 center = base.transform.TransformPoint(position);
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(center, a * 2f);
				position.x *= -1f;
				center = base.transform.TransformPoint(position);
				Gizmos.DrawWireCube(center, a * 2f);
			}
		}
	}

	private void OnDisable()
	{
		velocity = 0f;
	}

	public override void InitAfterSpawnByPlayer(int blockSpawn)
	{
		base.InitAfterSpawnByPlayer(blockSpawn);
		DestroyElement<CarMob>();
		DestroyElement<CarNavigator>();
	}

	public override void Dispose()
	{
		UnityEngine.Object.Destroy(base.transform.gameObject);
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
		CarMob component = GetComponent<CarMob>();
		if (component != null)
		{
			component.OnCarPickup(spawnedByBlock < 0);
		}
		UnityEngine.Object.Destroy(base.transform.gameObject);
	}

	public override void VehicleActivate(GameObject player)
	{
		base.VehicleActivate(player);
		isInUse = true;
		base.enabled = true;
		body.useGravity = false;
		body.isKinematic = false;
		EnablePlayerScripts(player, enable: false);
		analogInput = CameraEventsSender.MainInput;
		SetCamera();
		skeleton.colliderRide.SetActive(value: true);
		colliderStatic.SetActive(value: false);
		CarMob component = GetComponent<CarMob>();
		if (component != null && spawnedByBlock < 0)
		{
			component.HijackCar();
		}
		DestroyElement<CarMob>();
		DestroyElement<CarNavigator>();
		StartCoroutine(ActivateAfter(1f));
	}

	protected override void UpdatePlayerQuests(float distance)
	{
		Singleton<PlayerData>.get.playerQuests.InformOnPlayerWalk(base.transform.position, distance, PlayerMovement.Mode.MOUNTED_VEHICLE, inWater: false);
	}

	public override void StopUsing()
	{
		base.StopUsing();
		isInUse = false;
		base.enabled = false;
		body.useGravity = true;
		if (!(player == null))
		{
			player.transform.localPosition = FindBestExitPlace();
			player.transform.SetParent(null);
			EnablePlayerScripts(player, enable: true);
			RestoreVehicleFov();
			player.transform.localScale = Vector3.one;
			player.transform.localRotation = Quaternion.identity;
			EnableCarInterior(enable: false);
			skeleton.colliderRide.SetActive(value: false);
			colliderStatic.SetActive(value: true);
			StopAllCoroutines();
		}
	}

	private void SetupWheelsInitialRotation()
	{
		if (steeringWheel != null)
		{
			initialSteeringWheelRotation = steeringWheel.transform.localRotation;
		}
		if (wheels != null && wheels.Length > 0)
		{
			initialWheelRotations = new Quaternion[wheels.Length];
			for (int i = 0; i < wheels.Length; i++)
			{
				initialWheelRotations[i] = wheels[i].transform.localRotation;
			}
		}
	}

	private void SpawnSkeleton()
	{
		if (vehicleSkeletonPrefab == null || vehicleSkeletonRoot == null)
		{
			UnityEngine.Debug.LogError("Missing vehicle skeleton references!");
		}
		else
		{
			skeleton = UnityEngine.Object.Instantiate(vehicleSkeletonPrefab, vehicleSkeletonRoot, worldPositionStays: false);
		}
	}

	[ContextMenu("SetupRigidbodyFromConfig")]
	private void SetupRigidbodyFromConfig()
	{
		if (!(body == null) && !(config == null))
		{
			body.mass = config.mass;
			body.angularDrag = config.angularDrag;
			body.drag = config.drag;
			body.centerOfMass = config.centerOfMass;
		}
	}

	protected void EnableCarInterior(bool enable)
	{
		if (carInterior != null)
		{
			carInterior.SetActive(enable);
		}
		if (mainMesh != null)
		{
			mainMesh.SetActive(!enable);
		}
	}

	protected virtual IEnumerator ActivateAfter(float s)
	{
		yield return new WaitForSeconds(s);
		CarMob carMob = GetComponent<CarMob>();
		if (carMob != null && spawnedByBlock < 0)
		{
			carMob.HijackCar();
		}
	}

	private void SetCamera()
	{
		SetParent(insideCameraPosition.transform);
		vehicleCamera = CameraController.instance.MainCamera;
		SetVehicleFov();
		EnableCarInterior(enable: true);
	}

	protected virtual void MoveCarWheel(float value)
	{
		float num = GetForwardVelocity() * PIx2;
		traveledDistance += num;
		Quaternion rhs = Quaternion.Euler(rotationAxes.wheelForward * traveledDistance);
		for (int i = 0; i < wheels.Length; i++)
		{
			if (wheels[i] != null)
			{
				wheels[i].transform.localRotation = initialWheelRotations[i] * rhs;
				if (IsWheelRotatedBySteering(i))
				{
					wheels[i].transform.Rotate(rotationAxes.wheelTurning * (value * config.frontWheelsMaxAngle), Space.World);
				}
			}
		}
		if (steeringWheel != null)
		{
			steeringWheel.transform.localRotation = initialSteeringWheelRotation * Quaternion.Euler(rotationAxes.steering * (value * config.steeringWheelMaxAngle));
		}
	}

	private bool IsWheelRotatedBySteering(int index)
	{
		int[] array = rotatableWheelIndex;
		foreach (int num in array)
		{
			if (index == num)
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void AddHoverForces()
	{
		float num = config.hoverHeight;
		if (!isInUse)
		{
			num = 0f;
		}
		bool flag = false;
		if (skeleton.hoverPoints != null)
		{
			for (int i = 0; i < skeleton.hoverPoints.Length; i++)
			{
				GameObject gameObject = skeleton.hoverPoints[i];
				if (Physics.Raycast(gameObject.transform.position, -Vector3.up, out RaycastHit hitInfo, num, layerMask))
				{
					body.AddForceAtPosition(Vector3.up * config.hoverForce * (num - hitInfo.distance / num), gameObject.transform.position);
					flag = true;
					continue;
				}
				Vector3 position = base.transform.position;
				float y = position.y;
				Vector3 position2 = gameObject.transform.position;
				if (y > position2.y)
				{
					body.AddForceAtPosition(gameObject.transform.up * config.gravityForce, gameObject.transform.position);
				}
				else
				{
					body.AddForceAtPosition(gameObject.transform.up * (0f - config.gravityForce), gameObject.transform.position);
				}
			}
		}
		if (flag)
		{
			body.drag = config.groundedDrag;
			return;
		}
		body.drag = 0.1f;
		thrust /= 100f;
		turnValue /= 100f;
	}

	protected float GetForwardVelocity()
	{
		Vector3 vector = base.transform.InverseTransformDirection(body.velocity);
		return vector.z;
	}

	protected virtual void AddForces()
	{
		if (analogInput != null)
		{
			Vector3 angularVelocity = body.angularVelocity;
			angularVelocity.y = config.turnStrength * turnValue;
			body.angularVelocity = angularVelocity;
		}
		if (Mathf.Abs(thrust) > 0f)
		{
			body.AddForce(base.transform.forward * thrust);
		}
		if (body.velocity.sqrMagnitude > (body.velocity.normalized * config.maxVelocity).sqrMagnitude)
		{
			body.velocity = body.velocity.normalized * config.maxVelocity;
		}
		body.drag = config.groundedDrag;
		body.velocity *= config.dumpValueByVelocityAngle.Evaluate(Vector3.Angle(base.transform.forward, body.velocity.normalized));
	}

	private void CheckPlayerYPosition()
	{
		Vector3 position = base.transform.position;
		if (position.y < -16f)
		{
			Transform transform = base.transform;
			Vector3 position2 = base.transform.position;
			float x = position2.x;
			Vector3 position3 = base.transform.position;
			transform.position = new Vector3(x, 61f, position3.z);
		}
	}

	protected void Realign()
	{
		Vector3 velocity = body.velocity;
		velocity.x *= friction;
		velocity.z *= friction;
		body.velocity = velocity;
		body.angularVelocity *= friction;
	}

	private Vector3 FindBestExitPlace()
	{
		Vector3 halfExtents = new Vector3(1f, 1.8f, 1f) / 2f;
		Vector3 position = default(Vector3);
		foreach (Vector3 exitOffset in exitOffsets)
		{
			position = exitOffset;
			Vector3 vector = base.transform.TransformPoint(position);
			if (!Physics.CheckBox(vector, halfExtents))
			{
				return player.transform.InverseTransformPoint(vector);
			}
		}
		if (mainMesh != null)
		{
			Vector3 size = mainMesh.GetComponent<MeshRenderer>().bounds.size;
			position = new Vector3(0f - (size.x / 2f + 1f), 1f, 0f);
			Vector3 vector = base.transform.TransformPoint(position);
			if (!Physics.CheckBox(vector, halfExtents))
			{
				return player.transform.InverseTransformPoint(vector);
			}
			position.x *= -1f;
			vector = base.transform.TransformPoint(position);
			if (!Physics.CheckBox(vector, halfExtents))
			{
				return player.transform.InverseTransformPoint(vector);
			}
		}
		return new Vector3(-3f, 4f);
	}
}
