// DecompilerFi decompiler from Assembly-CSharp.dll class: McpeMovementTutorialGenerator
using com.ootii.Cameras;
using Common.Managers;
using GameUI;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class McpeMovementTutorialGenerator : TutorialGenerator
{
	public McpeSteering mcpeContext => MonoBehaviourSingleton<McpeSteering>.get;

	public override void InitTutorial()
	{
		genericTutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
		PlayerMovement component = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
		component.ChangeMoveMode(PlayerMovement.Mode.WALKING);
		PlayerPrefs.DeleteKey("lastOpenedRarityBlocksCategory");
		PlayerPrefs.DeleteKey("lastOpenedBlocksCategory");
		PlayerPrefs.Save();
		DisableAll();
		genericTutorial.AddDelayStep(2.5f);
		AddMovementStep();
		AddTutortialDoneStep();
		genericTutorial.StartTutorial();
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
				StartCoroutine(ContinueAfterReachedDistance(4f));
			}
		});
		genericTutorial.AddTutorialStep(tutorialStep);
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
		genericTutorial.AddTutorialStep(tutorialStep);
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

	public override void EndTutorial()
	{
		base.EndTutorial();
		CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
		CameraController.instance.InputSource.IsEnabled = true;
		mcpeContext.skipAdding = false;
		mcpeContext.skipDigging = false;
		GetGameplayPauseModule().gameObject.SetActive(value: true);
		GetGameplayBlockPopupButton().gameObject.SetActive(value: true);
		GetGameplayMovementAnalog().gameObject.SetActive(value: true);
		GetGameplayTouchInputAreaModule().gameObject.SetActive(value: true);
		ActivateHiddenGameplayModules();
		CloseTutorial();
	}

	public void DisableAll()
	{
		CameraController.instance.InputSource.IsEnabled = false;
		mcpeContext.skipAdding = true;
		mcpeContext.skipDigging = true;
		GetGameplayPauseModule().gameObject.SetActive(value: false);
		GetGameplayBlockPopupButton().gameObject.SetActive(value: false);
		GetGameplayMovementAnalog().gameObject.SetActive(value: false);
		GetGameplayTouchInputAreaModule().gameObject.SetActive(value: false);
		GetAndHideGameplayModules();
	}
}
