// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.PlayerMovement
using com.ootii.Cameras;
using Common.Audio;
using Common.Behaviours;
using Common.ImageEffects;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay;
using Gameplay.Audio;
using LimitWorld;
using States;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Uniblocks
{
	public class PlayerMovement : MonoBehaviour, IGameCallbacksListener, IFactChangedListener
	{
		public enum Mode
		{
			WALKING = 0,
			MOUNTED = 3,
			FLYING = 1,
			MOUNTED_FLYING = 4,
			MOUNTED_VEHICLE = 5
		}

		protected Mode mode;

		public const float HEIGHT_TO_TELEPORT_IF_FALLING = 61f;

		public const float MAX_DEPTH = -16f;

		public const float PLAYER_UNDERWATER_HEIGHT = 1.49f;

		public PlayerStats stat;

		public float walkingMaxForwardSpeed = 3f;

		public float walkingMaxBackwardsSpeed = 3f;

		public float walkingMaxSidewaysSpeed = 3f;

		private float speedModifier = 1f;

		private float flySpeedModifier = 1f;

		private float jumpModifier = 1f;

		private UnderwaterDistortionEffect _underwaterEffect;

		private bool stopMoving;

		public CharacterController controller;

		private CharacterMotor motor;

		private Animator cameraAnimator;

		private AudioSource MovementSource;

		private Transform cameraTransform;

		protected Animator playerAnimator;

		private Vector3 lastPosition;

		private float positionYGrounded;

		private float baseControllerHeight = 1.1f;

		[HideInInspector]
		public float playerInputX;

		[HideInInspector]
		public float playerInputY;

		[HideInInspector]
		public bool fallPressed;

		public SimpleVelocity hVelocityX = new SimpleVelocity(2f, 50f, 0.01f);

		public SimpleVelocity hVelocityZ = new SimpleVelocity(2f, 50f, 0.01f);

		public SimpleVelocity hVelocityY = new SimpleVelocity(1f, 10f, 0.1f);

		public Vector3 cameraOffset = new Vector3(0f, 0.25f, 0f);

		public float mountCameraTranslationLerpValue = 1f;

		public bool allowCameraBob = true;

		private bool wasInWater;

		private GameSound previousSound;

		private Material _skyboxShader;

		[Header("Movement Stats config")]
		[SerializeField]
		private int[] movementDistanceTiers = new int[3]
		{
			10,
			320,
			1000
		};

		private int[] squaredMovementDistanceTiers;

		private int currentMovementDistanceTier;

		private string currentMovementDistanceTierKey = "MovementDistanceTierKey";

		private Vector3 cameraPosition;

		public float innerCollisionCheckRadius = 0.5f;

		private bool flyInCameraDirection;

		private Sound jumpSound;

		private bool initialized;

		private Vector3 positionBeforeCutscene;

		private Quaternion rotationBeforeCutscene;

		private Transform parentBeforeCutscene;

		private Scene sceneBeforeCutscene;

		private GlobalSettings.MovingMode modeBeforeCutscene;

		private int layerBeforeCutscene;

		[HideInInspector]
		public Mountable currentlyMountedMountable;

		private float currentlyMountedHeight;

		private GlobalSettings.MovingMode previousMode;

		private float ySpeedInCameraDirection;

		public bool overwriteYOffset;

		public bool PlayerIsMoving => playerInputX != 0f || playerInputY != 0f || fallPressed || jumpPressed;

		public bool underwater
		{
			get;
			private set;
		}

		private UnderwaterDistortionEffect underwaterEffect => _underwaterEffect ?? (_underwaterEffect = MonoBehaviourSingleton<ImageEffectsController>.get.GetEffect<UnderwaterDistortionEffect>());

		[HideInInspector]
		public bool jumpPressed
		{
			get
			{
				bool? flag = ((object)motor != null) ? new bool?(motor.inputJump) : null;
				return flag.HasValue && flag.Value;
			}
			set
			{
				if (motor == null)
				{
					motor = GetComponent<CharacterMotor>();
				}
				motor.inputJump = value;
			}
		}

		private Material skyboxShader
		{
			get
			{
				if (_skyboxShader == null)
				{
					_skyboxShader = RenderSettings.skybox;
				}
				return _skyboxShader;
			}
		}

		public bool inCutscene
		{
			get;
			private set;
		}

		public bool movementBlock
		{
			get;
			private set;
		}

		private int autoJumpMask => int.MaxValue & ~(1 << LayerMask.NameToLayer("Collectible"));

		private string playerPositionKey
		{
			get
			{
				if (Manager.Contains<SavedWorldManager>())
				{
					return $"player.position.{Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId}";
				}
				return "player.position";
			}
		}

		public Mode GetMode()
		{
			return mode;
		}

		public virtual void ChangeMoveMode(Mode newMode)
		{
			mode = newMode;
		}

		private void Start()
		{
			if (!initialized)
			{
				Init();
			}
		}

		private void Init()
		{
			initialized = true;
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
			MonoBehaviourSingleton<GameplayFacts>.get.RegisterFactChangedListener(this, Fact.MOVE_MODE, Fact.MCPE_STEERING);
			OnFactsChanged(new HashSet<Fact>
			{
				Fact.MOVE_MODE,
				Fact.MCPE_STEERING
			});
			controller = GetComponent<CharacterController>();
			motor = GetComponent<CharacterMotor>();
			cameraAnimator = CameraController.instance.MainCamera.GetComponent<Animator>();
			cameraTransform = cameraAnimator.transform;
			playerAnimator = GetComponent<PlayerGraphic>().graphicRepresentation.GetComponent<Animator>();
			MovementSource = GetComponent<AudioSource>();
			baseControllerHeight = controller.height;
			RefreshCameraPos();
			if (GlobalSettings.mode == GlobalSettings.MovingMode.FLYING)
			{
				ChangeMoveMode(Mode.FLYING);
			}
			else
			{
				ChangeMoveMode(Mode.WALKING);
			}
			currentMovementDistanceTier = PlayerPrefs.GetInt(currentMovementDistanceTierKey, 0);
			SquaredMovementDistance();
			InitMovementStat();
			if (underwaterEffect != null)
			{
				underwaterEffect.enabled = false;
			}
			jumpSound = new Sound
			{
				clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.JUMP),
				mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup
			};
		}

		private void OnDisable()
		{
			if (underwaterEffect != null)
			{
				underwaterEffect.enabled = false;
			}
		}

		private void SquaredMovementDistance()
		{
			squaredMovementDistanceTiers = new int[movementDistanceTiers.Length];
			for (int i = 0; i < movementDistanceTiers.Length; i++)
			{
				squaredMovementDistanceTiers[i] = movementDistanceTiers[i] * movementDistanceTiers[i];
			}
		}

		private void InitMovementStat()
		{
			speedModifier = Manager.Get<ModelManager>().configSettings.GetPlayerMoveSpeed();
			if (stat != null)
			{
				stat.Add(new PlayerStats.Modifier
				{
					value = speedModifier,
					priority = 0,
					Action = ((float toAction, float value) => value + toAction)
				});
				stat.Register(OnStatsChanged);
			}
			flySpeedModifier = Manager.Get<ModelManager>().configSettings.GetPlayerFlySpeed();
			jumpModifier = Manager.Get<ModelManager>().configSettings.GetJumpForce();
		}

		public void EnableMovement(bool enable)
		{
			if (!initialized)
			{
				Init();
			}
			movementBlock = !enable;
			playerInputX = 0f;
			playerInputY = 0f;
			fallPressed = false;
			jumpPressed = false;
			PerformMovement(0f, 0f);
		}

		public void SetControllerSize(Vector3 center, float height, float radius)
		{
			baseControllerHeight = height;
			if (controller == null)
			{
				controller = GetComponent<CharacterController>();
			}
			controller.height = height;
			controller.center = center;
			controller.radius = radius;
		}

		public void StartInteractiveCutscene(Transform container)
		{
			StartCustomCutscene(container);
			inCutscene = false;
		}

		public void StartCustomCutscene(Transform container)
		{
			PlayerGraphic.GetControlledPlayerInstance().HidePlayerGraphic();
			UnityEngine.Debug.LogError("setting fpp");
			CameraController.instance.SetCameraPreset(CameraController.CameraPresets.FPP_CarDriving);
			CameraController.instance.InputSource.IsEnabled = false;
			SaveTransformSettings();
			StartCoroutine(SetRotation(container));
			inCutscene = true;
			movementBlock = true;
			base.transform.SetParent(container);
			Vector3 zero = Vector3.zero;
			zero.y = 0f;
			base.transform.localPosition = zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.SetLayerRecursively(17);
			GlobalSettings.mode = GlobalSettings.MovingMode.FLYING;
			ChangeMoveMode(Mode.FLYING);
			motor.movement.gravity = 0f;
			UpdateMotorParameters();
		}

		private IEnumerator SetRotation(Transform container)
		{
			for (int i = 0; i < 3; i++)
			{
				yield return null;
			}
			CameraController.instance.transform.localRotation = container.localRotation;
		}

		private void SaveTransformSettings()
		{
			parentBeforeCutscene = base.transform.parent;
			positionBeforeCutscene = base.transform.position;
			rotationBeforeCutscene = base.transform.rotation;
			sceneBeforeCutscene = base.gameObject.scene;
			modeBeforeCutscene = GlobalSettings.mode;
			layerBeforeCutscene = base.gameObject.layer;
			PerformMovement(0f, 0f);
			playerAnimator.SetBool("walking", value: false);
		}

		public void EndCustomCutscene()
		{
			LoadTransformSettings();
			inCutscene = false;
			movementBlock = false;
			PlayerGraphic.GetControlledPlayerInstance().ShowPlayerGraphic();
			UpdateCurrentMode();
			CameraController.instance.InputSource.IsEnabled = true;
			CameraController.instance.SetDefaultCameraPreset();
		}

		private void LoadTransformSettings(bool setParent = true)
		{
			if (setParent)
			{
				base.transform.SetParent(parentBeforeCutscene);
			}
			base.transform.localPosition = positionBeforeCutscene;
			base.transform.localRotation = rotationBeforeCutscene;
			GlobalSettings.mode = modeBeforeCutscene;
			base.transform.SetLayerRecursively(layerBeforeCutscene);
			if (base.transform.parent == null)
			{
				SceneManager.MoveGameObjectToScene(base.gameObject, sceneBeforeCutscene);
			}
		}

		public void StartCutscene(GameObject cutsceneContainer, CutsceneVoxelEvents cutscene)
		{
			PlayerGraphic.GetControlledPlayerInstance().HidePlayerGraphic();
			SaveTransformSettings();
			CameraController.instance.SetCameraPreset(CameraController.CameraPresets.FPP);
			CameraController.instance.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
			CameraController.instance.InputSource.IsEnabled = false;
			inCutscene = true;
			movementBlock = true;
			base.transform.SetParent(cutsceneContainer.transform, worldPositionStays: true);
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			Animator animator = base.gameObject.AddComponent<Animator>();
			animator.runtimeAnimatorController = cutscene.cutsceneAnimatorController;
			StartCoroutine(EndCutscene(cutsceneContainer, cutscene));
		}

		private IEnumerator EndCutscene(GameObject cutsceneContainer, CutsceneVoxelEvents cutscene)
		{
			yield return new WaitForSeconds(cutscene.duration);
			UnityEngine.Object.Destroy(GetComponent<Animator>());
			CameraController.instance.SetDefaultCameraPreset();
			CameraController.instance.InputSource.IsEnabled = true;
			base.transform.SetParent(null, worldPositionStays: true);
			LoadTransformSettings(setParent: false);
			movementBlock = false;
			inCutscene = false;
			PlayerGraphic.GetControlledPlayerInstance().ShowPlayerGraphic();
			UpdateCurrentMode();
		}

		public bool IsGrounded()
		{
			return motor.IsGrounded();
		}

		public bool IsMounted()
		{
			return mode == Mode.MOUNTED || mode == Mode.MOUNTED_FLYING || mode == Mode.MOUNTED_VEHICLE;
		}

		public void Mount(Mountable mountable)
		{
			if (currentlyMountedMountable != null)
			{
				Unmount();
			}
			currentlyMountedMountable = mountable;
			if (currentlyMountedMountable.isVehicle)
			{
				if (currentlyMountedMountable.GetComponent<TrainMountable>().canBeControlled)
				{
					mountable.MountVehicle(base.transform);
				}
				else
				{
					mountable = mountable.GetClosestTrain();
					currentlyMountedMountable = mountable;
					mountable.MountVehicle(base.transform);
				}
			}
			else
			{
				CameraController.instance.SetCameraPreset(CameraController.CameraPresets.FPP);
				PlayerGraphic.GetControlledPlayerInstance().TryToEnableHeadThings(enable: false);
				mountable.MountMob(base.transform);
			}
			currentlyMountedHeight = mountable.GetMountHeight();
			base.transform.Translate(Vector3.up * (currentlyMountedHeight - controller.height + 0.01f));
			controller.height = currentlyMountedHeight;
			RefreshCameraPos();
			previousMode = GlobalSettings.mode;
			if (mountable.isFlying)
			{
				ChangeMoveMode(Mode.MOUNTED_FLYING);
				GlobalSettings.mode = GlobalSettings.MovingMode.FLYING;
			}
			else if (currentlyMountedMountable.isVehicle)
			{
				ChangeMoveMode(Mode.MOUNTED_VEHICLE);
			}
			else
			{
				ChangeMoveMode(Mode.MOUNTED);
			}
		}

		public void Unmount()
		{
			CameraController.instance.SetDefaultCameraPreset();
			PlayerGraphic.GetControlledPlayerInstance().TryToEnableHeadThings(enable: true);
			currentlyMountedMountable.Unmount();
			controller.height = baseControllerHeight;
			base.transform.Translate(0.5f * Vector3.up * (currentlyMountedHeight + baseControllerHeight + 1f));
			RefreshCameraPos();
			currentlyMountedMountable = null;
			currentlyMountedHeight = 0f;
			GlobalSettings.mode = previousMode;
			ChangeMoveMode((GlobalSettings.mode == GlobalSettings.MovingMode.FLYING) ? Mode.FLYING : Mode.WALKING);
		}

		private void RefreshCameraPos()
		{
		}

		private Color FindWaterColor()
		{
			GameObject chunkObject = Engine.ChunkManagerInstance.ChunkObject;
			Renderer[] componentsInChildren = chunkObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				Material[] sharedMaterials = renderer.sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					if (material.name.Contains("Water"))
					{
						return material.color;
					}
				}
			}
			return Color.blue;
		}

		private void Update()
		{
			CheckPlayerYPosition();
			InformOnMovement();
			FixPlayerPosition();
		}

		public void TeleportToStartPosition()
		{
			base.transform.position = Engine.EngineInstance.startPlayerPosition;
			lastPosition = base.transform.position;
		}

		private void FixedUpdate()
		{
			if (!movementBlock)
			{
				PerformMovement(playerInputX, playerInputY);
				MonoBehaviourSingleton<LimitedWorld>.get.RaiseEvent(new DataLW
				{
					target = this
				}, EventTypeLW.PlayerMoved);
				UpdateAnimatorState();
			}
			else
			{
				MovementSource.Stop();
			}
			UpdateInertia();
			motor.UpdateFunction();
		}

		public void OnFactsChanged(HashSet<Fact> facts)
		{
			if (facts.Contains(Fact.MOVE_MODE))
			{
				UpdateMode();
			}
			if (facts.Contains(Fact.MCPE_STEERING))
			{
				McpeContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<McpeContext>(Fact.MCPE_STEERING);
				if (factContext == null)
				{
					flyInCameraDirection = false;
				}
				else
				{
					flyInCameraDirection = factContext.flyInCameraDirection;
				}
			}
		}

		private void UpdateMode()
		{
			if (GlobalSettings.mode == GlobalSettings.MovingMode.FLYING)
			{
				if (!IsMounted())
				{
					ChangeMoveMode(Mode.FLYING);
				}
			}
			else if (mode == Mode.FLYING)
			{
				ChangeMoveMode(Mode.WALKING);
			}
		}

		public void GetUnderwaterStatus()
		{
			VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(cameraTransform.position);
			VoxelInfo voxelInfo2 = Engine.PositionToVoxelInfo(cameraTransform.position + Vector3.up);
			if (voxelInfo == null || voxelInfo2 == null)
			{
				return;
			}
			ushort voxel = voxelInfo.GetVoxel();
			ushort voxel2 = voxelInfo2.GetVoxel();
			if (voxel == Engine.usefulIDs.waterBlockID || voxel2 == Engine.usefulIDs.waterBlockID)
			{
				if (!underwater)
				{
					UnderwaterStatusWillChange();
				}
				underwater = true;
			}
			else
			{
				if (underwater)
				{
					UnderwaterStatusWillChange();
				}
				underwater = false;
			}
			if (underwater && !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.UNDERWATER))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.UNDERWATER, new UnderwaterContext
				{
					waterColor = FindWaterColor()
				});
			}
			else if (!underwater)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.UNDERWATER, enabled: false);
			}
			UpdateSkyboxUnderwater();
		}

		protected virtual void UnderwaterStatusWillChange()
		{
		}

		public void UpdateSkyboxUnderwater()
		{
			if (skyboxShader == null)
			{
				return;
			}
			if (underwater)
			{
				float num = (float)Engine.TerrainGenerator.waterHeight - 1.49f;
				Vector3 localPosition = base.transform.localPosition;
				float num2 = Mathf.Clamp01((num - localPosition.y) / 5f);
				if (num2 > 0.2f)
				{
					if (underwaterEffect != null)
					{
						underwaterEffect.TryEnable();
					}
				}
				else if (underwaterEffect != null)
				{
					underwaterEffect.enabled = false;
				}
			}
			else if (underwaterEffect != null)
			{
				underwaterEffect.enabled = false;
			}
		}

		public void UpdateMotorParameters()
		{
			motor.jumping.baseHeight = 0.6f;
			if (mode == Mode.WALKING || (mode == Mode.FLYING && underwater))
			{
				UpdateWalkingMotor();
			}
			else if (mode == Mode.MOUNTED)
			{
				UpdateMountedMotor();
			}
			else if (mode == Mode.FLYING)
			{
				UpdateFlyingMotor();
			}
			else if (mode == Mode.MOUNTED_FLYING)
			{
				UpdateMountedFlyingMotor();
			}
			else if (mode == Mode.MOUNTED_VEHICLE)
			{
				UpdateMountedVehicleMotor();
			}
		}

		private void UpdateWalkingMotor()
		{
			if (underwater && flyInCameraDirection)
			{
				UpdateSwimmingInCameraDirectionMotor();
				return;
			}
			motor.movement.maxForwardSpeed = walkingMaxForwardSpeed * speedModifier;
			motor.movement.maxSidewaysSpeed = walkingMaxSidewaysSpeed * speedModifier;
			motor.movement.maxBackwardsSpeed = walkingMaxBackwardsSpeed * speedModifier;
			if (underwater)
			{
				motor.movement.maxFallSpeed = 6f * speedModifier;
				motor.jumping.extraHeight = 8f * speedModifier;
				float num = (float)Engine.TerrainGenerator.waterHeight - 1.54f;
				Vector3 localPosition = base.transform.localPosition;
				float num2 = Mathf.Lerp(18f, 0f, Mathf.Clamp01(num - localPosition.y));
				motor.movement.gravity = num2 * speedModifier;
			}
			else
			{
				motor.movement.gravity = 18f;
				motor.movement.maxFallSpeed = 40f;
				motor.jumping.extraHeight = 0.5f * jumpModifier;
			}
		}

		private void UpdateSwimmingInCameraDirectionMotor()
		{
			Vector3 forward = BaseCameraRig.Camera.transform.forward;
			float num = Mathf.Sqrt(Mathf.Pow(forward.x, 2f) + Mathf.Pow(forward.z, 2f));
			float num2 = (!MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper) ? 1f : PlayerPrefs.GetFloat("fastMovement", 1f);
			float num3 = (float)Engine.TerrainGenerator.waterHeight - 1.54f;
			Vector3 localPosition = base.transform.localPosition;
			float num4 = Mathf.Lerp(18f, 0f, Mathf.Clamp01(num3 - localPosition.y));
			motor.movement.gravity = num4 * speedModifier * num2;
			motor.movement.maxFallSpeed = 6f * speedModifier * num2;
			motor.jumping.extraHeight = 8f * speedModifier;
			motor.movement.maxForwardSpeed = num2 * walkingMaxForwardSpeed * num * speedModifier;
			motor.movement.maxSidewaysSpeed = num2 * walkingMaxSidewaysSpeed * num * speedModifier;
			motor.movement.maxBackwardsSpeed = num2 * walkingMaxBackwardsSpeed * num * speedModifier;
			if (playerInputY != 0f)
			{
				ySpeedInCameraDirection = forward.y * playerInputY;
				if (IsGrounded() && ySpeedInCameraDirection > 0.2f)
				{
					jumpPressed = true;
				}
			}
			else
			{
				ySpeedInCameraDirection = 0f;
			}
		}

		private void UpdateMountedMotor()
		{
			motor.movement.maxForwardSpeed = walkingMaxForwardSpeed * currentlyMountedMountable.speedMultiplier * speedModifier;
			motor.movement.maxSidewaysSpeed = walkingMaxSidewaysSpeed * Mathf.Max(1f, currentlyMountedMountable.speedMultiplier / 2f) * speedModifier;
			motor.movement.maxBackwardsSpeed = walkingMaxBackwardsSpeed * Mathf.Max(1f, currentlyMountedMountable.speedMultiplier / 2f) * speedModifier;
			if (underwater)
			{
				motor.movement.gravity = 1f * speedModifier;
				motor.movement.maxFallSpeed = 1f * speedModifier;
				motor.jumping.extraHeight = 8f * speedModifier;
				if (motor.inputJump)
				{
					motor.movement.velocity = Vector3.up * 2.25f * speedModifier;
				}
			}
			else
			{
				motor.movement.gravity = 18f;
				motor.movement.maxFallSpeed = 40f;
				motor.jumping.extraHeight = 0.5f * currentlyMountedMountable.jumpHightMult * jumpModifier;
			}
		}

		private void UpdateFlyingMotor()
		{
			if (flyInCameraDirection)
			{
				UpdateFlyInCameraDirectionMotor();
				return;
			}
			motor.movement.gravity = 0f;
			motor.movement.maxFallSpeed = 0f;
			motor.jumping.extraHeight = 0.6f * flySpeedModifier;
			float num = (!MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper) ? 1f : PlayerPrefs.GetFloat("fastMovement", 1f);
			motor.movement.maxForwardSpeed = num * 6f * flySpeedModifier;
			motor.movement.maxSidewaysSpeed = num * 6f * flySpeedModifier;
			motor.movement.maxBackwardsSpeed = num * 6f * flySpeedModifier;
			if (motor.inputJump)
			{
				motor.movement.velocity = num * Vector3.up * 4f * flySpeedModifier;
			}
			else if (fallPressed)
			{
				motor.movement.gravity = num * 10f * flySpeedModifier;
				motor.movement.maxFallSpeed = num * 10f * flySpeedModifier;
			}
			else
			{
				motor.movement.velocity = Vector3.zero;
			}
		}

		private void UpdateFlyInCameraDirectionMotor()
		{
			Vector3 forward = BaseCameraRig.Camera.transform.forward;
			float num = Mathf.Sqrt(Mathf.Pow(forward.x, 2f) + Mathf.Pow(forward.z, 2f));
			motor.movement.gravity = 0f;
			motor.movement.maxFallSpeed = 0f;
			motor.jumping.baseHeight = 0f;
			motor.jumping.extraHeight = 0f;
			float num2 = (!MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper) ? 1f : PlayerPrefs.GetFloat("fastMovement", 1f);
			motor.movement.maxForwardSpeed = num2 * 6f * flySpeedModifier * num;
			motor.movement.maxSidewaysSpeed = num2 * 6f * flySpeedModifier * num;
			motor.movement.maxBackwardsSpeed = num2 * 6f * flySpeedModifier * num;
			if (playerInputY != 0f)
			{
				float num3 = forward.y * playerInputY;
				if (num3 >= 0f)
				{
					motor.movement.velocity = num2 * Vector3.up * 6.5f * flySpeedModifier * num3;
				}
				else
				{
					motor.movement.gravity = 10000f;
					motor.movement.maxFallSpeed = num2 * 6.5f * flySpeedModifier * (0f - num3);
				}
				if (IsGrounded() && num3 > 0.2f)
				{
					jumpPressed = true;
				}
			}
			else
			{
				motor.movement.velocity = Vector3.zero;
			}
		}

		private void UpdateMountedFlyingMotor()
		{
			if (flyInCameraDirection)
			{
				UpdateFlyInCameraDirectionMountedFlyingMotor();
				return;
			}
			motor.movement.gravity = 0f;
			motor.movement.maxFallSpeed = 0f;
			motor.jumping.extraHeight = 0.6f * flySpeedModifier;
			motor.movement.maxForwardSpeed = walkingMaxForwardSpeed * currentlyMountedMountable.speedMultiplier * flySpeedModifier;
			motor.movement.maxSidewaysSpeed = walkingMaxSidewaysSpeed * Mathf.Max(1f, currentlyMountedMountable.speedMultiplier / 2f) * flySpeedModifier;
			motor.movement.maxBackwardsSpeed = walkingMaxBackwardsSpeed * Mathf.Max(1f, currentlyMountedMountable.speedMultiplier / 2f) * flySpeedModifier;
			if (motor.inputJump)
			{
				motor.movement.velocity = Vector3.up * 4f * flySpeedModifier;
			}
			else if (fallPressed)
			{
				motor.movement.gravity = 10f * flySpeedModifier;
				motor.movement.maxFallSpeed = 10f * flySpeedModifier;
			}
			else
			{
				motor.movement.velocity = Vector3.zero;
			}
		}

		private void UpdateFlyInCameraDirectionMountedFlyingMotor()
		{
			Vector3 forward = BaseCameraRig.Camera.transform.forward;
			float num = Mathf.Sqrt(Mathf.Pow(forward.x, 2f) + Mathf.Pow(forward.z, 2f));
			motor.movement.gravity = 0f;
			motor.movement.maxFallSpeed = 0f;
			motor.jumping.extraHeight = 0.6f * flySpeedModifier;
			motor.movement.gravity = 0f;
			motor.movement.maxFallSpeed = 0f;
			motor.jumping.extraHeight = 0.6f * flySpeedModifier;
			motor.movement.maxForwardSpeed = walkingMaxForwardSpeed * currentlyMountedMountable.speedMultiplier * flySpeedModifier * num;
			motor.movement.maxSidewaysSpeed = walkingMaxSidewaysSpeed * Mathf.Max(1f, currentlyMountedMountable.speedMultiplier / 2f) * flySpeedModifier * num;
			motor.movement.maxBackwardsSpeed = walkingMaxBackwardsSpeed * Mathf.Max(1f, currentlyMountedMountable.speedMultiplier / 2f) * flySpeedModifier * num;
			if (playerInputY != 0f)
			{
				float num2 = forward.y * playerInputY;
				if (num2 >= 0f)
				{
					motor.movement.velocity = Vector3.up * 6.5f * flySpeedModifier * num2;
				}
				else
				{
					motor.movement.gravity = 10000f;
					motor.movement.maxFallSpeed = 6.5f * flySpeedModifier * (0f - num2);
				}
				if (IsGrounded() && num2 > 0.2f)
				{
					motor.inputJump = true;
				}
			}
			else
			{
				motor.movement.velocity = Vector3.zero;
			}
		}

		private void UpdateMountedVehicleMotor()
		{
			motor.movement.maxForwardSpeed = walkingMaxForwardSpeed * currentlyMountedMountable.speedMultiplier * speedModifier;
			motor.movement.maxSidewaysSpeed = walkingMaxSidewaysSpeed * currentlyMountedMountable.speedMultiplier * speedModifier;
			motor.movement.maxBackwardsSpeed = walkingMaxBackwardsSpeed * currentlyMountedMountable.speedMultiplier * speedModifier;
			if (motor.inputJump)
			{
				motor.movement.velocity = Vector3.zero;
			}
			motor.movement.gravity = 18f;
			motor.movement.maxFallSpeed = 40f;
			motor.jumping.extraHeight = 0f;
		}

		private void UpdateInertia()
		{
			if (underwater)
			{
				if (!PlayerIsMoving)
				{
					PerformMovement(0f, 0f);
				}
				JumpAndFallUnderwater();
				wasInWater = true;
			}
			else if (wasInWater)
			{
				wasInWater = false;
				hVelocityY.Reset();
			}
		}

		private void JumpAndFallUnderwater()
		{
			float num = 0f;
			num = (motor.inputJump ? hVelocityY.GetOutput(1f) : ((!fallPressed) ? hVelocityY.GetOutput(0f) : hVelocityY.GetOutput(-1f)));
			if (flyInCameraDirection)
			{
				num = ySpeedInCameraDirection * 3f;
			}
			Vector3 velocity = motor.movement.velocity;
			velocity.y = num * speedModifier;
			motor.movement.velocity = velocity;
			UpdateUnderwaterStrenght();
		}

		private void UpdateUnderwaterStrenght()
		{
			if (stopMoving && motor.movement.velocity.sqrMagnitude > 0.2f)
			{
				stopMoving = false;
			}
			else if (!stopMoving && motor.movement.velocity.sqrMagnitude < 0.2f)
			{
				stopMoving = true;
			}
			if (stopMoving)
			{
				if (underwaterEffect != null)
				{
					underwaterEffect.LerpStrength(1f);
				}
			}
			else if (!stopMoving && underwaterEffect != null)
			{
				underwaterEffect.LerpStrength(-2f);
			}
		}

		private void PerformMovement(float horizontal, float vertical)
		{
			if (underwater)
			{
				horizontal = hVelocityX.GetOutput(horizontal) * Time.fixedDeltaTime;
				vertical = hVelocityZ.GetOutput(vertical) * Time.fixedDeltaTime;
			}
			Vector3 vector = new Vector3(horizontal, 0f, vertical);
			if (vector != Vector3.zero)
			{
				float magnitude = vector.magnitude;
				vector /= magnitude;
				magnitude = Mathf.Min(1f, magnitude);
				magnitude *= magnitude;
				vector *= magnitude;
			}
			if (currentlyMountedMountable != null && currentlyMountedMountable.isVehicle)
			{
				currentlyMountedMountable.Move(base.transform, vector, vertical);
				motor.inputMoveDirection = Vector3.zero;
			}
			else
			{
				motor.inputMoveDirection = base.transform.rotation * vector;
			}
			if (currentlyMountedMountable != null && mode != Mode.MOUNTED_VEHICLE)
			{
				currentlyMountedMountable.simulateMovement = (motor.inputMoveDirection.magnitude > 0.1f);
			}
			bool flag = false;
			bool flag2 = false;
			bool enabled = false;
			if (underwater || horizontal != 0f || vertical != 0f)
			{
				flag = CheckForAutoJump();
				flag2 = IsGrounded();
				enabled = flag2;
			}
			if (currentlyMountedMountable != null && currentlyMountedMountable.isVehicle)
			{
				flag2 = false;
				enabled = false;
			}
			if (underwater)
			{
				flag2 = false;
				enabled = true;
				if (previousSound != GameSound.SWIM)
				{
					MovementSource.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.SWIM);
					previousSound = GameSound.SWIM;
				}
			}
			else if (mode == Mode.FLYING && !IsGrounded())
			{
				enabled = true;
				if (previousSound != GameSound.FLY)
				{
					MovementSource.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.FLY);
					previousSound = GameSound.FLY;
				}
			}
			else if (previousSound != GameSound.WALK)
			{
				MovementSource.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.WALK);
				previousSound = GameSound.WALK;
			}
			cameraAnimator.SetBool("bob", allowCameraBob && flag2);
			MovementSource.enabled = enabled;
			if (MovementSource.enabled && !MovementSource.isPlaying)
			{
				MovementSource.Play();
			}
			if (mode == Mode.MOUNTED_VEHICLE)
			{
				flag = false;
				jumpPressed = false;
			}
			if (flag)
			{
				jumpPressed = true;
			}
			if (jumpPressed && IsGrounded())
			{
				if (!underwater)
				{
					jumpSound.Play();
				}
				Manager.Get<QuestManager>().OnJump();
			}
		}

		private bool CheckForAutoJump()
		{
			Vector3 position = base.transform.position;
			if (currentlyMountedHeight != 0f)
			{
				position.y += (0f - currentlyMountedHeight) / 2f + 0.5f;
			}
			UnityEngine.Debug.DrawRay(position, base.transform.forward, Color.blue);
			UnityEngine.Debug.DrawRay(base.transform.position, base.transform.forward, Color.red);
			if (Physics.Raycast(position, base.transform.forward, 1f, autoJumpMask) && !Physics.Raycast(position, Vector3.up, 2f, autoJumpMask) && !Physics.Raycast(position + Vector3.up, base.transform.forward, 1f, autoJumpMask) && !Physics.Raycast(position + Vector3.up * 2f, base.transform.forward, 1f, autoJumpMask))
			{
				return true;
			}
			return false;
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

		private void InformOnMovement()
		{
			if (!IsGrounded())
			{
				Vector3 position = base.transform.position;
				if (position.y > positionYGrounded)
				{
					Vector3 position2 = base.transform.position;
					positionYGrounded = position2.y;
				}
			}
			else if (positionYGrounded > float.MinValue)
			{
				QuestManager questManager = Manager.Get<QuestManager>();
				float num = positionYGrounded;
				Vector3 position3 = base.transform.position;
				questManager.OnFall(Mathf.RoundToInt(num - position3.y));
				positionYGrounded = float.MinValue;
			}
			float num2 = Vector3.Distance(lastPosition, base.transform.position);
			TryUpdateStatsManager();
			if (num2 > 0f)
			{
				Singleton<PlayerData>.get.playerQuests.InformOnPlayerWalk(base.transform.position, num2, mode, underwater);
				if (MonoBehaviourSingleton<LimitedWorld>.get.active)
				{
					MonoBehaviourSingleton<LimitedWorld>.get.RaiseEvent(new DataLW
					{
						target = this
					}, EventTypeLW.PlayerMoved);
				}
			}
			lastPosition = base.gameObject.transform.position;
		}

		private void TryUpdateStatsManager()
		{
			if (currentMovementDistanceTier < squaredMovementDistanceTiers.Length)
			{
				float num = squaredMovementDistanceTiers[currentMovementDistanceTier];
				float sqrMagnitude = (Engine.EngineInstance.startPlayerPosition.XZ() - base.transform.position.XZ()).sqrMagnitude;
				if (sqrMagnitude > num)
				{
					Manager.Get<StatsManager>().PlayerMovement(movementDistanceTiers[currentMovementDistanceTier]);
					currentMovementDistanceTier++;
					PlayerPrefs.SetInt(currentMovementDistanceTierKey, currentMovementDistanceTier);
				}
				if (sqrMagnitude > 0.5f)
				{
					StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.PLAYER_MOVED);
				}
			}
		}

		protected virtual void UpdateAnimatorState()
		{
			if (IsMounted() || mode != 0)
			{
				playerAnimator.SetBool("walking", value: false);
			}
			else if (playerInputX != 0f || playerInputY != 0f)
			{
				playerAnimator.SetBool("walking", value: true);
			}
			else
			{
				playerAnimator.SetBool("walking", value: false);
			}
		}

		private void OnStatsChanged()
		{
			speedModifier = stat.GetStats();
		}

		public void OnGameSavedFrequent()
		{
			string playerPositionKey = this.playerPositionKey;
			object[] array = new object[5];
			Vector3 position = base.transform.position;
			array[0] = position.x;
			array[1] = " ";
			Vector3 position2 = base.transform.position;
			array[2] = position2.y;
			array[3] = " ";
			Vector3 position3 = base.transform.position;
			array[4] = position3.z;
			PlayerPrefs.SetString(playerPositionKey, string.Concat(array));
		}

		public void OnGameSavedInfrequent()
		{
		}

		public void ClearPositionFromThisWorld()
		{
			PlayerPrefs.DeleteKey(playerPositionKey);
		}

		public void OnGameplayStarted()
		{
			if (PlayerPrefs.HasKey(playerPositionKey))
			{
				string @string = PlayerPrefs.GetString(playerPositionKey);
				string[] array = @string.Split(' ');
				float num = 0f;
				if (!overwriteYOffset)
				{
					num = ((GlobalSettings.mode != GlobalSettings.MovingMode.FLYING) ? 50 : 0);
				}
				Vector3 position = new Vector3(float.Parse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture) + num, float.Parse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture));
				base.transform.position = position;
			}
			else
			{
				base.transform.position = Engine.EngineInstance.startPlayerPosition;
				Transform transform = base.transform;
				Vector3 eulerAngles = base.transform.eulerAngles;
				float x = eulerAngles.x;
				float startPlayerYRotation = Engine.EngineInstance.startPlayerYRotation;
				Vector3 eulerAngles2 = base.transform.eulerAngles;
				transform.eulerAngles = new Vector3(x, startPlayerYRotation, eulerAngles2.z);
				if (Engine.EngineInstance.startPlayerVehiclePrefab != null)
				{
					SpawnAndMountVehicle(Engine.EngineInstance.startPlayerVehiclePrefab);
				}
				if (Engine.EngineInstance.startPlayerMountablePrefab != null)
				{
					SpawnAndMountMountable(Engine.EngineInstance.startPlayerMountablePrefab);
				}
			}
			ResetGraphicRotation();
		}
		private void ResetGraphicRotation() { }
		/*[DebuggerStepThrough]
		[AsyncStateMachine (typeof(_003CResetGraphicRotation_003Ec__async2))]
		private void ResetGraphicRotation()
		
			_003CResetGraphicRotation_003Ec__async2 stateMachine = default(_003CResetGraphicRotation_003Ec__async2);
			stateMachine._0024this = this;
			stateMachine._0024builder = AsyncVoidMethodBuilder.Create();
			stateMachine._0024builder.Start(ref stateMachine);*/


		public GameObject SpawnAndMountVehicle(VehicleController vehiclePrefab)
		{
			VehicleHoverAction hoverAction = UnityEngine.Object.FindObjectOfType<CameraEventsSender>().GetHoverAction<VehicleHoverAction>();
			if (hoverAction == null)
			{
				return null;
			}
			GameObject gameObject = vehiclePrefab.gameObject;
			GameObject gameObject2 = SpawnMount(gameObject, vehiclePrefab.isWaterVehicle);
			if (gameObject2 == null)
			{
				return null;
			}
			gameObject2.name = gameObject.name;
			hoverAction.ForceEnterVehicle(gameObject2);
			return gameObject2;
		}

		private void SpawnAndMountMountable(Mountable mountable)
		{
			MountMobsHoverAction hoverAction = UnityEngine.Object.FindObjectOfType<CameraEventsSender>().GetHoverAction<MountMobsHoverAction>();
			if (hoverAction != null)
			{
				GameObject gameObject = SpawnMount(mountable.gameObject);
				hoverAction.ForceMount(gameObject.GetComponentInChildren<Mountable>());
			}
		}

		private GameObject SpawnMount(GameObject prefabRoot, bool onlyOnWaterSurface = false)
		{
			while (prefabRoot.transform.parent != null)
			{
				prefabRoot = prefabRoot.transform.parent.gameObject;
			}
			Vector3 position = base.transform.position;
			if (onlyOnWaterSurface && !GetWaterSurfacePosition(ref position))
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefabRoot);
			gameObject.transform.position = position;
			gameObject.transform.rotation = base.transform.rotation;
			return gameObject;
		}

		private bool GetWaterSurfacePosition(ref Vector3 position)
		{
			VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position);
			if (voxelInfo == null)
			{
				return true;
			}
			ushort voxel = voxelInfo.GetVoxel();
			int num = 0;
			if (voxel == Engine.usefulIDs.waterBlockID)
			{
				while (voxel == Engine.usefulIDs.waterBlockID)
				{
					num++;
					voxel = Engine.PositionToVoxelInfo(base.transform.position + Vector3.up * num).GetVoxel();
				}
				position = base.transform.position + Vector3.up * num;
				return true;
			}
			return false;
		}

		public void ForceUnmount()
		{
			Object.FindObjectOfType<CameraEventsSender>().GetHoverAction<MountMobsHoverAction>()?.Unmount();
		}

		public void OnGameplayRestarted()
		{
			PlayerPrefs.DeleteKey(playerPositionKey);
		}

		public void UpdateCurrentMode()
		{
			ChangeMoveMode(mode);
		}

		private void OnDestroy()
		{
			if (Manager.Get<GameCallbacksManager>() != null)
			{
				Manager.Get<GameCallbacksManager>().UnregisterObject(this);
			}
			if (MonoBehaviourSingleton<GameplayFacts>.get != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.UnregisterFactChangedListener(this, Fact.UNDERWATER);
			}
		}

		private void FixPlayerPosition()
		{
			if (base.gameObject.layer == LayerMask.NameToLayer("NoClipMode"))
			{
				return;
			}
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			float y = position2.y + innerCollisionCheckRadius;
			Vector3 position3 = base.transform.position;
			if (!Physics.Raycast(new Vector3(x, y, position3.z), Vector3.down, innerCollisionCheckRadius * 2f, 1 << LayerMask.NameToLayer("Terrain")))
			{
				Vector3 position4 = base.transform.position;
				float x2 = position4.x;
				Vector3 position5 = base.transform.position;
				float y2 = position5.y - innerCollisionCheckRadius;
				Vector3 position6 = base.transform.position;
				if (!Physics.Raycast(new Vector3(x2, y2, position6.z), Vector3.up, innerCollisionCheckRadius * 2f, 1 << LayerMask.NameToLayer("Terrain")))
				{
					return;
				}
			}
			base.transform.position += Vector3.up;
		}
	}
}
