// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragGameManager
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DragMinigame
{
	public class DragGameManager : AbstractMinigameManager
	{
		public enum GameState
		{
			Init,
			Start,
			Race,
			Finish
		}

		public static DragGameManager instance;

		private const string TUTORIAL_PREFS_KEY = "dragGameTutorial";

		public GameState gameState;

		[HideInInspector]
		public bool isBoostOn;

		[SerializeField]
		protected DragGameConfig dragGameConfig;

		[SerializeField]
		private bool hasCustomStartBehaviour;

		[SerializeField]
		protected float countdownTime;

		[SerializeField]
		private bool skipTutorial;

		protected UIController uiController;

		protected DragPlayerController playerController;

		protected float currentCountdownTime;

		protected float raceTime;

		protected bool gameIsInit;

		protected DragPlayerConfig dragPlayerConfig;

		private TutorialController tutorialController;

		private TimeScaleController timeController;

		private MobsManager mobsManager;

		private bool isShowingTutorialStep;

		private bool isShowingTutorial;

		private string sceneName;

		private bool pushedGas;

		public override void Init(MinigameStartParameter parameter)
		{
			if (instance == null)
			{
				instance = this;
			}
			mobsManager = Manager.Get<MobsManager>();
			mobsManager.gameObject.SetActive(value: false);
			Application.targetFrameRate = 60;
			base.Init(parameter);
			GetReferences();
			PrepareGame();
			if (!hasCustomStartBehaviour)
			{
				StartGame();
			}
		}

		protected virtual void GetReferences()
		{
			playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<DragPlayerController>();
			uiController = GetComponent<UIController>();
			tutorialController = GetComponent<TutorialController>();
			timeController = GetComponent<TimeScaleController>();
		}

		protected virtual void PrepareGame()
		{
			isShowingTutorial = (!skipTutorial && ((PlayerPrefs.GetInt("dragGameTutorial") != 1) ? true : false));
			currentCountdownTime = countdownTime;
			uiController.Init();
			playerController.Init(dragGameConfig, dragPlayerConfig);
			gameIsInit = true;
		}

		protected virtual void Update()
		{
			if (gameIsInit)
			{
				switch (gameState)
				{
				case GameState.Init:
					UpdateInit();
					break;
				case GameState.Start:
					UpdateStart();
					break;
				case GameState.Race:
					UpdatePlay();
					break;
				case GameState.Finish:
					UpdateFinish();
					break;
				}
			}
		}

		public override void ProcessParameter(MinigameStartParameter parameter)
		{
			base.ProcessParameter(parameter);
			sceneName = (minigame as CustomSceneMinigame).sceneToLoad;
			dragPlayerConfig = (playerConfig as DragPlayerConfig);
			dragGameConfig = (gameConfig as DragGameConfig);
		}

		protected virtual void UpdateInit()
		{
			playerController.UpdateInit();
		}

		protected virtual void UpdateStart()
		{
			ProgressCountdown();
			playerController.UpdateStart();
		}

		protected virtual void UpdatePlay()
		{
			CheckFinish();
			CountGameTime();
			playerController.UpdatePlay();
		}

		protected virtual void UpdateFinish()
		{
			playerController.UpdateFinish();
		}

		public virtual void StartGame()
		{
			ShowNextTutorialStep();
			gameState = GameState.Start;
			uiController.StartCountDown();
		}

		public virtual void FinishGame()
		{
			gameState = GameState.Finish;
			playerController.FinishGame();
			PlayerPrefs.SetInt("dragGameTutorial", 1);
		}

		public virtual void CheckFinish()
		{
			if (dragGameConfig.gameTime > 0f && raceTime > dragGameConfig.gameTime)
			{
				FinishGame();
			}
		}

		public void SetTutorialUI(TutorialController.TutorialStep step)
		{
			uiController.SetTutorialUI(step);
		}

		private void CheckIfShouldHideTutorial()
		{
			if (isShowingTutorialStep)
			{
				uiController.HideTutorial();
				tutorialController.HideTutorial();
				timeController.SpeedUp();
				isShowingTutorialStep = false;
				SwitchSoundOnTutorial(von: true);
			}
		}

		protected virtual void SwitchSoundOnTutorial(bool von)
		{
		}

		private void ProgressCountdown()
		{
			if (pushedGas)
			{
				currentCountdownTime -= Time.deltaTime;
				int time = Mathf.CeilToInt(currentCountdownTime);
				uiController.UpdateCountDown(time);
				if (currentCountdownTime < 0f)
				{
					StartRace();
				}
			}
		}

		protected virtual void StartRace()
		{
			gameState = GameState.Race;
			playerController.StartRacing(dragGameConfig);
			uiController.StopCountDown();
		}

		public void UpdateShiftUI(int currentShift)
		{
			uiController.UpdateShiftUI(currentShift);
		}

		public void ShowSwitchInfo(DragPlayerController.ShiftRating gearSwitch)
		{
			uiController.ShowSwitchInfo(gearSwitch);
		}

		public void ShowNextTutorialStep()
		{
			if (isShowingTutorial)
			{
				TutorialController.TutorialStep nextStep = tutorialController.GetNextStep();
				if (nextStep != null)
				{
					uiController.SetTutorialText(nextStep.key, nextStep.defaultText);
					isShowingTutorialStep = true;
					timeController.SlowDown();
					SwitchSoundOnTutorial(von: false);
				}
			}
		}

		public bool CheckIfButtonIsActive(GameObject obj)
		{
			if (!isShowingTutorial)
			{
				return true;
			}
			if (isShowingTutorial && !isShowingTutorialStep && tutorialController.activatedButtons.Contains(obj))
			{
				return true;
			}
			if (isShowingTutorial && isShowingTutorialStep && tutorialController.currentActiveButton == obj)
			{
				return true;
			}
			return false;
		}

		public virtual void HandleStartInput(bool gasDown)
		{
			if (gasDown && !pushedGas)
			{
				pushedGas = true;
			}
			CheckIfShouldHideTutorial();
			playerController.HandleStartInput(gasDown);
		}

		public virtual void HandleRaceInput()
		{
			CheckIfShouldHideTutorial();
			playerController.HandleRaceInput();
		}

		public void EnableBoost()
		{
			CheckIfShouldHideTutorial();
			playerController.EnableBoost();
		}

		protected virtual void CountGameTime()
		{
			raceTime += Time.deltaTime;
		}

		public float GetBoostTime()
		{
			return playerController.playerConfig.BoostTime;
		}

		public override void ExitGame()
		{
			mobsManager.gameObject.SetActive(value: true);
			SceneManager.UnloadSceneAsync(sceneName);
			Manager.Get<StateMachineManager>().SetState<LoadLevelState>();
		}
	}
}
