// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.CameraEventsSender
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using GameUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uniblocks
{
	public class CameraEventsSender : MonoBehaviour
	{
		private Camera mainCam;

		private const float RANGE = 5f;

		private Func<bool> jumpButton;

		private Func<bool> fallButton;

		private PlayerMovement movement;

		private List<HoverAction> hoverActions;

		private HoverActionsConnector hoverConnector;

		public RaycastHitInfo raycastHitInfo;

		private VoxelHoverAction voxelHoverAction;

		public static UniversalAnalogInput MainInput
		{
			get;
			private set;
		}

		public RaycastHit inFrontObject => raycastHitInfo.hit;

		private McpeSteering mcpeSteering => MonoBehaviourSingleton<McpeSteering>.get;

		private void Awake()
		{
			movement = GetComponent<PlayerMovement>();
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MAIN_ANALOG, new AnalogInputContext
			{
				setAnalogController = delegate(AnalogController ac, SimpleRepeatButton b)
				{
					if (MainInput == null)
					{
						MainInput = new UniversalAnalogInput(ac, b);
					}
					else
					{
						MainInput.Init(ac, b);
					}
				}
			});
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MOVEMENT, new MovementContext
			{
				setFallButton = delegate(Func<bool> f)
				{
					fallButton = f;
				},
				setJumpButton = delegate(Func<bool> j)
				{
					jumpButton = j;
				}
			});
			InitHoverActions();
		}

		private void Start()
		{
			mainCam = CameraController.instance.MainCamera;
		}

		public T GetHoverAction<T>() where T : HoverAction
		{
			foreach (HoverAction hoverAction in hoverActions)
			{
				if (hoverAction.GetType() == typeof(T))
				{
					return hoverAction as T;
				}
			}
			return (T)null;
		}

		private void InitHoverActions()
		{
			hoverConnector = new HoverActionsConnector(movement);
			hoverActions = new List<HoverAction>();
			hoverActions.Add(new MountMobsHoverAction(hoverConnector));
			hoverActions.Add(new TameMobsHoverAction(hoverConnector));
			hoverActions.Add(new ChangeClothesHoverAction(hoverConnector));
			hoverActions.Add(new TalkHoverAction(hoverConnector));
			hoverActions.Add(new CutsceneHoverAction(hoverConnector));
			hoverActions.Add(new SwitchHoverAction(hoverConnector));
			hoverActions.Add(new VehicleHoverAction(hoverConnector));
			hoverActions.Add(new FixVehicleHoverAction(hoverConnector));
			hoverActions.Add(new InteractivePrefabHoverAction(hoverConnector));
			hoverActions.Add(new UsableInsideHoverAction(hoverConnector));
			if (Manager.Get<ModelManager>().fishingSettings.IsFishingEnabled())
			{
				hoverActions.Add(new FishingHoverAction(hoverConnector));
			}
			if (Manager.Contains<HospitalManager>())
			{
				hoverActions.Add(new CurePatientHoverAction(hoverConnector));
			}
			if (Manager.Contains<LoveManager>())
			{
				hoverActions.Add(new LoveHoverAction(hoverConnector));
			}
			if (Manager.Contains<FarmingManager>())
			{
				hoverActions.Add(new FarmingHoverAction(hoverConnector));
			}
			if (Manager.Contains<SurvivalManager>())
			{
				SurvivalVoxelHoverAction item = (SurvivalVoxelHoverAction)(voxelHoverAction = new SurvivalVoxelHoverAction(hoverConnector));
				hoverActions.Add(item);
				hoverActions.Add(new SurvivalDoorsHoverAction(hoverConnector));
			}
			else
			{
				VoxelHoverAction item2 = voxelHoverAction = new VoxelHoverAction(hoverConnector);
				hoverActions.Add(item2);
				hoverActions.Add(new DoorsHoverAction(hoverConnector));
			}
			mcpeSteering.RegisterVoxelHoverActionsCallbacks(voxelHoverAction.OnAdd, voxelHoverAction.OnDig);
		}

		public void Update()
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING) && !mcpeSteering.isMoving && mcpeSteering.isPointerDown)
			{
				UpdateMcpeSteeringHoverActions();
			}
			else
			{
				UpdateHoverActions();
			}
			UpdateInput();
		}

		private void UpdateHoverActions()
		{
			float num = 5f + Vector3.Distance(mainCam.transform.position, PlayerGraphic.GetControlledPlayerInstance().transform.position);
			VoxelInfo voxelHit = Engine.VoxelRaycast(mainCam.transform.position, mainCam.transform.forward, num, ignoreTransparent: false);
			RaycastHit hitInfo = default(RaycastHit);
			Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hitInfo, num, ~LayerMask.GetMask("Terrain", "Ignore Raycast"));
			List<VoxelInfo> gridRaycast = Engine.VoxelGridRaycastAll(mainCam.transform.position, mainCam.transform.forward, num);
			raycastHitInfo.hit = hitInfo;
			raycastHitInfo.voxelHit = voxelHit;
			raycastHitInfo.gridRaycast = gridRaycast;
			foreach (HoverAction hoverAction in hoverActions)
			{
				hoverAction.Update(raycastHitInfo);
			}
		}

		public void UpdateMcpeSteeringHoverActions()
		{
			mcpeSteering.passedHitTest = false;
			if (!mcpeSteering.pointerID.HasValue)
			{
				return;
			}
			int value = mcpeSteering.pointerID.Value;
			int i;
			for (i = 0; i < UnityEngine.Input.touchCount && UnityEngine.Input.GetTouch(i).fingerId != value; i++)
			{
			}
			if (i < UnityEngine.Input.touchCount)
			{
				Vector2 position = UnityEngine.Input.GetTouch(i).position;
				float interactionDistance = mcpeSteering.interactionDistance;
				Ray ray = CameraController.instance.MainCamera.ScreenPointToRay(position);
				mcpeSteering.blueprintVoxelDirectHit = Engine.VoxelGridRaycast(ray, interactionDistance);
				VoxelInfo voxelInfo = Engine.VoxelRaycast(ray, interactionDistance, ignoreTransparent: false);
				if (voxelInfo == null)
				{
					raycastHitInfo.voxelHit = null;
					return;
				}
				mcpeSteering.passedHitTest = true;
				RaycastHit hitInfo = default(RaycastHit);
				Physics.Raycast(ray, out hitInfo, interactionDistance, ~LayerMask.GetMask("Terrain", "Ignore Raycast"));
				raycastHitInfo.hit = hitInfo;
				raycastHitInfo.voxelHit = voxelInfo;
				List<VoxelInfo> gridRaycast = Engine.VoxelGridRaycastAll(ray.origin, ray.direction, interactionDistance);
				raycastHitInfo.gridRaycast = gridRaycast;
				voxelHoverAction.Update(raycastHitInfo);
			}
		}

		private void UpdateInput()
		{
			if (MainInput == null || !MainInput.IsInit())
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			num2 = UnityEngine.Input.GetAxis("Vertical");
			num = UnityEngine.Input.GetAxis("Horizontal");
			Vector2 vector = MainInput.CalculatePosition();
			if (MainInput.IsTouching())
			{
				if (vector.magnitude > 0.2f)
				{
					vector.Normalize();
				}
				else
				{
					vector = Vector2.zero;
				}
				num = vector.x;
				num2 = vector.y;
			}
			movement.playerInputX = num;
			movement.playerInputY = num2;
			movement.jumpPressed = (jumpButton() || UnityEngine.Input.GetKey(KeyCode.Space));
			movement.fallPressed = (fallButton() || UnityEngine.Input.GetKey(KeyCode.LeftControl));
			movement.UpdateMotorParameters();
		}
	}
}
