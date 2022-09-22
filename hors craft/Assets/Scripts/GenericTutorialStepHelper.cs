// DecompilerFi decompiler from Assembly-CSharp.dll class: GenericTutorialStepHelper
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public static class GenericTutorialStepHelper
{
	[CompilerGenerated]
	private static Action<GenericTutorial.TutorialStep, GameObject> _003C_003Ef__mg_0024cache0;

	public static GenericTutorial.TutorialStep GenerateNormalStep(GameObject element = null)
	{
		GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
		tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
		tutorialStep.element = element;
		tutorialStep.delayAfter = 0f;
		tutorialStep.delayBefore = 0f;
		return tutorialStep;
	}

	public static GenericTutorial.TutorialStep GenerateBlockInputStep(GenericTutorial tutorial)
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
				tutorial.allowInputThroughMask = false;
				tutorial.autoChangeSteps = false;
				tutorial.blockInputBetweenSteps = true;
			}
		});
		tutorialStep.RemoveInfoPanel();
		return tutorialStep;
	}

	public static void RestoreDefaultTutorialSettingsOnInit(this GenericTutorial.TutorialStep step, GenericTutorial tutorial)
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
				tutorial.RestoreDefaultTutorialSettings();
			}
		});
	}

	public static void RestoreDefaultTutorialSettings(this GenericTutorial tutorial)
	{
		tutorial.allowInputThroughMask = false;
		tutorial.blockInputBetweenSteps = false;
		tutorial.autoChangeSteps = true;
	}

	public static GenericTutorial.TutorialStep AddDelayStep(this GenericTutorial tutorial, float delay)
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
				tutorial.autoChangeSteps = false;
				tutorial.NextStep(delay);
			}
		});
		tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
			action = delegate
			{
				tutorial.autoChangeSteps = true;
			}
		});
		tutorialStep.configInfoPanelAfterSpawn = delegate(GameObject p)
		{
			p.SetActive(value: false);
		};
		tutorial.AddTutorialStep(tutorialStep);
		return tutorialStep;
	}

	public static void AddInfoPanel(this GenericTutorial.TutorialStep step, string key, string defaultText)
	{
		step.infoKey = key;
		step.infoDefaultText = defaultText;
	}

	public static void RemoveInfoPanel(this GenericTutorial.TutorialStep step)
	{
		step.configInfoPanelAfterSpawn = delegate(GameObject p)
		{
			p.SetActive(value: false);
		};
	}

	public static void AddElementButtonCallback(this GenericTutorial.TutorialStep step, GameObject go = null)
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
				step.element.GetComponentInChildren<Button>().onClick.Invoke();
			}
		});
	}

	public static void AddElementToStepOnSpawn(this GenericTutorial.TutorialStep step, Func<GameObject> getGO, Action<GenericTutorial.TutorialStep, GameObject> addOtherInitialCallbacks = null)
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

	public static void AddElementToClickOnSpawn(this GenericTutorial.TutorialStep step, Func<GameObject> getGO)
	{
		step.AddElementToStepOnSpawn(getGO, AddElementButtonCallback);
	}

	public static void ChangeToNextStepAfterDelay(GenericTutorial tutorial, GenericTutorial.TutorialStep step, float delay, GenericTutorial.CallbackType callbackType = GenericTutorial.CallbackType.ON_STEP_SPAWN)
	{
		if (step.tutorialCallbacks == null)
		{
			step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
		}
		step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
		{
			type = callbackType,
			action = delegate
			{
				tutorial.NextStep(delay);
			}
		});
	}

	public static void AddTutorialStep(this GenericTutorial genericTutorial, GenericTutorial.TutorialStep step)
	{
		if (genericTutorial.tutorialSteps == null)
		{
			genericTutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
		}
		GenericTutorial.TutorialStep[] array = new GenericTutorial.TutorialStep[genericTutorial.tutorialSteps.Length + 1];
		Array.Copy(genericTutorial.tutorialSteps, array, array.Length - 1);
		array[array.Length - 1] = step;
		genericTutorial.tutorialSteps = array;
	}

	public static void AddStepToTutorial(this GenericTutorial.TutorialStep step, GenericTutorial tutorial)
	{
		tutorial.AddTutorialStep(step);
	}
}
