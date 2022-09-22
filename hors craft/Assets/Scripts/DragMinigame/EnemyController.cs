// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.EnemyController
using UnityEngine;

namespace DragMinigame
{
	public class EnemyController : DragPlayerController
	{
		private CarPhysics carPhysics;

		private WheelRotator wheelRotator;

		private DragRacingGameManager dragRacingManager;

		private SoundController soundController;

		private float currentTranslation;

		private float targetFriction = 2f;

		private float currentFriction;

		private float startingTacho;

		private bool isStarting;

		private bool cameraRaceMovementStarted;

		private float startTacho;

		private float nextGearTacho;

		private DragRacingCarConfig enemyCarConfig;

		private DragRacingEnemyConfig enemyConfig;

		protected override void GetReferences()
		{
			base.GetReferences();
			carPhysics = GetComponent<CarPhysics>();
			wheelRotator = GetComponent<WheelRotator>();
			soundController = GetComponent<SoundController>();
		}

		public override void Init(DragGameConfig gameConfig, DragPlayerConfig playerConfig)
		{
			base.Init(gameConfig, playerConfig);
			dragRacingManager = (gameManager as DragRacingGameManager);
			DragRacingGameConfig dragRacingGameConfig = gameConfig as DragRacingGameConfig;
			wheelRotator.Init();
			enemyConfig = dragRacingGameConfig.enemyConfig;
			enemyCarConfig = (playerConfig as DragRacingCarConfig);
			carPhysics.Init(playerRepresentation.transform.FindChildRecursively("Body"));
		}

		protected override void Penalize(float penalty)
		{
			currentFriction += penalty * (1f + (float)shiftController.currentTransfer * 0.15f);
		}

		public override void UpdatePlay()
		{
			base.UpdatePlay();
			UpdatePlayerPosition();
			HandlePhysics();
			HandleWheels();
			HandleNitro();
			HandleShifting();
			HandleSound();
		}

		public override void UpdateFinish()
		{
			CalculateCurrentShiftProgress();
			UpdatePlayerPosition();
			CalculateCurrentOverallProgress();
			HandlePhysics();
			HandleWheels();
			HandleNitro();
			HandleSound();
		}

		protected override void CalculateCurrentShiftProgress()
		{
			currentShiftProgress = progressmeter.CalculateShiftProgress(shiftController.currentShiftMaxValue, currentOverallProgress);
		}

		protected override void CalculateCurrentOverallProgress(int multiplier = 1)
		{
			if (hasFinished)
			{
				if (currentOverallProgress > 0f)
				{
					currentOverallProgress -= enemyCarConfig.Braking * Time.deltaTime;
				}
				return;
			}
			if (currentOverallProgress > 0f)
			{
				if (currentOverallProgress >= shiftController.currentShiftMaxValue)
				{
					currentOverallProgress = shiftController.currentShiftMaxValue;
				}
				else
				{
					currentFriction = Mathf.MoveTowards(currentFriction, targetFriction, Time.deltaTime * enemyCarConfig.Acceleration);
					currentOverallProgress -= currentFriction * Time.deltaTime;
				}
			}
			else
			{
				currentOverallProgress = 0f;
			}
			float num = 1f + (float)(shiftController.currentTransfer - 1) * enemyCarConfig.GearSpeedRetention;
			if (currentShiftProgress >= gameConfig.maxProgressShiftValue)
			{
				num *= 2f;
			}
			if (isBoost && currentOverallProgress < shiftController.currentShiftMaxValue)
			{
				if (currentOverallProgress < enemyCarConfig.MaxSpeed)
				{
					currentOverallProgress += (playerConfig.Power + playerConfig.BoostValue) / 10f / num * Time.deltaTime;
				}
			}
			else if (currentOverallProgress < enemyCarConfig.MaxSpeed)
			{
				currentOverallProgress += playerConfig.Power / 10f / num * Time.deltaTime;
			}
			if (currentShiftProgress > startingTacho)
			{
				isStarting = false;
			}
		}

		private void UpdatePlayerPosition()
		{
			if (!(currentOverallProgress < 1f) || !hasFinished)
			{
				currentTranslation = currentOverallProgress * Time.deltaTime * 0.5f;
				base.transform.position += Vector3.forward * currentTranslation;
			}
		}

		private void HandleWheels()
		{
			if (!(currentOverallProgress < 1f) || !hasFinished)
			{
				wheelRotator.UpdateWheels(currentTranslation);
			}
		}

		private void HandleShifting()
		{
			if (shiftController.currentTransfer < 6 && gameManager.gameState == DragGameManager.GameState.Race && currentShiftProgress > nextGearTacho)
			{
				Shift();
			}
		}

		private void HandlePhysics()
		{
			if (gameManager.gameState == DragGameManager.GameState.Start)
			{
				carPhysics.Rotate(carPhysics.StartRotation.angle * currentShiftProgress / 5f);
			}
			else if (hasFinished)
			{
				if (currentOverallProgress > 0f)
				{
					carPhysics.RotateBraking();
				}
				else if (currentOverallProgress <= 0f)
				{
					carPhysics.Rotate(0f);
				}
			}
			else if (isBoost && currentOverallProgress < shiftController.currentShiftMaxValue)
			{
				carPhysics.RotateNitro();
			}
			else if (!isBoost && currentOverallProgress < 20f)
			{
				carPhysics.RotateStart();
			}
			else if (!isBoost && currentOverallProgress < shiftController.currentShiftMaxValue - 15f)
			{
				carPhysics.RotateDriving();
			}
			else if (!isBoost && currentOverallProgress >= shiftController.currentShiftMaxValue - 15f)
			{
				carPhysics.Rotate(0f);
			}
		}

		public override void StartRacing(DragGameConfig gameConfig)
		{
			DetermineNextGearTacho();
		}

		public override void EnableBoost()
		{
			base.EnableBoost();
			soundController.NitroPlay();
		}

		protected override float Shift()
		{
			if (gameConfig.resetProgressOnShift)
			{
				ResetProgress();
			}
			float penaltyAndRating = gameConfig.changeGearZones.GetPenaltyAndRating(currentShiftProgress, ref switchRating);
			soundController.GearPlay(penaltyAndRating / gameConfig.changeGearZones.maxPenalty);
			Penalize(penaltyAndRating);
			shiftController.NextTransfer();
			dragRacingManager.cameraEffects.ChangeGear(penaltyAndRating / gameConfig.changeGearZones.maxPenalty, isPlayer: false);
			DetermineNextGearTacho();
			return penaltyAndRating;
		}

		private void DetermineNextGearTacho()
		{
			int num = Random.Range(0, 10);
			bool flag = (float)num < enemyConfig.enemySkill;
			float num2 = 0f;
			float num3 = 0f;
			if (flag)
			{
				num2 = gameConfig.changeGearZones.minPerfectValue;
				num3 = gameConfig.changeGearZones.maxPerfectValue;
			}
			else if (Mathf.RoundToInt(Random.value) == 0)
			{
				num2 = gameConfig.changeGearZones.maxPerfectValue;
				num3 = gameConfig.changeGearZones.maxAcceptableValue;
			}
			else
			{
				num2 = gameConfig.changeGearZones.minAcceptableValue;
				num3 = gameConfig.changeGearZones.minPerfectValue;
			}
			nextGearTacho = Random.Range(num2, num3);
		}

		private void HandleNitro()
		{
			if (gameBoost.currentBoost > 0f && isBoost)
			{
				gameBoost.CalculateBoost();
				return;
			}
			isBoost = false;
			gameManager.isBoostOn = false;
			soundController.NitroStop();
		}

		public override void SwitchSoundOnTutorial(bool on)
		{
			if (on)
			{
				soundController.StartEngine();
			}
			else
			{
				soundController.StopEngine();
			}
		}

		private void HandleSound()
		{
			if (hasFinished)
			{
				if (currentOverallProgress <= 0f)
				{
					soundController.BrakingStop();
				}
			}
			else
			{
				soundController.SetEnginePItch(currentShiftProgress);
			}
		}

		public override void FinishGame()
		{
			base.FinishGame();
			soundController.StopEngine();
			soundController.BrakingPlay();
			carPhysics.RotateBraking();
		}
	}
}
