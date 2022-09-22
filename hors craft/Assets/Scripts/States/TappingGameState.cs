// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TappingGameState
using Common.Managers;
using Common.Managers.States;
using System;
using UnityEngine;

namespace States
{
	public class TappingGameState : XCraftUIState<TappingGameStateConnector>
	{
		public bool canExit = true;

		public Sprite leftButtonIcon;

		public Sprite rightButtonIcon;

		public Sprite iconsBG;

		private GameObject graphicsPrefab;

		private GenericTutorial tutorial;

		private TappingGameBehaviour tappingGameBehaviour;

		private TappingGameGraphics tappingGameGraphics;

		private bool paused;

		private int currentLevel;

		private TappingGameStateStartParameter startParameter;

		private GameObject graphicsPrefabInstance;

		public Action onWin;

		public Action onFail;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		//protected override bool canShowBanner => false;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			Pause(activate: true);
			startParameter = (parameter as TappingGameStateStartParameter);
			graphicsPrefab = startParameter.graphicPrefab;
			graphicsPrefabInstance = UnityEngine.Object.Instantiate(graphicsPrefab, base.connector.transform, worldPositionStays: false);
			graphicsPrefabInstance.transform.SetAsFirstSibling();
			tappingGameGraphics = graphicsPrefabInstance.GetComponent<TappingGameGraphics>();
			tappingGameGraphics.InitGraphics();
			tappingGameBehaviour = startParameter.gameBehaviour;
			tappingGameBehaviour.tappingGameGraphics = tappingGameGraphics;
			base.connector.leftActionButton.onClick.AddListener(OnLeftButtonTap);
			base.connector.rightActionButton.onClick.AddListener(OnRightButtonTap);
			base.connector.returnButton.onClick.AddListener(OnReturn);
			base.connector.returnButton.gameObject.SetActive(canExit);
			if (leftButtonIcon != null)
			{
				base.connector.leftButtonIcon.sprite = leftButtonIcon;
			}
			if (rightButtonIcon != null)
			{
				base.connector.rightButtonIcon.sprite = rightButtonIcon;
			}
			if (iconsBG != null)
			{
				base.connector.leftBackground.sprite = iconsBG;
				base.connector.rightBackground.sprite = iconsBG;
			}
			if (tappingGameBehaviour.LoadTutorialStatus() == 0)
			{
				tutorial = GetComponent<GenericTutorial>();
				tutorial.tutorialSteps = tappingGameBehaviour.GetTutorialSteps(this, base.connector, tutorial.tutorialSteps);
				tutorial.StartTutorial();
			}
			else
			{
				PrepareForRound();
			}
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (!paused)
			{
				if (tappingGameBehaviour.GetState == TappingGameBehaviour.GameState.lost)
				{
					Pause(activate: true);
					RoundLost();
				}
				else if (tappingGameBehaviour.GetState == TappingGameBehaviour.GameState.won)
				{
					Pause(activate: true);
					RoundWon();
				}
				else
				{
					tappingGameBehaviour.Update();
				}
			}
		}

		public override void FinishState()
		{
			tappingGameGraphics.OnFinish();
			MonoBehaviourSingleton<ProgressCounter>.get.Increment(ProgressCounter.Countables.MINIGAMES_PLAYED);
			base.FinishState();
		}

		public void PrepareForRound()
		{
			tappingGameBehaviour.ResetRound();
			tappingGameBehaviour.SetLevel(currentLevel);
			tappingGameGraphics.callAfterAnimations = StartRound;
			tappingGameGraphics.OnResetRound();
		}

		private void StartRound()
		{
			Pause(activate: false);
		}

		private void RoundWon()
		{
			if (tappingGameBehaviour.NextRoundAvailable())
			{
				currentLevel++;
				tappingGameGraphics.callAfterAnimations = PrepareForRound;
			}
			else
			{
				tappingGameGraphics.callAfterAnimations = OnReturn;
				Manager.Get<StatsManager>().MinigameFinished(tappingGameBehaviour.GetGameType(), success: true);
			}
			tappingGameGraphics.OnWin();
			if (onWin != null)
			{
				onWin();
			}
		}

		private void RoundLost()
		{
			tappingGameGraphics.callAfterAnimations = OnReturn;
			Manager.Get<StatsManager>().MinigameFinished(tappingGameBehaviour.GetGameType(), success: false);
			tappingGameGraphics.OnLose();
			if (onFail != null)
			{
				onFail();
			}
		}

		private void OnLeftButtonTap()
		{
			tappingGameBehaviour.LeftActionButton();
		}

		private void OnRightButtonTap()
		{
			tappingGameBehaviour.RightActionButton();
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		private void Pause(bool activate)
		{
			paused = activate;
			base.connector.leftActionButton.interactable = !activate;
			base.connector.rightActionButton.interactable = !activate;
		}
	}
}
