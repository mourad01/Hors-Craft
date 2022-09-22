// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TouchInputAreaModule
using Common.Managers;
using GameUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace States
{
	public class TouchInputAreaModule : GameplayModule, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		private enum InputState
		{
			None,
			Started,
			Moving,
			Digging
		}

		private SimpleRepeatButton rotateButton;

		private InputState inputState;

		private float holdTime;

		private float holdStartTime;

		private Coroutine diggingCoroutine;

		private BlueprintFillContext blueprintContext;

		private float movedDistance;

		private GameplayState _gameplayInstance;

		private McpeSteering context => MonoBehaviourSingleton<McpeSteering>.get;

		private GameplayState gameplayInstance
		{
			get
			{
				if (_gameplayInstance == null)
				{
					_gameplayInstance = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>();
				}
				return _gameplayInstance;
			}
		}

		protected override Fact[] listenedFacts => new Fact[3]
		{
			Fact.MOVEMENT,
			Fact.MCPE_STEERING,
			Fact.IN_BLUEPRINT_RANGE
		};

		private void Awake()
		{
			rotateButton = GetComponent<SimpleRepeatButton>();
		}

		private void OnEnable()
		{
			Reset();
		}

		private void OnDisable()
		{
			Reset();
		}

		private void Reset()
		{
			if (!(context == null))
			{
				context.isPointerDown = false;
				context.isMoving = false;
				context.pointerID = null;
				inputState = InputState.None;
				movedDistance = 0f;
			}
		}

		protected override void Update()
		{
			base.Update();
			if (context.isPointerDown)
			{
				movedDistance += CalculateMoveDelta();
				holdTime = Time.unscaledTime - holdStartTime;
				if (holdTime < context.digAfterTime && movedDistance > context.dragThreshold)
				{
					inputState = InputState.Moving;
					context.isMoving = true;
				}
			}
		}

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			bool flag = MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING);
			if (base.gameObject.activeSelf != flag)
			{
				base.gameObject.SetActive(flag);
			}
			if (flag)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<CameraRotationContext>(Fact.MOVEMENT).FirstOrDefault()?.setCameraRotationButton(rotateButton);
				blueprintContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<BlueprintFillContext>(Fact.IN_BLUEPRINT_RANGE).FirstOrDefault();
				if (blueprintContext != null)
				{
					blueprintContext.setFillVoxelButton(rotateButton);
				}
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (gameplayInstance.currentSubstate.substate == GameplayState.Substates.WALKING && inputState == InputState.None)
			{
				movedDistance = 0f;
				context.isPointerDown = true;
				context.pointerID = eventData.pointerId;
				holdStartTime = Time.unscaledTime;
				inputState = InputState.Started;
				StartCoroutine(TapAndHoldCheckCoroutine());
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (gameplayInstance.currentSubstate.substate == GameplayState.Substates.WALKING && (!context.pointerID.HasValue || eventData.pointerId == context.pointerID.Value))
			{
				if (diggingCoroutine != null)
				{
					StopCoroutine(diggingCoroutine);
					diggingCoroutine = null;
				}
				context.isPointerDown = false;
				context.pointerID = null;
				holdTime = Time.unscaledTime - holdStartTime;
				if (holdTime < context.tapBeforeTime && inputState == InputState.Started)
				{
					AddVoxel();
				}
				inputState = InputState.None;
				context.isMoving = false;
			}
		}

		private float CalculateMoveDelta()
		{
			Vector2 vector = Vector3.zero;
			if (context.pointerID.HasValue)
			{
				int value = context.pointerID.Value;
				vector = ((value >= 0 && UnityEngine.Input.touchCount > value) ? UnityEngine.Input.GetTouch(value).deltaPosition : new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")));
			}
			return (new Vector2(vector.x / (float)Screen.width, vector.y / (float)Screen.height) * 2f * 100f).magnitude;
		}

		private void AddVoxel()
		{
			if (blueprintContext != null && blueprintContext.placedBlueprintVoxel)
			{
				blueprintContext.placedBlueprintVoxel = false;
			}
			else if (context.passedHitTest && !context.skipAdding && context.OnAdd != null)
			{
				context.OnAdd();
			}
		}

		private void RemoveVoxel()
		{
			if (context.blueprintVoxelDirectHit != null && context.blueprintVoxelDirectHit.GetVoxel() == Engine.usefulIDs.blueprintID)
			{
				if (diggingCoroutine != null)
				{
					StopCoroutine(diggingCoroutine);
					diggingCoroutine = null;
				}
				context.isPointerDown = false;
				context.pointerID = null;
				holdTime = Time.unscaledTime - holdStartTime;
				inputState = InputState.None;
				context.isMoving = false;
				PlayerGraphic.GetControlledPlayerInstance().GetComponent<BlueprintController>().DeleteBlueprint(Engine.VoxelInfoToPosition(context.blueprintVoxelDirectHit));
			}
			else if (context.passedHitTest && !context.skipDigging && context.OnDig != null)
			{
				context.OnDig();
			}
		}

		private void TryDigging()
		{
			if (diggingCoroutine == null && inputState == InputState.Digging)
			{
				diggingCoroutine = StartCoroutine(DiggingCoroutine());
			}
		}

		private IEnumerator DiggingCoroutine()
		{
			RemoveVoxel();
			if (context.OnPingIndicator != null)
			{
				context.OnPingIndicator(context.digNextVoxelTimeInterval);
			}
			yield return new WaitForSecondsRealtime(context.digNextVoxelTimeInterval);
			diggingCoroutine = null;
			TryDigging();
		}

		private IEnumerator TapAndHoldCheckCoroutine()
		{
			if (context.OnPingIndicator != null)
			{
				context.OnPingIndicator(context.digAfterTime);
			}
			yield return new WaitForSecondsRealtime(context.digAfterTime + 0.05f);
			holdTime = Time.unscaledTime - holdStartTime;
			if (holdTime > context.digAfterTime && inputState == InputState.Started)
			{
				if (diggingCoroutine != null)
				{
					StopCoroutine(diggingCoroutine);
					diggingCoroutine = null;
				}
				inputState = InputState.Digging;
				TryDigging();
			}
		}
	}
}
