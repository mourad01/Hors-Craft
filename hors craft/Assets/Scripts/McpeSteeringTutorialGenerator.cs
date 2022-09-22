// DecompilerFi decompiler from Assembly-CSharp.dll class: McpeSteeringTutorialGenerator
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using GameUI;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class McpeSteeringTutorialGenerator : MonoBehaviour
{
	public GameObject highlightBlockPrefab;

	public GameObject toolkitTutorial;

	private GenericTutorial genericTutorial;

	private const string TUTORIAL_KEY = "tutorial.mcpe.steering.finished";

	private HashSet<GameObject> GameplayModulesToHide = new HashSet<GameObject>();

	private McpeSteering context => MonoBehaviourSingleton<McpeSteering>.get;

	private void Start()
	{
		McpeSteeringModule mcpeSteering = Manager.Get<ModelManager>().mcpeSteering;
		if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING) && !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.DEV_ENABLED) && !PlayerPrefs.HasKey("tutorial.mcpe.steering.finished") && mcpeSteering.GetEnabled() && mcpeSteering.GetTutorialEnabled())
		{
			genericTutorial = GetComponent<GenericTutorial>();
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_TUTORIAL);
			InitTutorial();
		}
		else
		{
			CloseTutorial();
		}
	}

	private void InitTutorial()
	{
		genericTutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
		Transform transform = PlayerGraphic.GetControlledPlayerInstance().transform;
		if (Vector3.Distance(transform.position.SetPositionY(0f), Engine.EngineInstance.startPlayerPosition.SetPositionY(0f)) < 60f)
		{
			transform.position = Engine.EngineInstance.startPlayerPosition;
		}
		else
		{
			transform.position += Vector3.up * 20f;
		}
		Transform transform2 = transform;
		Vector3 eulerAngles = transform.eulerAngles;
		float x = eulerAngles.x;
		float startPlayerYRotation = Engine.EngineInstance.startPlayerYRotation;
		Vector3 eulerAngles2 = transform.eulerAngles;
		transform2.eulerAngles = new Vector3(x, startPlayerYRotation, eulerAngles2.z);
		PlayerMovement component = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
		component.ChangeMoveMode(PlayerMovement.Mode.WALKING);
		PlayerPrefs.DeleteKey("lastOpenedRarityBlocksCategory");
		PlayerPrefs.DeleteKey("lastOpenedBlocksCategory");
		PlayerPrefs.Save();
		DisableAll();
		AddDelayStep(2.5f);
		AddMovementStep();
		AddDelayStep(0.2f);
		if (Manager.Get<ModelManager>().mcpeSteering.GetFlyInCameraDirection())
		{
			AddJumpStep();
			AddDelayStep(0.2f);
		}
		AddPutBlockTapHereStep();
		AddDelayStep(0.2f);
		AddChangeBlockStep();
		AddDelayStep(0.2f);
		AddChangeBlockChooseStep();
		AddDelayStep(0.2f);
		AddRemoveBlockInfoStep();
		AddDelayStep(0.2f);
		AddRemoveBlockTapAndHoldStep();
		AddDelayStep(0.2f);
		if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TOOLKIT_ENABLED))
		{
			AddTutortialDoneStep();
		}
		else
		{
			AddSwapToToolkitTutorialStep();
		}
		genericTutorial.StartTutorial();
	}

	private void AddDelayStep(float delay)
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
			action = delegate
			{
				genericTutorial.autoChangeSteps = false;
				genericTutorial.NextStep(delay);
			}
		});
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject p)
		{
			p.SetActive(value: false);
		};
		AddTutorialStepToArray(tutorialStep);
	}

	private IEnumerator ContinueAfterReachedDistance(float distanceToReach)
	{
		genericTutorial.autoChangeSteps = false;
		genericTutorial.blockInputBetweenSteps = true;
		genericTutorial.allowInputThroughMask = true;
		PlayerMovement playerMovement = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
		while (!playerMovement.IsGrounded() && !playerMovement.underwater)
		{
			yield return new WaitForEndOfFrame();
		}
		GameObject analog = GetGameplayMovementAnalog();
		analog.gameObject.SetActive(value: true);
		Vector3 previousPosition = CameraController.instance.Anchor.position;
		float distance = 0f;
		while (distance < distanceToReach)
		{
			Vector3 currentPostion = CameraController.instance.Anchor.position;
			distance += (currentPostion - previousPosition).magnitude;
			previousPosition = currentPostion;
			yield return new WaitForEndOfFrame();
		}
		analog.gameObject.SetActive(value: false);
		playerMovement.EnableMovement(enable: false);
		genericTutorial.autoChangeSteps = true;
		genericTutorial.allowInputThroughMask = false;
		genericTutorial.NextStep();
	}

	private void AddMovementStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "mcpe.steering.tutorial.text.1";
		tutorialStep.infoDefaultText = "Hi, welcome to the Theme Park craft! To move, use these buttons.";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.element = GetGameplayMovementAnalog().GetComponentInChildren<AnalogController>().gameObject;
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				PlayerPrefs.SetInt("tutorial.mcpe.steering.finished", 1);
				StartCoroutine(ContinueAfterReachedDistance(4f));
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddJumpStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "mcpe.steering.tutorial.text.jump";
		tutorialStep.infoDefaultText = "Tap on the analog controller to jump!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.element = GetGameplayMovementAnalog().GetComponentInChildren<AnalogController>().gameObject;
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				StartCoroutine(ContinueAfterJump());
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private IEnumerator ContinueAfterJump()
	{
		genericTutorial.autoChangeSteps = false;
		genericTutorial.blockInputBetweenSteps = true;
		genericTutorial.allowInputThroughMask = true;
		PlayerMovement playerMovement = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
		while (!playerMovement.IsGrounded() && !playerMovement.underwater)
		{
			yield return new WaitForEndOfFrame();
		}
		GameObject analog = GetGameplayMovementAnalog();
		analog.gameObject.SetActive(value: true);
		while (!playerMovement.jumpPressed)
		{
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSecondsRealtime(0.5f);
		while (!playerMovement.IsGrounded() && !playerMovement.underwater)
		{
			yield return new WaitForEndOfFrame();
		}
		analog.gameObject.SetActive(value: false);
		playerMovement.EnableMovement(enable: false);
		genericTutorial.autoChangeSteps = true;
		genericTutorial.allowInputThroughMask = false;
		genericTutorial.NextStep();
	}

	private IEnumerator ContinueAfterPlaceOrRemoveBlock(GenericTutorial.TutorialStep step, bool isRemove = false)
	{
		genericTutorial.autoChangeSteps = false;
		genericTutorial.blockInputBetweenSteps = true;
		genericTutorial.allowInputThroughMask = true;
		GetGameplayTouchInputAreaModule().gameObject.SetActive(value: true);
		if (Manager.Get<ModelManager>().blocksUnlocking.IsBlocksLimitedEnabled())
		{
			Singleton<BlocksController>.get.IncrementBlockCount(Engine.GetVoxelType(ExampleInventory.HeldBlock));
		}
		bool removed = false;
		bool added = false;
		VoxelHoverAction voxelHoverAction = CameraController.instance.Anchor.GetComponent<CameraEventsSender>().GetHoverAction<VoxelHoverAction>();
		Vector3 zero = Vector3.zero;
		Vector3 raycastPos = (CameraController.instance.currentCameraPreset != CameraController.CameraPresets.TPP_Fixed) ? (CameraController.instance.Anchor.position + CameraController.instance.Anchor.up * 8f + CameraController.instance.Anchor.forward * 3f) : (CameraController.instance.Anchor.position + CameraController.instance.Anchor.up * 8f + CameraController.instance.Anchor.forward * 2f + CameraController.instance.Anchor.right * 2f);
		VoxelInfo hitInfo = Engine.VoxelGridRaycastIgnoreWater(raycastPos, Vector3.down, 20f);
		if (hitInfo != null)
		{
			Vector3 hitPosition = isRemove ? voxelHoverAction.lastHitPosition : Engine.VoxelInfoToPosition(hitInfo);
			voxelHoverAction.SetCustomAddAction(delegate
			{
				voxelHoverAction.SetCustomAddAction(null);
				voxelHoverAction.OnAdd();
				added = true;
			});
			voxelHoverAction.SetCustomDeleteAction(delegate
			{
				voxelHoverAction.SetCustomDeleteAction(null);
				voxelHoverAction.OnDig();
				removed = true;
			});
			GameObject highlightBlock = UnityEngine.Object.Instantiate(highlightBlockPrefab);
			highlightBlock.transform.position = hitPosition + Vector3.up;
			highlightBlock.transform.GetChild(0).gameObject.SetActive(value: false);
			step.element = highlightBlock;
			CameraController.CameraPresets prevPreset = CameraController.instance.currentCameraPreset;
			CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
			LookAtTargetMotor lookAt = (LookAtTargetMotor)CameraController.instance.ActiveMotor;
			Vector3 offset = lookAt.TargetOffset;
			lookAt.TargetOffset = new Vector3(0f, 0.7f, 0f);
			lookAt.Target = highlightBlock.transform;
			float raycastDistance = context.interactionDistance;
			if (isRemove)
			{
				context.skipAdding = true;
				context.skipDigging = false;
				context.interactionDistance = 25f;
				while (!removed)
				{
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				context.skipAdding = false;
				context.skipDigging = true;
				context.interactionDistance = 25f;
				while (!added)
				{
					yield return new WaitForEndOfFrame();
				}
			}
			context.interactionDistance = raycastDistance;
			context.skipAdding = true;
			context.skipDigging = true;
			if (highlightBlock != null)
			{
				UnityEngine.Object.Destroy(highlightBlock);
			}
			lookAt.TargetOffset = offset;
			CameraController.instance.SetCameraPreset(prevPreset);
		}
		if (voxelHoverAction != null)
		{
			voxelHoverAction.SetCustomAddAction(null);
			voxelHoverAction.SetCustomDeleteAction(null);
		}
		GetGameplayTouchInputAreaModule().gameObject.SetActive(value: false);
		genericTutorial.autoChangeSteps = true;
		genericTutorial.allowInputThroughMask = false;
		genericTutorial.NextStep();
	}

	private void AddPutBlockTapHereStep()
	{
		GenericTutorial.TutorialStep step = new GenericTutorial.TutorialStep();
		step.element = Manager.Get<CanvasManager>().canvas.gameObject;
		step.infoKey = "mcpe.steering.tutorial.text.3";
		step.infoDefaultText = "Tap here to put a block.";
		step.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		step.forceShowArrow = true;
		step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				StartCoroutine(ContinueAfterPlaceOrRemoveBlock(step));
			}
		});
		AddTutorialStepToArray(step);
	}

	private void AddChangeBlockStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.infoKey = "mcpe.steering.tutorial.text.4";
		tutorialStep.infoDefaultText = "Maybe you want to change the block?";
		AddElementToClickOnSpawn(tutorialStep, GetGameplayBlockPopupButton, AddDefaultButtonCallbacks);
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				GetGameplayBlockPopupButton().gameObject.SetActive(value: true);
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddChangeBlockChooseStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "mcpe.steering.tutorial.text.5";
		tutorialStep.infoDefaultText = "This block looks cool, choose it.";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetBlocksPopupBlockToChooseButton, AddDefaultButtonCallbacks);
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				GetGameplayBlockPopupButton().gameObject.SetActive(value: false);
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddRemoveBlockInfoStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "mcpe.steering.tutorial.text.6";
		tutorialStep.infoDefaultText = "You already know how to put blocks! But what about removing them?";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks.Add(TurnOnAutoChangeStepsCallback());
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddRemoveBlockTapAndHoldStep()
	{
		GenericTutorial.TutorialStep step = new GenericTutorial.TutorialStep();
		step.element = Manager.Get<CanvasManager>().canvas.gameObject;
		step.infoKey = "mcpe.steering.tutorial.text.7";
		step.infoDefaultText = "Tap and hold to remove the block.";
		step.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		step.forceShowArrow = true;
		step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				StartCoroutine(ContinueAfterPlaceOrRemoveBlock(step, isRemove: true));
			}
		});
		AddTutorialStepToArray(step);
	}

	private void AddTutortialDoneStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "mcpe.steering.tutorial.text.11";
		tutorialStep.infoDefaultText = "Now you know everything! Have a nice game!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(TurnOnAutoChangeStepsCallback());
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
			action = delegate
			{
				EndTutorial();
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddSwapToToolkitTutorialStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(TurnOnAutoChangeStepsCallback());
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				genericTutorial.EndTutorial();
				EndTutorial();
				UnityEngine.Object.Instantiate(toolkitTutorial);
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private void GetAndHideGameplayModules()
	{
		foreach (GameplayModule module in Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules())
		{
			if (module.gameObject.activeSelf && !(module is MovementAnalogModule) && !(module is TouchIndicatorModule) && !(module is TouchInputAreaModule) && !(module is PauseModule) && !(module is BlocksPopupButtonModule))
			{
				GameplayModulesToHide.Add(module.gameObject);
				module.gameObject.SetActive(value: false);
			}
		}
	}

	private void ActivateHiddenGameplayModules()
	{
		foreach (GameObject item in GameplayModulesToHide)
		{
			item.gameObject.SetActive(value: true);
		}
	}

	private GameObject GetGameplayMovementAnalog()
	{
		MovementAnalogModule movementAnalogModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is MovementAnalogModule) as MovementAnalogModule;
		if (movementAnalogModule == null || !movementAnalogModule.IsEnabled())
		{
			return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
				.FirstOrDefault((GameplayModule m) => m is AnalogModule)
				.gameObject;
			}
			return movementAnalogModule.analog.gameObject;
		}

		private TouchInputAreaModule GetGameplayTouchInputAreaModule()
		{
			return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
				.FirstOrDefault((GameplayModule m) => m is TouchInputAreaModule) as TouchInputAreaModule;
		}

		private PauseModule GetGameplayPauseModule()
		{
			return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
				.FirstOrDefault((GameplayModule m) => m is PauseModule) as PauseModule;
		}

		private GameObject GetGameplayBlockPopupButton()
		{
			BlocksPopupButtonModule blocksPopupButtonModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
				.FirstOrDefault((GameplayModule m) => m is BlocksPopupButtonModule) as BlocksPopupButtonModule;
			return blocksPopupButtonModule.blocksButton.gameObject;
		}

		private GameObject GetBlocksPopupBlockToChooseButton()
		{
			ChooseBlockButtonController[] componentsInChildren = Manager.Get<StateMachineManager>().GetStateInstance<BlocksPopupState>().blocks.scrollListContentView.GetComponentsInChildren<ChooseBlockButtonController>();
			return componentsInChildren[2].gameObject;
		}

		private void DisableAll()
		{
			CameraController.instance.InputSource.IsEnabled = false;
			context.skipAdding = true;
			context.skipDigging = true;
			GetGameplayPauseModule().gameObject.SetActive(value: false);
			GetGameplayBlockPopupButton().gameObject.SetActive(value: false);
			GetGameplayMovementAnalog().gameObject.SetActive(value: false);
			GetGameplayTouchInputAreaModule().gameObject.SetActive(value: false);
			GetAndHideGameplayModules();
		}

		private GenericTutorial.TutorialCallback TurnOnAutoChangeStepsCallback()
		{
			GenericTutorial.TutorialCallback tutorialCallback = new GenericTutorial.TutorialCallback();
			tutorialCallback.type = GenericTutorial.CallbackType.ON_STEP_INIT;
			tutorialCallback.action = delegate
			{
				genericTutorial.autoChangeSteps = true;
			};
			return tutorialCallback;
		}

		private void AddTutorialStepToArray(GenericTutorial.TutorialStep step)
		{
			GenericTutorial.TutorialStep[] array = new GenericTutorial.TutorialStep[genericTutorial.tutorialSteps.Length + 1];
			Array.Copy(genericTutorial.tutorialSteps, array, array.Length - 1);
			array[array.Length - 1] = step;
			genericTutorial.tutorialSteps = array;
		}

		private void AddElementToClickOnSpawn(GenericTutorial.TutorialStep step, Func<GameObject> getGO, Action<GenericTutorial.TutorialStep, GameObject> addOtherInitialCallbacks = null)
		{
			if (step.tutorialCallbacks == null)
			{
				step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
			}
			step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_INIT,
				action = delegate
				{
					step.element = getGO();
					genericTutorial.autoChangeSteps = true;
					if (addOtherInitialCallbacks != null)
					{
						addOtherInitialCallbacks(step, step.element);
					}
				}
			});
		}

		private void AddDefaultButtonCallbacks(GenericTutorial.TutorialStep step, GameObject buttonGO)
		{
			if (step.tutorialCallbacks == null)
			{
				step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
			}
			step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
				action = delegate
				{
					buttonGO.GetComponentInChildren<Button>().onClick.Invoke();
				}
			});
		}

		private void EndTutorial()
		{
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
			CameraController.instance.InputSource.IsEnabled = true;
			context.skipAdding = false;
			context.skipDigging = false;
			GetGameplayPauseModule().gameObject.SetActive(value: true);
			GetGameplayBlockPopupButton().gameObject.SetActive(value: true);
			GetGameplayMovementAnalog().gameObject.SetActive(value: true);
			GetGameplayTouchInputAreaModule().gameObject.SetActive(value: true);
			ActivateHiddenGameplayModules();
			PlayerPrefs.SetInt("tutorial.mcpe.steering.finished", 1);
			PlayerPrefs.Save();
			CloseTutorial();
		}

		private void CloseTutorial()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_TUTORIAL);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
