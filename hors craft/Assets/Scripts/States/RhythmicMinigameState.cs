// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RhythmicMinigameState
using com.ootii.Cameras;
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Gameplay.RhythmicMinigame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace States
{
	public class RhythmicMinigameState : XCraftUIState<RhythmicMinigameStateConnector>
	{
		public RhythmicMinigameConfigBehaviour configBehaviour;

		private const float TAP_CD = 0.2f;

		private RhythmicMinigameStateStartParameter startParameter;

		private RhythmReactor graphicScene;

		private RhythmController rhythmController;

		private RhythmBlobsController blobsController;

		private int currentDifficulty;

		private int nextBeatIndex;

		private int currentBeatId;

		private float levelProgress;

		private float levelProgressMax;

		private int correctBitsInSequenceCounter;

		private RhythmicMinigameConfigBehaviour.Sequence currentSequence;

		private RhythmicMinigameConfigBehaviour.Sequence emptySequence;

		private float tapCooldownEndTime;

		private IEnumerator<RhythmicMinigameConfigBehaviour.Sequence> sequencesEnumerator;

		private int lastDifficulty;

		private bool tutorial;

		private bool waitingToReturn;

		private bool anyBlobHit;

		public bool pauseWhileActive = true;

		public Sprite speakersSprite;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		//protected override bool canShowBanner => false;

		public override void StartState(StartParameter parameter)
		{
			base.StartState((StartParameter)startParameter);
			if (pauseWhileActive)
			{
				TimeScaleHelper.value = 0f;
			}
			else
			{
				TimeScaleHelper.value = 1f;
			}
			if (configBehaviour == null)
			{
				UnityEngine.Debug.Log("No RhythmicMinigameConfigBehaviour set up in RhythmicMinigameState! Assigning default from Resources. ");
				configBehaviour = Resources.Load<GameObject>("prefabs/defaultRhythmicMinigameConfig").GetComponent<RhythmicMinigameConfigBehaviour>();
			}
			waitingToReturn = false;
			startParameter = (parameter as RhythmicMinigameStateStartParameter);
			graphicScene = startParameter.graphicScene;
			graphicScene.SetPosition(base.connector.cameraPlayerPreview);
			base.connector.onTapButton = OnTap;
			base.connector.onReturnButton = OnReturn;
			if (speakersSprite != null)
			{
				base.connector.speakerImage.sprite = speakersSprite;
			}
			nextBeatIndex = 0;
			currentDifficulty = 0;
			correctBitsInSequenceCounter = 0;
			anyBlobHit = false;
			rhythmController = new RhythmController();
			blobsController = new RhythmBlobsController(this, rhythmController, base.connector.bitPrefab);
			rhythmController.AddOnBeatListener(blobsController.OnBeat);
			rhythmController.AddOnBeatListener(OnBeat);
			emptySequence = new RhythmicMinigameConfigBehaviour.Sequence
			{
				beats = new int[4]
			};
			PrepareSequences();
			FirstSequence();
			rhythmController.Play();
		}

		private void PrepareSequences()
		{
			foreach (RhythmicMinigameConfigBehaviour.SequencesList item in configBehaviour.sequencesPerDifficulty)
			{
				foreach (RhythmicMinigameConfigBehaviour.Sequence item2 in item.list)
				{
					item2.PrepareBeats();
				}
			}
		}

		private void FirstSequence()
		{
			nextBeatIndex = 0;
			correctBitsInSequenceCounter = 0;
			StartCurrentDifficulty();
			blobsController.AssignNewSequence(currentSequence);
		}

		private void NextSequence()
		{
			nextBeatIndex = 0;
			correctBitsInSequenceCounter = 0;
			tutorial = (levelProgress == 0f && currentDifficulty == 0);
			if (levelProgress >= levelProgressMax)
			{
				if (currentDifficulty + 1 < configBehaviour.sequencesPerDifficulty.Count)
				{
					currentDifficulty++;
					StartCurrentDifficulty();
				}
				else
				{
					StartSuccessScene();
				}
			}
			else
			{
				NextSequenceInCurrentDifficulty();
			}
			blobsController.AssignNewSequence(currentSequence);
		}

		private void StartCurrentDifficulty()
		{
			string text = (currentDifficulty != 0) ? Manager.Get<TranslationsManager>().GetText("dance.faster", "FASTER!").ToUpper() : Manager.Get<TranslationsManager>().GetText("dance.getready", "GET READY!").ToUpper();
			base.connector.getReadyText.text = text;
			RhythmicMinigameConfigBehaviour.SequencesList sequencesList = configBehaviour.sequencesPerDifficulty[currentDifficulty];
			sequencesEnumerator = Enumerators.RandomButNotPrevious(sequencesList.list);
			rhythmController.SetTempoAndClip(sequencesList.tempo, sequencesList.musicClip);
			graphicScene.SetTempo(sequencesList.tempo / 2f);
			currentSequence = emptySequence;
			base.connector.getReadyVisible = true;
			levelProgress = 0f;
			levelProgressMax = configBehaviour.sequencesPerDifficulty[currentDifficulty].progressToFinish;
		}

		private void NextSequenceInCurrentDifficulty()
		{
			sequencesEnumerator.MoveNext();
			currentSequence = sequencesEnumerator.Current;
			base.connector.getReadyVisible = false;
		}

		public override void UpdateState()
		{
			base.UpdateState();
			rhythmController.Update();
			graphicScene.Update();
			if (base.connector != null && base.connector.tamePanel != null)
			{
				base.connector.tamePanel.Refresh(levelProgress, levelProgressMax, tamed: true);
			}
			if (base.connector != null)
			{
				base.connector.helpInfoVisible = (tutorial && rhythmController.paused);
			}
			if (tutorial && rhythmController.paused && !blobsController.IsFirstBlobInPerfectPlace())
			{
				rhythmController.Play();
			}
		}

		public override void FinishState()
		{
			rhythmController.Dispose();
			graphicScene.OnFinish();
			if (anyBlobHit)
			{
				MonoBehaviourSingleton<ProgressCounter>.get.Increment(ProgressCounter.Countables.MINIGAMES_PLAYED);
			}
			base.FinishState();
			PlayerBetterMovement component = PlayerGraphic.GetControlledPlayerInstance().GetComponent<PlayerBetterMovement>();
			if (component != null)
			{
				component.ChangeMoveMode(PlayerMovement.Mode.WALKING);
			}
		}

		private void OnReturn()
		{
			CameraController.instance.SetDefaultCameraPreset();
			PlayerGraphic.GetControlledPlayerInstance().mainBody.transform.localEulerAngles = Vector3.zero;
			PlayerGraphic.GetControlledPlayerInstance().ShowPlayerGraphic();
			PlayerGraphic.GetControlledPlayerInstance().GetComponent<PlayerMovement>().UpdateCurrentMode();
			Manager.Get<StateMachineManager>().PopState();
		}

		public void OnBeat()
		{
			base.connector.OnBeat(nextBeatIndex);
			graphicScene.OnBeat(nextBeatIndex);
			nextBeatIndex++;
			if ((float)nextBeatIndex >= (float)currentSequence.beats.Length * 2f)
			{
				NextSequence();
			}
			if (tutorial && !rhythmController.paused && blobsController.IsFirstBlobInPerfectPlace())
			{
				rhythmController.Pause();
			}
		}

		private void OnTap()
		{
			if ((tutorial && !blobsController.IsFirstBlobInPerfectPlace()) || waitingToReturn || !(Time.realtimeSinceStartup > tapCooldownEndTime))
			{
				return;
			}
			switch (blobsController.TryToTap())
			{
			case RhythmBlobsController.TapEffect.TAP_CORRECT:
				correctBitsInSequenceCounter++;
				levelProgress += 1f;
				tapCooldownEndTime = Time.realtimeSinceStartup + 0.2f;
				if (currentSequence != null && correctBitsInSequenceCounter == (from s in currentSequence.beats
					where s > 0
					select s).Count())
				{
					OnCorrectSequence();
					levelProgress += 1f;
				}
				break;
			case RhythmBlobsController.TapEffect.TAP_INCORRECT:
				tapCooldownEndTime = Time.realtimeSinceStartup + 0.2f;
				levelProgress = Mathf.Max(0f, levelProgress - 0.75f);
				break;
			}
		}

		public void CorrectHitEffect(int beatType)
		{
			graphicScene.OnCorrectHit(beatType);
			base.connector.OnCorrectHit(beatType);
			anyBlobHit = true;
		}

		public void IncorrectHitEffect(int beatType)
		{
			base.connector.OnIncorrectHit(beatType);
			graphicScene.OnIncorrectHit(beatType);
			base.connector.hp--;
			if (base.connector.hp <= 0)
			{
				StartFailureScene();
			}
		}

		public void OnBlobFall(int beatType)
		{
			base.connector.OnBlobFall(beatType);
			graphicScene.OnBlobFall(beatType);
			base.connector.hp--;
			if (base.connector.hp <= 0)
			{
				StartFailureScene();
			}
		}

		public void OnSpawnBlob(int beatType)
		{
			base.connector.OnSpawnBlob(beatType);
			graphicScene.OnSpawnBlob(beatType);
		}

		public void OnCorrectSequence()
		{
			base.connector.OnCorrectSequence();
			graphicScene.OnCorrectSequence();
		}

		public void OnBlobInShootZone(int beatType)
		{
			base.connector.OnBlobInShootZone(beatType);
			graphicScene.OnBlobInShootZone(beatType);
		}

		[ContextMenu("Start success scene")]
		private void StartSuccessScene()
		{
			rhythmController.Pause();
			base.connector.OnSuccessScene();
			graphicScene.OnSuccessScene();
			StartCoroutine(ReturnAfter(3f));
			base.connector.getReadyText.text = Manager.Get<TranslationsManager>().GetText("dance.success", "SUCCESS!").ToUpper();
			Manager.Get<StatsManager>().MinigameFinished(startParameter.graphicScene.GetGameType(), success: true);
			if (startParameter.onWin != null)
			{
				startParameter.onWin();
			}
		}

		private void StartFailureScene()
		{
			rhythmController.Pause();
			base.connector.OnFailureScene();
			graphicScene.OnFailureScene();
			StartCoroutine(ReturnAfter(3f));
			base.connector.getReadyText.text = Manager.Get<TranslationsManager>().GetText("dance.failure", "FAILED!").ToUpper();
			Manager.Get<StatsManager>().MinigameFinished(startParameter.graphicScene.GetGameType(), success: false);
			if (startParameter.onFail != null)
			{
				startParameter.onFail();
			}
		}

		private IEnumerator ReturnAfter(float duration)
		{
			waitingToReturn = true;
			yield return new WaitForSecondsRealtime(duration);
			OnReturn();
			waitingToReturn = false;
		}
	}
}
