// DecompilerFi decompiler from Assembly-CSharp.dll class: FlowerTutorialGenerator
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class FlowerTutorialGenerator : MonoBehaviour
{
	public GameObject flowerMockPrefab;

	public Vector3 flowerPosition;

	private GameObject flower;

	private GenericTutorial genericTutorial;

	private StringsContainer pathsContainer;

	private void Start()
	{
		genericTutorial = GetComponent<GenericTutorial>();
		pathsContainer = GetComponent<StringsContainer>();
		GameplayPopupsController gameplayPopupsController = UnityEngine.Object.FindObjectOfType<GameplayPopupsController>();
		if (gameplayPopupsController != null)
		{
			UnityEngine.Object.Destroy(gameplayPopupsController.gameObject);
		}
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_TUTORIAL);
		InitTutorial();
	}

	private void InitTutorial()
	{
		genericTutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
		AddFirstStep();
		AddSecondStep();
		AddSecondAndAHalfStep();
		AddThirdStep();
		AddFourthStep();
		AddFifthStep();
		AddSixthStep();
		AddSeventhStep();
		AddSeventhAndAHalfStep();
		AddEighthStep();
		AddNinthStep();
		AddTenthStep();
		AddEleventhStep();
		AddTwelfthStep();
		AddTwelfthAndAHalfStep();
		genericTutorial.StartTutorial();
	}

	private void AddFirstStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.1";
		tutorialStep.infoDefaultText = "Welcome young gardener! Good to see you, we need your help!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				SpawnFlower();
				CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: false);
				CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
				((LookAtTargetMotor)CameraController.instance.ActiveMotor).Target = flower.transform;
				CameraController.instance.InputSource.IsEnabled = false;
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private void SpawnFlower()
	{
		flower = UnityEngine.Object.Instantiate(flowerMockPrefab);
		flower.transform.position = flowerPosition;
	}

	private void AddSecondStep()
	{
		GenericTutorial.TutorialStep step = new GenericTutorial.TutorialStep();
		step.infoKey = "tutorial.text.2";
		step.infoDefaultText = "These flowers look ready to be picked!";
		step.shape = GenericTutorial.Shape.CUSTOM;
		step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				step.element = flower;
			}
		});
		AddTutorialStepToArray(step);
	}

	private void AddSecondAndAHalfStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetGameplayHarvestButton, AddDefaultButtonCallbacks);
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject g)
		{
			g.SetActive(value: false);
		};
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetGameplayHarvestButton()
	{
		FarmButtonModule farmButtonModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is FarmButtonModule) as FarmButtonModule;
		return farmButtonModule.harvestButton.gameObject;
	}

	private void AddThirdStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.3";
		tutorialStep.infoDefaultText = "Awesome! For every flower you pick, I will give you some coins!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_BRIGHT;
		tutorialStep.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_INIT,
			action = delegate
			{
				genericTutorial.autoChangeSteps = false;
				genericTutorial.blockInputBetweenSteps = true;
				genericTutorial.allowInputThroughMask = true;
				StartCoroutine(ContinueWhenPickupFinished(3f));
			}
		});
		AddTutorialStepToArray(tutorialStep);
	}

	private IEnumerator ContinueWhenPickupFinished(float waitTime = 4f)
	{
		yield return new WaitForSecondsRealtime(waitTime);
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
		tutorialStep.infoDefaultText = "Now we need to plant a new flower here!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.delayAfter = 0.2f;
		AddElementToClickOnSpawn(tutorialStep, GetGameplaySowButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetGameplaySowButton()
	{
		FarmButtonModule farmButtonModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is FarmButtonModule) as FarmButtonModule;
		return farmButtonModule.sowButton.gameObject;
	}

	private void AddFifthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.5";
		tutorialStep.infoDefaultText = "Good job, we need to wait for it to grow and blossom!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddSixthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.6";
		tutorialStep.infoDefaultText = "You can make it grow faster by watering it!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetGameplayWaterButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetGameplayWaterButton()
	{
		FarmButtonModule farmButtonModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is FarmButtonModule) as FarmButtonModule;
		return farmButtonModule.waterButton.gameObject;
	}

	private void AddSeventhStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.7";
		tutorialStep.infoDefaultText = "If you're so inpatient, you can make the time go faster! This time you can do it for free!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetGameplayFasterTimeButton, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetGameplayFasterTimeButton()
	{
		SkipTimeModule skipTimeModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is SkipTimeModule) as SkipTimeModule;
		return skipTimeModule.skipButton.gameObject;
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
				StartCoroutine(ContinueWhenPickupFinished(0.5f));
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
		tutorialStep.infoDefaultText = "Would you like to plant some other flowers?";
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
					categoryToOpen = "Flowers"
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
		tutorialStep.infoDefaultText = "Here are all the flowers you can plant, together with their prices for one bed";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.delayBefore = 0.5f;
		AddElementToClickOnSpawn(tutorialStep, GetFlowersGrid);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetFlowersGrid()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("flowers_grid");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddTenthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.10";
		tutorialStep.infoDefaultText = "Some flowers require higher prestige level!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetPrestigeLevel);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetPrestigeLevel()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("prestige_level");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddEleventhStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.infoKey = "tutorial.text.11";
		tutorialStep.infoDefaultText = "Choose this flower!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetFirstFlower, AddDefaultButtonCallbacks);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetFirstFlower()
	{
		GameObject gameObject = Manager.Get<CanvasManager>().canvas.gameObject;
		string value = pathsContainer.GetValue("flowers_grid_first_flower");
		return GetGameObjectAtPath(value, gameObject);
	}

	private void AddTwelfthStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.12";
		tutorialStep.infoDefaultText = "Plant it anywhere you want with this button!";
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		AddElementToClickOnSpawn(tutorialStep, GetGameplayAddButton);
		AddTutorialStepToArray(tutorialStep);
	}

	private GameObject GetGameplayAddButton()
	{
		WorldInteractionModule worldInteractionModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
			.FirstOrDefault((GameplayModule m) => m is WorldInteractionModule) as WorldInteractionModule;
		return worldInteractionModule.addButton.gameObject;
	}

	private void AddTwelfthAndAHalfStep()
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
				FarmingManager farmingManager = Manager.Get<FarmingManager>();
				farmingManager.onBuy = (Action)Delegate.Combine(farmingManager.onBuy, new Action(OnBuyAction));
				genericTutorial.autoChangeSteps = false;
				genericTutorial.blockInputBetweenSteps = true;
				genericTutorial.allowInputThroughMask = true;
				Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(20);
				CameraController.instance.InputSource.IsEnabled = true;
				CameraController.instance.SetDefaultCameraPreset();
				CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
			}
		});
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject g)
		{
			g.SetActive(value: false);
		};
		AddTutorialStepToArray(tutorialStep);
	}

	private void OnBuyAction()
	{
		AddThirteenStep();
		AddFourteenStep();
		genericTutorial.autoChangeSteps = true;
		genericTutorial.NextStep();
	}

	private void AddThirteenStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.13";
		tutorialStep.infoDefaultText = "Each new bed costs money!";
		tutorialStep.shape = GenericTutorial.Shape.WHOLE_SCREEN_DARK;
		AddTutorialStepToArray(tutorialStep);
	}

	private void AddFourteenStep()
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.element = Manager.Get<CanvasManager>().canvas.gameObject;
		tutorialStep.infoKey = "tutorial.text.14";
		tutorialStep.infoDefaultText = "I've taught you everything I know! Plant flowers, pick them, get money and design your dream garden!";
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
		FarmingManager farmingManager = Manager.Get<FarmingManager>();
		farmingManager.onBuy = (Action)Delegate.Remove(farmingManager.onBuy, new Action(OnBuyAction));
		MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_TUTORIAL);
		UnityEngine.Object.Destroy(flower);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
