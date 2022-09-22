// DecompilerFi decompiler from Assembly-CSharp.dll class: BlueprintTutorial
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using States;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class BlueprintTutorial : MonoBehaviour
{
	private GenericTutorial genericTutorial;

	private BlueprintController blueprintController;

	private BlueprintManager blueprintManager;

	private const int blocksToFill = 3;

	private PlayerMovement.Mode beforeMode;

	public void StartTutorial()
	{
		genericTutorial = GetComponent<GenericTutorial>();
		blueprintController = PlayerGraphic.GetControlledPlayerInstance().GetComponent<BlueprintController>();
		blueprintManager = Manager.Get<BlueprintManager>();
		blueprintController.inBlueprintTutorial = true;
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_TUTORIAL);
		SetUpTutorial();
		genericTutorial.StartTutorial();
		PlayerPrefs.SetInt("blueprint.tutorial.showed", 1);
	}

	private IEnumerator trySetTimeScale()
	{
		TimeScaleHelper.value = 0f;
		yield return new WaitForSecondsRealtime(0.5f);
		while (TimeScaleHelper.value == 1f)
		{
			TimeScaleHelper.value = 0f;
			yield return new WaitForSecondsRealtime(0.5f);
		}
	}

	private void SetUpTutorial()
	{
		genericTutorial.tutorialSteps = new GenericTutorial.TutorialStep[6];
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "blueprint.tutorial.0";
		tutorialStep.infoDefaultText = "To Fill blueprint you have to tap blocks";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
		{
			new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_INIT,
				action = delegate
				{
					StartCoroutine(trySetTimeScale());
					genericTutorial.autoChangeSteps = true;
					PlayerMovement component = PlayerGraphic.GetControlledPlayerInstance().GetComponent<PlayerMovement>();
					if (component.IsMounted())
					{
						component.ForceUnmount();
					}
					beforeMode = component.GetMode();
					component.ChangeMoveMode(PlayerMovement.Mode.FLYING);
					component.EnableMovement(enable: false);
					CameraController.instance.InputSource.IsEnabled = false;
				}
			}
		};
		GenericTutorial.TutorialStep tutorialStep2 = tutorialStep;
		tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = blueprintController.highlightBlock;
		tutorialStep.infoKey = "blueprint.tutorial.1";
		tutorialStep.infoDefaultText = "Tap Here!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
		{
			new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_INIT,
				action = delegate
				{
					blueprintController.ActivateHighlightBlock();
					CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
					((LookAtTargetMotor)CameraController.instance.ActiveMotor).UseCurrentPosition = false;
					((LookAtTargetMotor)CameraController.instance.ActiveMotor).UseRigAnchor = true;
					((LookAtTargetMotor)CameraController.instance.ActiveMotor).Target = blueprintController.highlightBlock.transform;
					genericTutorial.tutorialSteps[genericTutorial.currentStep].element = blueprintController.highlightBlock;
				}
			},
			new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
				action = delegate
				{
					blueprintManager.FillVoxelInPosition(blueprintController.highlightBlock.transform.position);
				}
			}
		};
		GenericTutorial.TutorialStep tutorialStep3 = tutorialStep;
		tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModule<BlueprintsModule>()
			.fillBlueprintButton.gameObject;
			tutorialStep.infoKey = "blueprint.tutorial.2";
			tutorialStep.infoDefaultText = "You can also tap this button to fill blueprint instantly";
			tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
			GenericTutorial.TutorialStep tutorialStep4 = tutorialStep;
			tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
			tutorialStep.infoKey = "blueprint.tutorial.3";
			tutorialStep.infoDefaultText = "Now you can finish filling blueprint by yourself";
			tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
			tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>
			{
				new GenericTutorial.TutorialCallback
				{
					type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
					action = delegate
					{
						TimeScaleHelper.value = 1f;
						CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
						CameraController.instance.InputSource.IsEnabled = true;
						CameraController.instance.Anchor.GetComponent<PlayerMovement>().ChangeMoveMode(beforeMode);
						CameraController.instance.SetDefaultCameraPreset();
						MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_TUTORIAL);
						blueprintController.inBlueprintTutorial = false;
					}
				}
			};
			GenericTutorial.TutorialStep tutorialStep5 = tutorialStep;
			genericTutorial.tutorialSteps[0] = tutorialStep2;
			for (int i = 1; i <= 3; i++)
			{
				genericTutorial.tutorialSteps[i] = tutorialStep3;
			}
			genericTutorial.tutorialSteps[genericTutorial.tutorialSteps.Length - 2] = tutorialStep4;
			genericTutorial.tutorialSteps[genericTutorial.tutorialSteps.Length - 1] = tutorialStep5;
		}
	}
