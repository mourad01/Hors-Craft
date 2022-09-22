// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragRacingGameManager
using UnityEngine;

namespace DragMinigame
{
	public class DragRacingGameManager : DragGameManager
	{
		private readonly string RACE_RECORD_KEY = "raceRecord: ";

		public static DragRacingGameManager dragRacingManagerInstance;

		[HideInInspector]
		public CameraEffectsController cameraEffects;

		private DragRacingUIController dragRacingUIController;

		private DragRacingPlayerController dragRacingPlayerController;

		private EnemyController enemyController;

		private DragRacingGameConfig dragRacingGameConfig;

		private RoadController roadController;

		private AudioSource Sound2d;

		private float raceRecord;

		private float acctualRacelength;

		protected override void GetReferences()
		{
			base.GetReferences();
			enemyController = Object.FindObjectOfType<EnemyController>();
			cameraEffects = Object.FindObjectOfType<CameraEffectsController>();
			roadController = GetComponent<RoadController>();
			dragRacingManagerInstance = (DragGameManager.instance as DragRacingGameManager);
			dragRacingUIController = (uiController as DragRacingUIController);
			dragRacingPlayerController = (playerController as DragRacingPlayerController);
			dragRacingGameConfig = (dragGameConfig as DragRacingGameConfig);
			Sound2d = GetComponent<AudioSource>();
		}

		public void SkipIntro()
		{
			cameraEffects.ActivateTransitionCamera("Skip Intro", cameraEffects.cameraController.ActiveMotorIndex);
		}

		protected override void UpdateInit()
		{
			base.UpdateInit();
			enemyController.UpdateInit();
		}

		protected override void UpdateStart()
		{
			base.UpdateStart();
			enemyController.UpdateStart();
		}

		protected override void UpdatePlay()
		{
			base.UpdatePlay();
			enemyController.UpdatePlay();
			CheckEnemyFinish();
		}

		protected override void UpdateFinish()
		{
			base.UpdateFinish();
			enemyController.UpdateFinish();
			CheckEnemyFinish();
		}

		protected override void PrepareGame()
		{
			base.PrepareGame();
			raceRecord = PlayerPrefs.GetFloat(RACE_RECORD_KEY + dragGameConfig.Name, float.PositiveInfinity);
			int num = Mathf.CeilToInt(dragRacingGameConfig.raceLength / roadController.GetRoadPartLength());
			roadController.SetRoadsToSpawnAmount(num);
			acctualRacelength = (float)num * roadController.GetRoadPartLength();
			enemyController.Init(dragGameConfig, (dragGameConfig as DragRacingGameConfig).enemyConfig.carConfig);
			cameraEffects.Init();
			roadController.Init();
		}

		protected override void CountGameTime()
		{
			base.CountGameTime();
			dragRacingUIController.UpdateTimer(raceTime);
		}

		public void CheckCameraEvents()
		{
			string activeMotorName = cameraEffects.GetActiveMotorName();
			if (activeMotorName == "Start")
			{
				StartGame();
			}
			else if (activeMotorName.Contains("Showcase"))
			{
				dragRacingUIController.FadeOutIn();
			}
		}

		public override void StartGame()
		{
			base.StartGame();
			dragRacingUIController.FadeInDashBoard();
			dragRacingUIController.DisableSkipIntro();
		}

		protected override void StartRace()
		{
			base.StartRace();
			enemyController.StartRacing(dragGameConfig);
		}

		public override void CheckFinish()
		{
			Vector3 position = playerController.transform.position;
			if (position.z >= acctualRacelength && gameState != GameState.Finish)
			{
				FinishGame();
			}
		}

		private void CheckEnemyFinish()
		{
			Vector3 position = enemyController.transform.position;
			if (position.z >= acctualRacelength && !enemyController.CheckIfFishshed())
			{
				enemyController.FinishGame();
			}
		}

		public override void FinishGame()
		{
			base.FinishGame();
			cameraEffects.ActivateTransitionCamera("To End", cameraEffects.cameraController.ActiveMotorIndex);
			CheckRecord();
		}

		private void CheckRecord()
		{
			if (raceTime < raceRecord)
			{
				dragRacingUIController.ShowRecord();
				PlayerPrefs.SetFloat(RACE_RECORD_KEY + dragGameConfig.Name, raceTime);
			}
		}

		protected override void SwitchSoundOnTutorial(bool on)
		{
			playerController.SwitchSoundOnTutorial(on);
			enemyController.SwitchSoundOnTutorial(on);
		}

		public override void ExitGame()
		{
			base.ExitGame();
			roadController.UnloadRoad();
		}

		public void Play2dSound(AudioClip clip, float pitch = 1f, float volume = 1f)
		{
			Sound2d.clip = clip;
			Sound2d.pitch = pitch;
			Sound2d.volume = volume;
			Sound2d.Play();
		}
	}
}
