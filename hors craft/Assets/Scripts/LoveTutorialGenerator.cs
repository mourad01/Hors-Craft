// DecompilerFi decompiler from Assembly-CSharp.dll class: LoveTutorialGenerator
using com.ootii.Cameras;
using Common.Managers;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class LoveTutorialGenerator : MonoBehaviour
{
	private GameObject humanNPC;

	private GenericTutorial genericTutorial;

	private LoveManager loveManager;

	private StringsContainer pathsContainer;

	private void Start()
	{
		genericTutorial = GetComponent<GenericTutorial>();
		pathsContainer = GetComponent<StringsContainer>();
		loveManager = Manager.Get<LoveManager>();
		humanNPC = loveManager.lovableNPCs[0];
		GameplayPopupsController gameplayPopupsController = UnityEngine.Object.FindObjectOfType<GameplayPopupsController>();
		if (gameplayPopupsController != null)
		{
			UnityEngine.Object.Destroy(gameplayPopupsController.gameObject);
		}
		InitTutorial();
	}

	private void InitTutorial()
	{
		loveManager.isTutorialOn = true;
		genericTutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
		AddZeroStep();
		AddFirstStep();
		AddSecondStep();
		AddThirdStep();
		AddThirdAndAHalfStep();
		AddFourthStep();
		AddFifthStep();
		AddSixthStep();
		AddSeventhStep();
		AddSeventhAndAHalfStep();
		AddEighthStep();
		AddNinthStep();
		AddTenthStep();
		AddEleventhStep();
		AddEleventhAndAHalfStep();
		AddTwelfthStep();
		AddThirteenthStep();
		genericTutorial.StartTutorial();
	}

	private void AddZeroStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_SPAWN,
			action = delegate
			{
				genericTutorial.autoChangeSteps = false;
				genericTutorial.NextStep(2.5f);
			}
		});
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject p)
		{
			p.SetActive(value: false);
		};
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddFirstStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.1";
		tutorialStep.infoDefaultText = "Being single and partying have bored you. This is time for serious relationship.";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				genericTutorial.autoChangeSteps = true;
				SpawnHuman();
				CameraController.instance.InputSource.IsEnabled = false;
				CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: false);
				CameraController.instance.Anchor.LookAt(humanNPC.transform);
				CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
				((LookAtTargetMotor)CameraController.instance.ActiveMotor).Target = humanNPC.transform;
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private void SpawnHuman()
	{
		humanNPC = UnityEngine.Object.Instantiate(humanNPC);
		Transform transform = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<PlayerMovement>().transform;
		humanNPC.transform.position = transform.position + transform.forward * 2.5f;
		humanNPC.GetComponent<HumanMob>().logic = AnimalMob.AnimalLogic.STAY_IN_PLACE;
		Vector3 eulerAngles = Quaternion.LookRotation(transform.position - humanNPC.transform.position, Vector3.up).eulerAngles;
		eulerAngles.x = 0f;
		humanNPC.transform.rotation = Quaternion.Euler(eulerAngles);
		if (loveManager.skinConfig != null)
		{
			humanNPC.GetComponentInChildren<PlayerGraphic>().SetCustomSkin(loveManager.skinConfig.skinInfo);
		}
		else
		{
			Skin.Gender gender = AssumePlayerGender();
			humanNPC.GetComponent<HumanMob>().SetSkin((gender == Skin.Gender.MALE) ? Skin.Gender.FEMALE : Skin.Gender.MALE);
		}
		humanNPC.GetComponentInParent<Rigidbody>().useGravity = false;
	}

	private Skin.Gender AssumePlayerGender()
	{
		PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
		return controlledPlayerInstance.gender;
	}

	private void AddSecondStep()
	{
		GenericTutorial.TutorialStep step = new GenericTutorial.TutorialStep();
		step.infoKey = "tutorial.text.2";
		step.infoDefaultText = "This person is very attractive.";
		step.shape = GenericTutorial.Shape.CUSTOM;
		step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				step.element = humanNPC;
			}
		});
		AddTutorialStepToArray(step);
	}

	private void AddThirdStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.3";
		tutorialStep.infoDefaultText = "Would you like to flirt with him?";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetGameplayLoveButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetGameplayLoveButton()
	{
		LoveButtonModule loveButtonModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is LoveButtonModule) as LoveButtonModule;
		return loveButtonModule.loveButton.gameObject;
	}

	private void AddThirdAndAHalfStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				genericTutorial.autoChangeSteps = false;
				genericTutorial.blockInputBetweenSteps = true;
				genericTutorial.allowInputThroughMask = true;
				StartCoroutine(ContinueWhenPickupFinished());
			}
		});
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject g)
		{
			g.SetActive(value: false);
		};
		AddTutorialStepToArray(tutorialStep);
	}

	private IEnumerator ContinueWhenPickupFinished()
	{
		yield return new WaitForSecondsRealtime(4f);
		while (!(Manager.Get<StateMachineManager>().currentState is GameplayState))
		{
			yield return new WaitForEndOfFrame();
		}
		genericTutorial.autoChangeSteps = true;
		genericTutorial.allowInputThroughMask = false;
		genericTutorial.NextStep(1f);
	}

	private void AddFourthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.4";
		tutorialStep.infoDefaultText = "He likes you! Give him a gift maybe...";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.delayAfter = 0.2f;
		AddElementToClickOnSpawn(tutorialStep, GetGameplayGiftButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetGameplayGiftButton()
	{
		LoveButtonModule loveButtonModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is LoveButtonModule) as LoveButtonModule;
		return loveButtonModule.giftButton.gameObject;
	}

	private void AddFifthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.5";
		tutorialStep.infoDefaultText = "These balloons look cute!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetFirstGiftItem, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetFirstGiftItem()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("crafting_small_element");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddSixthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.6";
		tutorialStep.infoDefaultText = "Click on Craft button!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetCraftButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetCraftButton()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("craft_button");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddSeventhStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.7";
		tutorialStep.infoDefaultText = "Click on Use button to give a gift.";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetUseButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetUseButton()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("craft_use_button");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddSeventhAndAHalfStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				genericTutorial.autoChangeSteps = false;
				genericTutorial.blockInputBetweenSteps = true;
				StartCoroutine(ContinueWhenPickupFinished());
			}
		});
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject g)
		{
			g.SetActive(value: false);
		};
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddEighthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.8";
		tutorialStep.infoDefaultText = "Click now on Menu button.";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.delayAfter = 0.5f;
		AddElementToClickOnSpawn(tutorialStep, GetPauseButton);
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
			action = delegate
			{
				Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
				{
					disableAds = true,
					categoryToOpen = "Relationship"
				});
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetPauseButton()
	{
		PauseModule pauseModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is PauseModule) as PauseModule;
		return pauseModule.pauseButton.gameObject;
	}

	private void AddNinthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.9";
		tutorialStep.infoDefaultText = "Here is your relationship level!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.delayBefore = 0.5f;
		AddElementToClickOnSpawn(tutorialStep, GetRelationshipProgressBar);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetRelationshipProgressBar()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("slider");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddTenthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.10";
		tutorialStep.infoDefaultText = "Your partner will be delighted, if you give him his favourite gift!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetFavouritGiftIcon);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetFavouritGiftIcon()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("favourite_gift");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddEleventhStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.11";
		tutorialStep.infoDefaultText = "Go on a date with your partner!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetDateButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetDateButton()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("date_button");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddEleventhAndAHalfStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				genericTutorial.autoChangeSteps = false;
				genericTutorial.blockInputBetweenSteps = true;
				StartCoroutine(ContinueWhenPickupFinished());
			}
		});
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject g)
		{
			g.SetActive(value: false);
		};
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddTwelfthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.12";
		tutorialStep.infoDefaultText = "You look amazing together! Who knows, maybe you will be married in no time!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				genericTutorial.autoChangeSteps = true;
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddThirteenthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.13";
		tutorialStep.infoDefaultText = "Spend time with your partner. Visit various attractions and play mini games to improve your relationship level!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
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

	private GameObject GetGameObjectAtPath(string path, GameObject startPoint)
	{
		string[] array = path.Split('/');
		Transform transform = startPoint.transform;
		int num = 0;
		while (transform != null && num < array.Length)
		{
			Transform transform2 = transform.Find(array[num]);
			if (transform2 == null)
			{
				transform2 = transform.Find(array[num] + "(Clone)");
			}
			transform = transform2;
			num++;
		}
		if (transform == null)
		{
			UnityEngine.Debug.LogError("Couldn't find object " + array[num - 1] + " in path " + path);
		}
		return transform.gameObject;
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
		Manager.Get<LoveManager>().isTutorialOn = false;
		humanNPC.GetComponentInParent<Rigidbody>().useGravity = true;
		CameraController.instance.SetDefaultCameraPreset();
		CameraController.instance.InputSource.IsEnabled = true;
		CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
