// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragRacingPlayerController
using com.ootii.Cameras;
using UnityEngine;

namespace DragMinigame
{
	public class DragRacingPlayerController : DragPlayerController
	{
		private DragRacingCarConfig dragRaceCarConfig;

		private DragRacingGameManager dragRacingManager;

		private CarPhysics carPhysics;

		private SoundController soundController;

		private Speedometer speedometer;

		private Tachometer tachometer;

		private WheelRotator wheelRotator;

		private CameraController cameraController;

		private Nitrometer nitrometer;

		private float currentTranslation;

		private float targetFriction = 2f;

		private float currentFriction;

		private float startingTacho;

		private bool isStarting;

		private bool cameraRaceMovementStarted;

		protected override void GetReferences()
		{
			base.GetReferences();
			cameraController = Object.FindObjectOfType<CameraController>();
			speedometer = GetComponent<Speedometer>();
			soundController = GetComponent<SoundController>();
			carPhysics = GetComponent<CarPhysics>();
			wheelRotator = GetComponent<WheelRotator>();
		}

		public override void Init(DragGameConfig gameConfig, DragPlayerConfig playerConfig)
		{
			base.Init(gameConfig, playerConfig);
			dragRacingManager = (gameManager as DragRacingGameManager);
			nitrometer = (gameBoost as Nitrometer);
			dragRaceCarConfig = (playerConfig as DragRacingCarConfig);
			tachometer = (progressmeter as Tachometer);
			wheelRotator.Init();
			carPhysics.Init(playerRepresentation.transform.FindChildRecursively("Body"));
		}

		public float GetNitroTime()
		{
			return nitrometer.fullBoost;
		}

		protected override void Penalize(float penalty)
		{
			currentFriction += penalty * (1f + (float)shiftController.currentTransfer * 0.15f);
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

		public override void UpdateInit()
		{
			HandleSound();
		}

		public override void UpdateStart()
		{
			base.UpdateStart();
			HandleSound();
			HandlePhysics();
		}

		public override void UpdatePlay()
		{
			base.UpdatePlay();
			HandleEffects();
			UpdatePlayerPosition();
			HandleSound();
			HandlePhysics();
			HandleWheels();
			HandleNitro();
		}

		public override void UpdateFinish()
		{
			CalculateCurrentShiftProgress();
			HandleUI();
			HandleEffects();
			UpdatePlayerPosition();
			HandleSound();
			CalculateCurrentOverallProgress();
			HandlePhysics();
			HandleWheels();
			HandleNitro();
		}

		protected override void CalculateCurrentOverallProgress(int multiplier = 1)
		{
			if (gameManager.gameState == DragGameManager.GameState.Finish && hasFinished)
			{
				if (currentOverallProgress > 0f)
				{
					currentOverallProgress -= dragRaceCarConfig.Braking * Time.deltaTime;
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
					currentFriction = Mathf.MoveTowards(currentFriction, targetFriction, Time.deltaTime * dragRaceCarConfig.Acceleration);
					currentOverallProgress -= currentFriction * Time.deltaTime;
				}
			}
			else
			{
				currentOverallProgress = 0f;
			}
			float num = 1f + (float)(shiftController.currentTransfer - 1) * dragRaceCarConfig.GearSpeedRetention;
			if (currentShiftProgress >= gameConfig.changeGearZones.maxAcceptableValue)
			{
				num *= 2f;
			}
			if (isBoost && currentOverallProgress < shiftController.currentShiftMaxValue)
			{
				if (currentOverallProgress < dragRaceCarConfig.MaxSpeed)
				{
					currentOverallProgress += (playerConfig.Power + playerConfig.BoostValue) / 10f / num * Time.deltaTime;
				}
			}
			else if (currentOverallProgress < dragRaceCarConfig.MaxSpeed)
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
			if (!(currentOverallProgress < 1f) || gameManager.gameState != DragGameManager.GameState.Finish)
			{
				currentTranslation = currentOverallProgress * Time.deltaTime * 0.5f;
				base.transform.position += Vector3.forward * currentTranslation;
			}
		}

		protected override void HandleUI()
		{
			speedometer.ShowSpeed(currentOverallProgress);
			if (shiftController.currentTransfer == 1 && currentShiftProgress < startingTacho)
			{
				tachometer.ShowArrowAngleAtStart(currentShiftProgress, startingTacho, tachometer.maxValue);
			}
			else
			{
				tachometer.ShowArrowAngle(shiftController.currentShiftMaxValue, currentOverallProgress);
			}
		}

		private void HandleWheels()
		{
			if (!(currentOverallProgress < 1f) || gameManager.gameState != DragGameManager.GameState.Finish)
			{
				wheelRotator.UpdateWheels(currentTranslation);
			}
		}

		private void HandlePhysics()
		{
			if (gameManager.gameState == DragGameManager.GameState.Start)
			{
				carPhysics.Rotate(carPhysics.StartRotation.angle * currentShiftProgress / 5f);
			}
			else if (gameManager.gameState == DragGameManager.GameState.Finish)
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

		private void HandleEffects()
		{
			bool isFinished = gameManager.gameState == DragGameManager.GameState.Finish;
			if (currentOverallProgress > 5f && !cameraRaceMovementStarted)
			{
				cameraRaceMovementStarted = true;
				dragRacingManager.cameraEffects.ActivateTransitionCamera("Transition 1");
			}
			dragRacingManager.cameraEffects.UpdateValues(currentShiftProgress, currentOverallProgress, isFinished);
			dragRacingManager.cameraEffects.UpdateEffects();
		}

		private void HandleSound()
		{
			if (gameManager.gameState == DragGameManager.GameState.Finish)
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

		private void HandleNitro()
		{
			if (nitrometer.currentBoost > 0f && isBoost)
			{
				nitrometer.CalculateBoost();
			}
			else
			{
				isBoost = false;
				gameManager.isBoostOn = false;
				soundController.NitroStop();
			}
			nitrometer.ShowNitro();
		}

		protected override float Shift()
		{
			float num = base.Shift();
			dragRacingManager.cameraEffects.ChangeGear(num / gameConfig.changeGearZones.maxPenalty, isPlayer: true);
			soundController.GearPlay(num / gameConfig.changeGearZones.maxPenalty);
			gameManager.UpdateShiftUI(shiftController.currentTransfer);
			return num;
		}

		public override void EnableBoost()
		{
			base.EnableBoost();
			if (gameManager.gameState == DragGameManager.GameState.Race && !(nitrometer.currentBoost <= 0f))
			{
				dragRacingManager.cameraEffects.NitroFov();
				soundController.NitroPlay();
			}
		}

		public override void HandleRaceInput()
		{
			if (!isStarting)
			{
				base.HandleRaceInput();
			}
		}

		protected override void HandleStart()
		{
			float target = (!gasDown) ? 0f : shiftController.currentShiftMaxValue;
			currentOverallProgress = Mathf.MoveTowards(currentOverallProgress, target, Time.deltaTime * playerConfig.Power / 10f);
			tachometer.ShowArrowAngle(tachometer.maxValue, currentShiftProgress);
		}

		public override void StartRacing(DragGameConfig gameConfig)
		{
			base.StartRacing(gameConfig);
			startingTacho = currentShiftProgress;
			dragRacingManager.cameraEffects.StartShake();
			isStarting = true;
		}

		public override void FinishGame()
		{
			base.FinishGame();
			soundController.StopEngine();
			soundController.BrakingPlay();
			carPhysics.RotateBraking();
		}

		protected override void CalculateCurrentShiftProgress()
		{
			base.CalculateCurrentShiftProgress();
			tachometer.ShowTachoText(currentShiftProgress);
		}
	}
}
