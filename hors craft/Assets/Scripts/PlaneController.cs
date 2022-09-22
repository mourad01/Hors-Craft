// DecompilerFi decompiler from Assembly-CSharp.dll class: PlaneController
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay;
using Gameplay.Audio;
using States;
using System.Collections;
using Uniblocks;
using UnityEngine;

public class PlaneController : VehicleController, ICollisionReceiver, IFlyingVehicle
{
	[Header("Vehicle Config")]
	[PlaneVehicleConfigsDropdown]
	public PlaneVehicleConfig config;

	[HoverVehicleSkeletonsDropdown]
	public HoverVehicleSkeleton vehicleSkeletonPrefab;

	[Header("References")]
	public Transform vehicleSkeletonRoot;

	public GameObject cameraPositionToUse;

	public GameObject planeObject;

	public GameObject physicCenter;

	public GameObject[] gameobjectsToDisableWhileStopped;

	private Rigidbody rigidBody;

	private Animator animator;

	private float maxAngle = 30f;

	private bool isOnGround;

	private bool forwardObstalce;

	private bool upObstacle;

	private bool wasMovedBackward;

	private float thrust;

	private HoverVehicleSkeleton skeleton;

	public bool isThrustHold
	{
		get;
		set;
	}

	public UniversalAnalogInput analogInput => CameraEventsSender.MainInput;

	public bool isGrounded
	{
		get
		{
			UnityEngine.Debug.DrawRay(base.transform.position, -base.transform.up * config.groundDistanceCheck, Color.yellow);
			RaycastHit hitInfo;
			return Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo, config.groundDistanceCheck);
		}
	}

	public bool isObstalceUp
	{
		get
		{
			UnityEngine.Debug.DrawRay(base.transform.position, base.transform.up * config.obstacleUpCheck, Color.yellow);
			RaycastHit hitInfo;
			return Physics.Raycast(base.transform.position, base.transform.up, out hitInfo, config.obstacleUpCheck);
		}
	}

	public bool isObstaleForward
	{
		get
		{
			UnityEngine.Debug.DrawRay(base.transform.position, base.transform.forward * config.obstacleForwardCheck, Color.yellow);
			RaycastHit hitInfo;
			return Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, config.obstacleForwardCheck, -1, QueryTriggerInteraction.Ignore);
		}
	}

	public bool allRayChecks
	{
		get
		{
			if (config.ignoreRaycastChecks)
			{
				return false;
			}
			return isOnGround || forwardObstalce || upObstacle;
		}
	}

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
		SpawnSkeleton();
		SetupRigidbodyFromConfig();
		TestInvertMethod();
		GameObject[] array = gameobjectsToDisableWhileStopped;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(value: false);
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
		if (isInUse)
		{
			isOnGround = isGrounded;
			forwardObstalce = isObstaleForward;
			upObstacle = isObstalceUp;
			ForwardObstalceReact();
			AnalogInputUpdateWithoutZRotation();
			if (analogInput.IsInDeadZone(0.15f) && config.analogStick)
			{
				IfNoInputReturn();
			}
			AddGraivty();
			InformOnMovement();
		}
	}

	public override void VehicleActivate(GameObject player)
	{
		base.VehicleActivate(player);
		rigidBody.freezeRotation = false;
		rigidBody.useGravity = false;
		rigidBody.drag = 10f;
		rigidBody.centerOfMass = physicCenter.transform.localPosition;
		EnablePlayerScripts(player, enable: false);
		SetCamera();
		thrust = 0f;
		isInUse = true;
		CameraController.instance.MainCamera.GetComponent<Animator>().enabled = false;
		animator = planeObject.GetComponentInChildren<Animator>();
		if (animator != null)
		{
			animator.SetTrigger("Start");
		}
		GameObject[] array = gameobjectsToDisableWhileStopped;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(value: true);
		}
		StartCoroutine(ActivateAfter(1f));
	}

	public override void StopUsing()
	{
		base.StopUsing();
		GameObject[] array = gameobjectsToDisableWhileStopped;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(value: false);
		}
		if (!(player == null))
		{
			player.transform.localPosition = new Vector3(-3f, 4f);
			player.transform.SetParent(null);
			EnablePlayerScripts(player, enable: true);
			player.transform.localScale = Vector3.one;
			rigidBody.drag = 0.1f;
			isInUse = false;
			player.transform.localRotation = Quaternion.identity;
			player.SetActive(value: true);
			rigidBody.useGravity = true;
			thrust = 0f;
			CameraController.instance.MainCamera.GetComponent<Animator>().enabled = true;
			RestoreVehicleFov();
			if (animator != null)
			{
				animator.SetTrigger("Stop");
			}
			StopAllCoroutines();
		}
	}

	protected override void UpdatePlayerQuests(float distance)
	{
		Singleton<PlayerData>.get.playerQuests.InformOnPlayerWalk(base.transform.position, distance, PlayerMovement.Mode.FLYING, inWater: false);
	}

	public void OnCollision(Transform collider)
	{
		TogglePlaneSound(newState: false);
		OnHitEffects(Vector3.up);
		ShowPopupOnCollision();
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
		if (!(rigidBody == null) && !(config == null))
		{
			rigidBody.mass = config.mass;
			rigidBody.angularDrag = config.angularDrag;
			rigidBody.drag = config.drag;
		}
	}

	private IEnumerator ActivateAfter(float s)
	{
		yield return new WaitForSeconds(s);
		CarMob carMob = GetComponent<CarMob>();
		if (carMob != null && spawnedByBlock < 0)
		{
			carMob.HijackCar();
		}
		EnablePlayerScripts(player, enable: false);
	}

	private void SetCamera()
	{
		SetParent(cameraPositionToUse.transform);
		vehicleCamera = CameraController.instance.MainCamera;
		SetVehicleFov();
	}

	private void ChangeHeightOfHoverPoint(float lerp)
	{
		skeleton.hoverPoints[2].transform.localPosition = skeleton.hoverPoints[2].transform.localPosition.WithY(Mathf.Lerp(0.39f, 1f, lerp));
		skeleton.hoverPoints[3].transform.localPosition = skeleton.hoverPoints[3].transform.localPosition.WithY(Mathf.Lerp(0.39f, 1f, lerp));
	}

	public void UpdateCameraStuff()
	{
		UpdateFov(rigidBody.velocity.sqrMagnitude / config.maximumVelocity);
	}

	private bool ForwardObstalceReact()
	{
		if (isGrounded && forwardObstalce)
		{
			rigidBody.AddRelativeForce(0f, 0f, 0f - config.goBackPower);
			return true;
		}
		return false;
	}

	public void AnalogInputUpdateWithoutZRotation()
	{
		float num = (!config.invertControls) ? 1f : (-1f);
		Vector2 vector = analogInput.CalculatePosition();
		float num2 = num * vector.y * (Time.deltaTime * config.pitchSpeed);
		Vector2 vector2 = analogInput.CalculatePosition();
		float yaw = vector2.x * (Time.deltaTime * config.yawSpeed);
		Vector3 velocity = rigidBody.velocity;
		float lift = Mathf.Abs(velocity.z * config.liftFactor);
		if (isOnGround && num2 > 0f)
		{
			num2 = 0f;
		}
		thrust += (float)(isThrustHold ? 1 : 0) * (Time.deltaTime * config.accelarate);
		if (thrust > config.maximalThrust)
		{
			thrust = config.maximalThrust;
		}
		if (thrust > 0.01f || wasMovedBackward)
		{
			AddForces(num2, yaw, lift);
		}
		thrust *= config.dumpValue;
		if (forwardObstalce)
		{
			thrust *= 0.8f;
		}
		if (!analogInput.IsInDeadZone(0.15f) && !isOnGround)
		{
			Vector2 vector3 = analogInput.CalculatePosition();
			FakePlaneRotation(vector3.x);
		}
		Vector2 vector4 = analogInput.CalculatePosition();
		ChangeHeightOfHoverPoint(0f - vector4.y);
	}

	private void AddGraivty()
	{
		if (allRayChecks)
		{
			AddHoverForces();
		}
		else
		{
			rigidBody.AddForce(new Vector3(0f, 0f - config.gravityPower, 0f), ForceMode.Acceleration);
		}
	}

	public void AddForces(float pitch, float yaw, float lift)
	{
		rigidBody.AddRelativeTorque(new Vector3(pitch, yaw, 0f));
		rigidBody.AddForce(base.transform.forward * thrust + Vector3.up * lift);
		UnityEngine.Debug.DrawRay(base.transform.position, base.transform.forward * thrust, Color.blue, Time.fixedDeltaTime);
		UnityEngine.Debug.DrawRay(base.transform.position, new Vector3(0f, lift, 0f), Color.white, Time.fixedDeltaTime);
	}

	private float BetterSign(float value)
	{
		if (Mathf.Abs(value) < config.epsilon)
		{
			return 0f;
		}
		return Mathf.Lerp(0.1f, 1f, Mathf.Abs(config.currentAngleValue / maxAngle)) * Mathf.Sign(value);
	}

	private void FakePlaneRotation(float rollPower)
	{
		rollPower /= config.angleDumpValue;
		float num = 0f;
		num = ((!(config.currentAngleValue / maxAngle + rollPower > 0f)) ? (0f - maxAngle) : maxAngle);
		float num2 = SmoothFunctionLn(Mathf.Abs(config.currentAngleValue / maxAngle + rollPower)) * num;
		config.currentAngleValue = InverseSmoothFunctionLn(num2 / num) * num;
		planeObject.transform.localRotation = Quaternion.Euler(planeObject.transform.localRotation.eulerAngles.WithZ(0f - num2));
	}

	private void TestInvertMethod()
	{
		float num = 0f;
		for (float num2 = 0f; num2 < 1f; num2 += 0.01f)
		{
			num += InverseSmoothFunction(SmoothFunction(num2)) - num2;
		}
	}

	private float SmoothFunction(float value)
	{
		value = Mathf.Clamp01(value);
		return (3f - 2f * value) * value * value;
	}

	private float InverseSmoothFunction(float value)
	{
		value = Mathf.Clamp01(value);
		return value * (2f * value * value - 3f * value + 2f);
	}

	private float SmoothFunctionLn(float value)
	{
		value = Mathf.Clamp01(value);
		return Mathf.Log(value + 0.02f) * 0.25f + 1f;
	}

	private float InverseSmoothFunctionLn(float value)
	{
		value = Mathf.Clamp01(value);
		return (Mathf.Exp(4f * value - 4f) * 50f - 1f) / 50f;
	}

	private void IfNoInputReturn()
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		float num = eulerAngles.z;
		float z = 0f;
		if (num > 0f && num < 180f)
		{
			z = num - config.returnToZeroAngle;
		}
		if (num < 360f && num > 180f)
		{
			z = num + config.returnToZeroAngle;
			num = 360f - num;
		}
		if (num > config.returnToZeroAngle * 1.5f)
		{
			base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.WithZ(z));
		}
		if (!isOnGround)
		{
			float num2 = 0f - BetterSign(config.currentAngleValue);
			if (num2 == 0f)
			{
				config.currentAngleValue = 0f;
			}
			FakePlaneRotation(num2);
		}
	}

	private void KeysInputUpdate()
	{
		float num = UnityEngine.Input.GetAxis("Roll") * (Time.deltaTime * config.rollSpeed);
		float num2 = UnityEngine.Input.GetAxis("Pitch") * (Time.deltaTime * config.pitchSpeed);
		float y = UnityEngine.Input.GetAxis("Yaw") * (Time.deltaTime * config.yawSpeed);
		Vector3 velocity = rigidBody.velocity;
		float y2 = Mathf.Abs(velocity.z * config.liftFactor);
		rigidBody.AddRelativeTorque(0f - num2, y, 0f - num);
		thrust += (float)(isThrustHold ? 1 : 0) * (Time.deltaTime * config.accelarate);
		if (thrust > 0f)
		{
			rigidBody.AddRelativeForce(0f, y2, thrust);
		}
		rigidBody.AddForce(new Vector3(0f, 0f - config.gravityPower, 0f), ForceMode.Acceleration);
	}

	private void AddHoverForces()
	{
		for (int i = 0; i < skeleton.hoverPoints.Length; i++)
		{
			GameObject gameObject = skeleton.hoverPoints[i];
			if (Physics.Raycast(gameObject.transform.position, -Vector3.up, out RaycastHit hitInfo, config.hoverHeight))
			{
				rigidBody.AddForceAtPosition(Vector3.up * config.hoverForce * (1f - hitInfo.distance / config.hoverHeight), gameObject.transform.position);
				continue;
			}
			Vector3 position = base.transform.position;
			float y = position.y;
			Vector3 position2 = gameObject.transform.position;
			if (y > position2.y)
			{
				rigidBody.AddForceAtPosition(gameObject.transform.up * config.gravityForce, gameObject.transform.position);
			}
			else
			{
				rigidBody.AddForceAtPosition(gameObject.transform.up * (0f - config.gravityForce), gameObject.transform.position);
			}
		}
	}

	private void OnHitEffects(Vector3 dir)
	{
		if (isInUse)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.GOT_HIT, new GotHitContext
			{
				angle = Mathf.Atan2(0f - dir.z, 0f - dir.x) * 57.29578f
			});
			HitShakeEffect componentInChildren = GetComponentInChildren<HitShakeEffect>();
			if (componentInChildren != null)
			{
				componentInChildren.StartShake();
			}
			PlaySound(GameSound.ZOMBIE_HIT);
		}
	}

	private void PlaySound(GameSound gameSound)
	{
		Sound sound = new Sound();
		sound.clip = Manager.Get<ClipsManager>().GetClipFor(gameSound);
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		Sound sound2 = sound;
		sound2.Play();
	}

	private void TogglePlaneSound(bool newState)
	{
		AudioSource component = GetComponent<AudioSource>();
		if ((bool)component)
		{
			component.mute = !newState;
		}
	}

	private void ShowPopupOnCollision()
	{
		Health playerHealth = null;
		PlayerController component = player.GetComponent<PlayerController>();
		if (!(component == null) && component.showAdPopupOnDeath && !(Manager.Get<StateMachineManager>().currentState as AdForRespawnState))
		{
			Manager.Get<StateMachineManager>().PushState<AdForRespawnState>(new AdForRespawnStartParameter("gameover.watchads", "menu.watch", "resetworld.return", new PopupTextConfig("YOUR SCORE:", "gameover.score"), delegate
			{
				if (Application.isEditor)
				{
					Respawn();
					Manager.Get<StateMachineManager>().PopState();
				}
				else
				{
					UnityEngine.Debug.Log("Trying to show ad");
					TryToShowAd();
				}
			}, delegate
			{
				UnityEngine.Debug.Log("Refused!");
				playerHealth = player.gameObject.GetComponent<Health>();
				playerHealth.invulnerability = false;
				playerHealth.OnHit(100f, Vector3.up);
			}));
		}
	}

	private void TryToShowAd()
	{
		Manager.Get<RewardedAdsManager>().ShowRewardedAd(StatsManager.AdReason.XCRAFT_RESPAWN, delegate(bool success)
		{
			if (success)
			{
				Respawn();
				Manager.Get<StateMachineManager>().PopState();
			}
		});
	}

	private void Respawn()
	{
		TogglePlaneSound(newState: true);
		base.transform.position = base.transform.position + Vector3.up * 20f;
		base.transform.eulerAngles = new Vector3(0f, 0f, 0f);
		Fixable component = GetComponent<Fixable>();
		if (component != null)
		{
			component.Fix(toMaxHp: true);
		}
	}
}
