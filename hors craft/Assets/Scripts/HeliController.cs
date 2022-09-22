// DecompilerFi decompiler from Assembly-CSharp.dll class: HeliController
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using States;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class HeliController : VehicleController, IFlyingVehicle
{
	[Header("Vehicle Config")]
	[HeliVehicleConfigsDropdown]
	public HeliVehicleConfig config;

	[Header("References")]
	public GameObject cameraPositionToUse;

	public GameObject heliObject;

	public Rigidbody rigidBody;

	public GameObject physicCenter;

	public Rotor[] rotors;

	private AudioSource audioSource;

	private float thrust;

	private float currentFovStep;

	private UniversalAnalogInput analogInput;

	public bool isAscendHold
	{
		get;
		set;
	}

	public bool isDescendHold
	{
		get;
		set;
	}

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
		SetupRigidbodyFromConfig();
		isInUse = false;
		audioSource = GetComponent<AudioSource>();
		if (!audioSource)
		{
			UnityEngine.Debug.Log("AudioSource component not present");
		}
	}

	private void Update()
	{
		if (isInUse)
		{
			UpdateCameraStuff();
		}
	}

	private void FixedUpdate()
	{
		if (!isInUse)
		{
			return;
		}
		if (analogInput == null)
		{
			analogInput = CameraEventsSender.MainInput;
			return;
		}
		AnalogInputUpdateWithoutZRotation();
		FakeHeliRotation(analogInput.CalculatePosition());
		Stabilize();
		if (!isInUse)
		{
			AddGravity();
		}
		InformOnMovement();
	}

	public override void VehicleActivate(GameObject p)
	{
		player = p;
		base.VehicleActivate(player);
		rigidBody.freezeRotation = false;
		rigidBody.useGravity = false;
		rigidBody.drag = 10f;
		rigidBody.centerOfMass = physicCenter.transform.localPosition;
		EnablePlayerScripts(player, enable: false);
		analogInput = CameraEventsSender.MainInput;
		SetCamera();
		CameraController.instance.transform.forward = player.transform.forward;
		thrust = 0f;
		isInUse = true;
		StartRotors();
		audioSource.Play();
		CarMob component = GetComponent<CarMob>();
		if (component != null && spawnedByBlock < 0)
		{
			component.HijackCar();
		}
	}

	public override void StopUsing()
	{
		base.StopUsing();
		isInUse = false;
		rigidBody.useGravity = true;
		if (!(player == null))
		{
			rigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
			player.transform.localPosition = new Vector3(-3f, 4f);
			player.transform.SetParent(null);
			EnablePlayerScripts(player, enable: true);
			player.transform.localScale = Vector3.one;
			player.transform.localRotation = Quaternion.identity;
			thrust = 0f;
			RestoreVehicleFov();
			StopRotors();
			audioSource.Stop();
			StopAllCoroutines();
			PlayerGraphic.GetControlledPlayerInstance().ShowPlayerGraphic();
			player.GetComponent<PlayerMovement>().UpdateCurrentMode();
			heliObject.transform.rotation = Quaternion.identity;
			rigidBody.drag = 0.1f;
			player.SetActive(value: true);
			CameraController.instance.MainCamera.GetComponent<Animator>().enabled = true;
		}
	}

	protected override void UpdateFov(float currentSpeed)
	{
		if (!(vehicleCamera == null))
		{
			if (currentSpeed > currentFovStep && (double)Mathf.Abs(currentSpeed - currentFovStep) > 0.05)
			{
				currentFovStep += config.fovChangeRate * Time.deltaTime;
			}
			else if (currentSpeed < currentFovStep && (double)Mathf.Abs(currentSpeed - currentFovStep) > 0.05)
			{
				currentFovStep -= config.fovChangeRate * Time.deltaTime;
			}
			currentFovStep = Mathf.Clamp01(currentFovStep);
			vehicleCamera.fieldOfView = fovChangeType.Evaluate(currentFovStep);
		}
	}

	private void SetupRigidbodyFromConfig()
	{
		if (!(rigidBody == null) && !(config == null))
		{
			rigidBody.mass = config.mass;
			rigidBody.angularDrag = config.angularDrag;
			rigidBody.drag = config.drag;
		}
	}

	private void StartRotors()
	{
		Rotor[] array = rotors;
		foreach (Rotor rotor in array)
		{
			rotor.StartRotating();
		}
	}

	private void StopRotors()
	{
		Rotor[] array = rotors;
		foreach (Rotor rotor in array)
		{
			rotor.StopRotating();
		}
	}

	private void SetCamera()
	{
		SetParent(cameraPositionToUse.transform);
		vehicleCamera = CameraController.instance.MainCamera;
		SetVehicleFov();
	}

	public void UpdateCameraStuff()
	{
		Vector2 vector = analogInput.CalculatePosition();
		UpdateFov(vector.y);
	}

	public void AnalogInputUpdateWithoutZRotation()
	{
		thrust += (float)(isAscendHold ? 1 : 0) * (Time.deltaTime * config.accelarate);
		thrust -= (float)(isDescendHold ? 1 : 0) * (Time.deltaTime * config.accelarate);
		Vector2 vector = analogInput.CalculatePosition();
		float y = vector.y;
		Vector2 vector2 = analogInput.CalculatePosition();
		AddForces(y, vector2.x, 0f);
		thrust *= config.dumpValue;
	}

	private void AddGravity()
	{
		rigidBody.AddForce(new Vector3(0f, 0f - config.gravityPower, 0f), ForceMode.Acceleration);
	}

	public void AddForces(float pitch, float yaw, float roll)
	{
		if (pitch < 0f)
		{
			pitch /= 2f;
		}
		rigidBody.AddForce(Vector3.up * thrust);
		rigidBody.AddRelativeForce(Vector3.forward * pitch * config.forwardSpeed * Time.deltaTime);
		rigidBody.AddTorque(Vector3.up * yaw * config.turnSpeed * Time.deltaTime);
		UnityEngine.Debug.DrawRay(base.transform.position, base.transform.forward * thrust, Color.blue, Time.fixedDeltaTime);
	}

	private void FakeHeliRotation(Vector2 analog)
	{
		float x = analog.x;
		float y = analog.y;
		GameplayState gameplayState = Manager.Get<StateMachineManager>().currentState as GameplayState;
		if (gameplayState != null)
		{
			CameraRotationModule cameraRotationModule = gameplayState.GetModules().FirstOrDefault((GameplayModule m) => m is CameraRotationModule) as CameraRotationModule;
			if (cameraRotationModule != null)
			{
				PlayerInputInfo.inputArea = cameraRotationModule.rotateButton;
			}
		}
		LayerMask mask = ~(1 << LayerMask.NameToLayer("UI"));
		Ray ray = CameraController.instance.MainCamera.ScreenPointToRay(PlayerInputInfo.inputInfo.position);
		if (PlayerInputInfo.inputArea.pressed && Physics.Raycast(ray, out RaycastHit hitInfo, 10000f, mask, QueryTriggerInteraction.Ignore) && this != hitInfo.collider.gameObject.GetComponentInParent<HeliController>() && !analogInput.IsTouching())
		{
			Vector3 forward = hitInfo.point - heliObject.transform.position;
			Quaternion b = Quaternion.LookRotation(forward);
			heliObject.transform.rotation = Quaternion.Lerp(heliObject.transform.rotation, b, config.modelRotationSpeed);
		}
		else
		{
			Quaternion b = Quaternion.Euler(y * config.maxPitchAngle, x * config.maxRollAngle, 0f);
			heliObject.transform.localRotation = Quaternion.Lerp(heliObject.transform.localRotation, b, config.modelRotationSpeed);
		}
	}

	private void Stabilize()
	{
		Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
		Quaternion b = Quaternion.Euler(0f, eulerAngles.y, 0f);
		base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, b, config.modelRotationSpeed);
	}
}
