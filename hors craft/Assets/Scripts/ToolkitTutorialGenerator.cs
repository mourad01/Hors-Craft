// DecompilerFi decompiler from Assembly-CSharp.dll class: ToolkitTutorialGenerator
using com.ootii.Cameras;
using Common.Managers;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

public class ToolkitTutorialGenerator : MonoBehaviour
{
	private GenericTutorial genericTutorial;

	private const string TUTORIAL_KEY = "tutorial.toolkit.finished";

	[CompilerGenerated]
	private static Action<GenericTutorial.TutorialStep, GameObject> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<GenericTutorial.TutorialStep, GameObject> _003C_003Ef__mg_0024cache1;

	private void Awake()
	{
		if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TOOLKIT_ENABLED) && !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.DEV_ENABLED) && !PlayerPrefs.HasKey("tutorial.toolkit.finished"))
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
		UseToolkitStep();
		genericTutorial.AddDelayStep(0.2f);
		HoldToChangeBlockStep();
		genericTutorial.AddDelayStep(0.2f);
		ReturnToGameplayStep();
		genericTutorial.AddDelayStep(0.2f);
		TutorialDoneStep();
		genericTutorial.StartTutorial();
	}

	private void UseToolkitStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.infoKey = "toolkit.tutorial.text.use.toolkit";
		tutorialStep.infoDefaultText = "Use toolkit to swap blocks quickly.";
		tutorialStep.AddElementToStepOnSpawn(GetToolkitChild);
		tutorialStep.AddStepToTutorial(genericTutorial);
	}

	private void HoldToChangeBlockStep()
	{
		GenericTutorial.TutorialStep tutorialStep = GenericTutorialStepHelper.GenerateNormalStep();
		tutorialStep.infoKey = "toolkit.tutorial.text.hold.block";
		tutorialStep.infoDefaultText = "Hold toolkit's slot to change the block!";
		tutorialStep.AddElementToStepOnSpawn(GetToolkitFirstItem, GenericTutorialStepHelper.AddElementButtonCallback);
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				StartCoroutine(WaitForHold());
			}
		});
		tutorialStep.AddStepToTutorial(genericTutorial);
	}

	private IEnumerator WaitForHold()
	{
		genericTutorial.autoChangeSteps = false;
		genericTutorial.blockInputBetweenSteps = true;
		genericTutorial.allowInputThroughMask = true;
		while (!Manager.Get<StateMachineManager>().IsCurrentStateA<BlocksPopupState>())
		{
			yield return null;
		}
		genericTutorial.autoChangeSteps = true;
		genericTutorial.allowInputThroughMask = false;
		genericTutorial.NextStep();
	}

	private void ReturnToGameplayStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = null;
		tutorialStep.infoKey = "toolkit.tutorial.text.return.to.game";
		tutorialStep.infoDefaultText = "You're ready to play! Return to game.";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.AddElementToStepOnSpawn(GetBlocksReturnButton, GenericTutorialStepHelper.AddElementButtonCallback);
		tutorialStep.AddStepToTutorial(genericTutorial);
	}

	private void TutorialDoneStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "mcpe.steering.tutorial.text.11";
		tutorialStep.infoDefaultText = "Now you know everything! Have a nice game!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.RestoreDefaultTutorialSettingsOnInit(genericTutorial);
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
			action = delegate
			{
				EndTutorial();
			}
		});
		tutorialStep.AddStepToTutorial(genericTutorial);
	}

	private GameObject GetToolkitChild()
	{
		return GetToolkitModule().transform.GetChild(0).gameObject;
	}

	private GameObject GetToolkitModule()
	{
		ToolkitModule toolkitModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is ToolkitModule) as ToolkitModule;
		return toolkitModule.gameObject;
	}

	private GameObject GetToolkitFirstItem()
	{
		ToolkitModule component = GetToolkitModule().GetComponent<ToolkitModule>();
		return component.slotGOs.First();
	}

	private GameObject GetBlocksReturnButton()
	{
		return Manager.Get<CanvasManager>().canvas.GetComponentInChildren<BlocksPopupStateConnector>().returnButton.gameObject;
	}

	private void EndTutorial()
	{
		CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
		CameraController.instance.InputSource.IsEnabled = true;
		PlayerPrefs.SetInt("tutorial.toolkit.finished", 1);
		PlayerPrefs.Save();
		CloseTutorial();
	}

	private void CloseTutorial()
	{
		MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_TUTORIAL);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
