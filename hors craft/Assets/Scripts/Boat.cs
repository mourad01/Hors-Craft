// DecompilerFi decompiler from Assembly-CSharp.dll class: Boat
using com.ootii.Cameras;
using Gameplay;
using Uniblocks;
using UnityEngine;

public class Boat : VehicleController
{
	[Header("Vehicle Config")]
	[BoatVehicleConfigsDropdown]
	public BoatVehicleConfig config;

	[HoverVehicleSkeletonsDropdown]
	public HoverVehicleSkeleton vehicleSkeletonPrefab;

	[Header("References")]
	public BoatData boatData;

	public Rigidbody body;

	public Transform vehicleSkeletonRoot;

	public GameObject insideCameraPosition;

	[Header("References gfx")]
	public GameObject steeringWheel;

	public GameObject carInterior;

	public GameObject mainMesh;

	private float waterLevel;

	private RigidbodyConstraints rigidbodyConstraints;

	private float turnValue;

	private float thrust;

	private float deadZone = 0.1f;

	private HoverVehicleSkeleton skeleton;

	public UniversalAnalogInput analogInput;

	[Header("Stering wheel and arms")]
	[SerializeField]
	protected float steringWheelMaxAngle = 30f;

	[SerializeField]
	protected Vector3 steeringWheelRotationAxis = Vector3.forward;

	protected Quaternion initialSteeringWheelRotation;

	[SerializeField]
	protected GameObject leftHand;

	[SerializeField]
	protected GameObject rightHand;

	private Vector3 leftHandRotation;

	private Vector3 rightHandRotation;

	public float MaxVelocity => config.maxVelocity;

	private void Awake()
	{
		if (skeleton == null)
		{
			SpawnSkeleton();
		}
		EnableCarInterior(enable: false);
		EnableHands(enable: false);
		body.useGravity = false;
	}

	private void Start()
	{
		SetupRigidbodyFromConfig();
		rigidbodyConstraints = body.constraints;
		body.centerOfMass = Vector3.down;
		InitLayerMask();
		VoxelInfo voxelInfo = Engine.VoxelGridRaycastFor(base.transform.position + Vector3.up * 10f, Vector3.down, 100f, Engine.usefulIDs.waterBlockID);
		if (voxelInfo != null)
		{
			Vector3 vector = Engine.VoxelInfoToPosition(voxelInfo);
			waterLevel = vector.y + 1f;
		}
		else
		{
			waterLevel = Engine.TerrainGenerator.waterHeight;
		}
		body.drag = config.groundedDrag;
		if (steeringWheel != null)
		{
			initialSteeringWheelRotation = steeringWheel.transform.localRotation;
		}
		if (leftHand != null && rightHand != null)
		{
			leftHandRotation = leftHand.transform.localEulerAngles;
			rightHandRotation = rightHand.transform.localEulerAngles;
		}
	}

	private void Update()
	{
		if (analogInput != null)
		{
			thrust = 0f;
			Vector2 vector = analogInput.CalculatePosition();
			Vector2 vector2 = analogInput.CalculatePosition();
			MoveHands(vector2.x);
			float y = vector.y;
			if (y > deadZone)
			{
				thrust = y * config.forwardAcceleration;
			}
			else if (y < 0f - deadZone)
			{
				thrust = y * config.reverseAcceleration;
			}
			turnValue = 0f;
			float x = vector.x;
			turnValue = x * Mathf.Sign(GetForwardVelocity());
			MoveCarWheel(vector.x);
			CheckPlayerYPosition();
			InformOnMovement();
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
			{
				player.transform.eulerAngles = Vector3.zero;
			}
		}
	}

	protected virtual void FixedUpdate()
	{
		AddHoverForces();
		AddForces();
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
		base.player = player;
		body.constraints = rigidbodyConstraints;
		isInUse = true;
		base.enabled = true;
		body.useGravity = false;
		body.isKinematic = false;
		EnablePlayerScripts(player, enable: false);
		EnableHands(enable: true);
		analogInput = CameraEventsSender.MainInput;
		SetCamera();
		PlayerFishingController component = base.player.GetComponent<PlayerFishingController>();
		if (component != null)
		{
			component.isInBoat = true;
			component.currentBoat = this;
		}
		skeleton.colliderRide.SetActive(value: true);
		colliderStatic.SetActive(value: false);
		CarMob component2 = GetComponent<CarMob>();
		if (component2 != null && spawnedByBlock < 0)
		{
			component2.HijackCar();
		}
		DestroyElement<CarMob>();
		DestroyElement<CarNavigator>();
		if (renderPlayer)
		{
			PlayerGraphic.GetControlledPlayerInstance().mainBody.gameObject.SetActive(value: true);
		}
	}

	public override void StopUsing()
	{
		base.StopUsing();
		body.constraints = (rigidbodyConstraints | RigidbodyConstraints.FreezePositionY);
		isInUse = false;
		base.enabled = false;
		body.useGravity = false;
		EnableHands(enable: false);
		EnableCarInterior(enable: false);
		skeleton.colliderRide.SetActive(value: false);
		colliderStatic.SetActive(value: true);
		if (!(player == null))
		{
			player.transform.localPosition = new Vector3(0f, 1.1f);
			player.transform.SetParent(null);
			EnablePlayerScripts(player, enable: true);
			RestoreVehicleFov();
			PlayerFishingController component = player.GetComponent<PlayerFishingController>();
			if (component != null)
			{
				component.isInBoat = false;
			}
			player.transform.localScale = Vector3.one;
			player.transform.localRotation = Quaternion.identity;
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
			skeleton = Object.Instantiate(vehicleSkeletonPrefab, vehicleSkeletonRoot, worldPositionStays: false);
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
		skeleton.colliderRide.SetActive(enable);
		colliderStatic.SetActive(!enable);
	}

	private void SetCamera()
	{
		SetParent(insideCameraPosition.transform);
		vehicleCamera = CameraController.instance.MainCamera;
		SetVehicleFov();
		EnableCarInterior(enable: true);
	}

	private void MoveCarWheel(float value)
	{
		if (steeringWheel != null)
		{
			steeringWheel.transform.localRotation = initialSteeringWheelRotation * Quaternion.Euler(steeringWheelRotationAxis * (value * steringWheelMaxAngle));
		}
	}

	private void AddHoverForces()
	{
		for (int i = 0; i < skeleton.hoverPoints.Length; i++)
		{
			GameObject gameObject = skeleton.hoverPoints[i];
			Vector3 position = gameObject.transform.position;
			if (position.y > waterLevel)
			{
				body.AddForceAtPosition(gameObject.transform.up * (0f - config.gravityForce), gameObject.transform.position);
			}
			else
			{
				body.AddForceAtPosition(gameObject.transform.up * config.gravityForce, gameObject.transform.position);
			}
		}
	}

	private float GetForwardVelocity()
	{
		Vector3 vector = base.transform.InverseTransformDirection(body.velocity);
		return vector.z;
	}

	private void AddForces()
	{
		body.AddRelativeTorque(Vector3.up * turnValue * config.turnStrength);
		if (Mathf.Abs(thrust) > 0f)
		{
			body.AddForce(base.transform.forward * thrust);
		}
		if (body.velocity.sqrMagnitude > (body.velocity.normalized * config.maxVelocity).sqrMagnitude)
		{
			body.velocity = body.velocity.normalized * config.maxVelocity;
		}
		body.velocity *= config.dumpValue;
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

	private void EnableHands(bool enable)
	{
		if (!(leftHand == null) && !(rightHand == null))
		{
			leftHand.SetActive(enable);
			rightHand.SetActive(enable);
		}
	}

	private void MoveHands(float value)
	{
		if (!(leftHand == null) && !(rightHand == null))
		{
			float angle = Quaternion.Angle(initialSteeringWheelRotation, steeringWheel.transform.localRotation);
			leftHand.transform.localEulerAngles = new Vector3(leftHandRotation.x, leftHandRotation.y, leftHandRotation.z);
			leftHand.transform.Rotate(Vector3.up, angle, Space.World);
			rightHand.transform.localEulerAngles = new Vector3(rightHandRotation.x, rightHandRotation.y, rightHandRotation.z);
			rightHand.transform.Rotate(Vector3.up, angle, Space.World);
		}
	}
}
