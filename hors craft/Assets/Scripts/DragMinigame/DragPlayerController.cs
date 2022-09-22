// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragPlayerController
using UnityEngine;

namespace DragMinigame
{
	public class DragPlayerController : MonoBehaviour
	{
		public enum ShiftRating
		{
			PERFECT,
			GOOD,
			BAD
		}

		[SerializeField]
		protected Transform VisualRepresentationAnchor;

		public DragPlayerConfig playerConfig;

		protected DragGameManager gameManager;

		protected ShiftingController shiftController;

		protected Progressmeter progressmeter;

		protected Boost gameBoost;

		protected DragGameConfig gameConfig;

		protected GameObject playerRepresentation;

		protected ShiftRating switchRating;

		protected int transitionIndex = 1;

		protected bool gasDown;

		protected float currentShiftProgress;

		protected float currentOverallProgress;

		protected bool isBoost;

		protected bool hasFinished;

		protected virtual void GetReferences()
		{
			shiftController = GetComponent<ShiftingController>();
			progressmeter = GetComponent<Progressmeter>();
			gameBoost = GetComponent<Boost>();
		}

		public virtual void Init(DragGameConfig gameConfig, DragPlayerConfig playerConfig)
		{
			GetReferences();
			this.playerConfig = playerConfig;
			this.gameConfig = gameConfig;
			gameManager = DragGameManager.instance;
			shiftController.Init(playerConfig);
			progressmeter.Init(gameConfig.startZones, gameConfig.maxProgressShiftValue);
			gameBoost.Init(playerConfig.BoostTime);
			SetVisualRepresentation();
		}

		private void SetVisualRepresentation()
		{
			if (!(playerConfig.VisualRepresentation == null))
			{
				playerRepresentation = UnityEngine.Object.Instantiate(playerConfig.VisualRepresentation, VisualRepresentationAnchor);
				playerRepresentation.transform.localPosition = Vector3.zero;
			}
		}

		public virtual void UpdateInit()
		{
		}

		public virtual void UpdateStart()
		{
			HandleStart();
			CalculateCurrentShiftProgress();
		}

		public virtual void UpdatePlay()
		{
			HandleUI();
			CalculateCurrentShiftProgress();
			CalculateCurrentOverallProgress();
		}

		public virtual void UpdateFinish()
		{
		}

		protected virtual void Penalize(float penalty)
		{
		}

		public virtual void SwitchSoundOnTutorial(bool on)
		{
		}

		public virtual void FinishGame()
		{
			hasFinished = true;
		}

		public bool CheckIfFishshed()
		{
			return hasFinished;
		}

		public virtual void HandleStartInput(bool gasDown)
		{
			this.gasDown = gasDown;
		}

		public virtual void HandleRaceInput()
		{
			if (shiftController.currentTransfer < 6 && gameManager.gameState == DragGameManager.GameState.Race)
			{
				Shift();
			}
		}

		protected virtual float Shift()
		{
			if (gameConfig.resetProgressOnShift)
			{
				ResetProgress();
			}
			float penaltyAndRating = gameConfig.changeGearZones.GetPenaltyAndRating(currentShiftProgress, ref switchRating);
			gameManager.ShowSwitchInfo(switchRating);
			Penalize(penaltyAndRating);
			shiftController.NextTransfer();
			if (shiftController.currentTransfer == 5)
			{
				gameManager.ShowNextTutorialStep();
			}
			return penaltyAndRating;
		}

		public virtual void EnableBoost()
		{
			if (gameManager.gameState == DragGameManager.GameState.Race && !(gameBoost.currentBoost <= 0f))
			{
				isBoost = true;
				gameManager.isBoostOn = true;
			}
		}

		protected virtual void HandleUI()
		{
			progressmeter.ShowProgress(currentShiftProgress, gameConfig.changeGearZones);
		}

		protected virtual void HandleStart()
		{
			if (gasDown)
			{
				CalculateCurrentOverallProgress();
			}
			else if (currentOverallProgress > 0f)
			{
				CalculateCurrentOverallProgress(-1);
			}
			progressmeter.ShowProgress(currentShiftProgress, gameConfig.startZones);
		}

		protected virtual void CalculateCurrentShiftProgress()
		{
			currentShiftProgress = progressmeter.CalculateShiftProgress(shiftController.currentShiftMaxValue, currentOverallProgress);
			if (currentShiftProgress >= gameConfig.changeGearZones.minPerfectValue && shiftController.currentTransfer == 1 && gameManager.gameState == DragGameManager.GameState.Race)
			{
				gameManager.ShowNextTutorialStep();
			}
		}

		public void ResetProgress()
		{
			currentOverallProgress = 0f;
		}

		protected virtual void CalculateCurrentOverallProgress(int multiplier = 1)
		{
			currentOverallProgress += Time.deltaTime * 20f * (float)multiplier;
		}

		public virtual void StartRacing(DragGameConfig gameConfig)
		{
			progressmeter.Init(gameConfig.changeGearZones, gameConfig.maxProgressShiftValue);
			float penaltyAndRating = gameConfig.startZones.GetPenaltyAndRating(currentShiftProgress, ref switchRating);
			gameManager.ShowSwitchInfo(switchRating);
			Penalize(penaltyAndRating);
			if (gameConfig.resetProgressOnStart)
			{
				ResetProgress();
			}
		}
	}
}
