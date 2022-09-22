// DecompilerFi decompiler from Assembly-CSharp.dll class: TutorialGenerator
using Common.Managers;
using States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialGenerator : MonoBehaviour
{
	public UnityEvent onTutorialFinished;

	public GenericTutorial genericTutorial;

	private HashSet<GameObject> gameplayModulesToHide = new HashSet<GameObject>();

	public virtual void Start()
	{
		genericTutorial = GetComponent<GenericTutorial>();
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_TUTORIAL);
		InitTutorial();
	}

	public virtual void InitTutorial()
	{
	}

	public virtual void EndTutorial()
	{
		onTutorialFinished.Invoke();
	}

	private void OnDestroy()
	{
		onTutorialFinished.RemoveAllListeners();
	}

	public void GetAndHideGameplayModules()
	{
		foreach (GameplayModule module in Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules())
		{
			if (module.gameObject.activeSelf && !(module is MovementAnalogModule) && !(module is TouchIndicatorModule) && !(module is TouchInputAreaModule) && !(module is PauseModule) && !(module is BlocksPopupButtonModule))
			{
				gameplayModulesToHide.Add(module.gameObject);
				module.gameObject.SetActive(value: false);
			}
		}
	}

	public void ActivateHiddenGameplayModules()
	{
		foreach (GameObject item in gameplayModulesToHide)
		{
			item.gameObject.SetActive(value: true);
		}
	}

	public GameObject GetGameplayMovementAnalog()
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

		public TouchInputAreaModule GetGameplayTouchInputAreaModule()
		{
			return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
				.FirstOrDefault((GameplayModule m) => m is TouchInputAreaModule) as TouchInputAreaModule;
		}

		public PauseModule GetGameplayPauseModule()
		{
			return Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
				.FirstOrDefault((GameplayModule m) => m is PauseModule) as PauseModule;
		}

		public GameObject GetGameplayBlockPopupButton()
		{
			BlocksPopupButtonModule blocksPopupButtonModule = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().GetModules()
				.FirstOrDefault((GameplayModule m) => m is BlocksPopupButtonModule) as BlocksPopupButtonModule;
			return blocksPopupButtonModule.blocksButton.gameObject;
		}

		public GameObject GetBlocksPopupBlockToChooseButton()
		{
			ChooseBlockButtonController[] componentsInChildren = Manager.Get<StateMachineManager>().GetStateInstance<BlocksPopupState>().blocks.scrollListContentView.GetComponentsInChildren<ChooseBlockButtonController>();
			return componentsInChildren[2].gameObject;
		}

		public GenericTutorial.TutorialCallback TurnOnAutoChangeStepsCallback()
		{
			GenericTutorial.TutorialCallback tutorialCallback = new GenericTutorial.TutorialCallback();
			tutorialCallback.type = GenericTutorial.CallbackType.ON_STEP_INIT;
			tutorialCallback.action = delegate
			{
				genericTutorial.autoChangeSteps = true;
			};
			return tutorialCallback;
		}

		public void CloseTutorial()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_TUTORIAL);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
