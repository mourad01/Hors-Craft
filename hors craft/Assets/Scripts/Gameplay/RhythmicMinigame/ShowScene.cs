// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RhythmicMinigame.ShowScene
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using Gameplay.Minigames;
using GameUI;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace Gameplay.RhythmicMinigame
{
	public class ShowScene : RhythmReactor
	{
		private RuntimeAnimatorController playerOldAnimatorController;

		private Dictionary<Mob, RuntimeAnimatorController> mobsOldAnimatorControllers;

		private Animator playerAnimator;

		private List<HumanMob> _mobs;

		private List<GameObject> spawnedInstruments;

		private ParticleSystem[] botHeartComboParticles;

		private Transform cameraAndHumanPivot;

		private string[] correctTriggers = new string[3]
		{
			"correct1",
			"correct2",
			"correct3"
		};

		private string wrongTrigger = "fail";

		private string beatTrigger = "rhythm";

		private Vector3 destCameraPosition;

		private Quaternion destCameraRotation;

		private HumanRepresentation playerRepresentation;

		private Vector3 baseCameraPosition;

		private Vector3 baseCameraRotation;

		private AudioClip[] successClips;

		private Vector3 heartsParticlesOffset = new Vector3(0.25f, 1f, 0f);

		private GameObject[] gosToHideInGame;

		private Camera mainCamera;

		private GameObject rememberedOverlayCamera;

		private bool lastCameraLookOnPlayer;

		private List<HumanMob> mobs
		{
			get
			{
				_mobs = (from m in _mobs
					where m != null && m.gameObject != null
					select m).ToList();
				return _mobs;
			}
			set
			{
				_mobs = value;
			}
		}

		public ShowScene(PlayerGraphic playerGraphic, InstrumentConfig[] instrumentConfigs, RuntimeAnimatorController newPlayerAnimatorController, Transform humanPivot, HashSet<HumanMob> mobs, AudioClip[] successClips, GameObject[] gosToHideInGame)
		{
			playerAnimator = playerGraphic.graphicRepresentation.GetComponent<Animator>();
			this.mobs = new List<HumanMob>(mobs);
			cameraAndHumanPivot = humanPivot;
			this.successClips = successClips;
			mainCamera = CameraController.instance.MainCamera;
			PrepareParticles();
			playerGraphic.GetComponent<PlayerMovement>().EnableMovement(enable: false);
			playerRepresentation = new HumanRepresentation(playerGraphic);
			ReplacedAnimatorsOn(newPlayerAnimatorController);
			SpawnInstruments(instrumentConfigs);
			CameraOnMobs();
			this.gosToHideInGame = gosToHideInGame;
			foreach (GameObject gameObject in gosToHideInGame)
			{
				gameObject.SetActive(value: false);
			}
		}

		private void PrepareParticles()
		{
			GameObject original = Resources.Load<GameObject>("prefabs/dance_heart_combo");
			botHeartComboParticles = new ParticleSystem[mobs.Count];
			int num = 0;
			foreach (HumanMob mob in mobs)
			{
				GameObject gameObject = Object.Instantiate(original);
				botHeartComboParticles[num] = gameObject.GetComponent<ParticleSystem>();
				botHeartComboParticles[num].transform.position = mob.transform.position + heartsParticlesOffset;
				botHeartComboParticles[num].transform.SetLayerRecursively(LayerMask.GetMask("Default"));
				num++;
			}
		}

		public void OnFinish()
		{
			GameObject[] array = gosToHideInGame;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(value: true);
			}
			DestroyParticles();
			DestroyInstruments();
			ReplacedAnimatorsOff();
			playerAnimator.transform.localEulerAngles = Vector3.zero;
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
		}

		private void DestroyInstruments()
		{
			foreach (GameObject spawnedInstrument in spawnedInstruments)
			{
				UnityEngine.Object.Destroy(spawnedInstrument);
			}
		}

		private void DestroyParticles()
		{
			ParticleSystem[] array = botHeartComboParticles;
			foreach (ParticleSystem particleSystem in array)
			{
				UnityEngine.Object.Destroy(particleSystem.gameObject);
			}
		}

		private void ReplacedAnimatorsOn(RuntimeAnimatorController newPlayerAnimatorController)
		{
			playerRepresentation.UIModeOn(delegate(GameObject parent)
			{
				parent.transform.position = cameraAndHumanPivot.position;
				parent.transform.rotation = cameraAndHumanPivot.rotation;
			}, setLayerToClothes: false);
			playerOldAnimatorController = playerAnimator.runtimeAnimatorController;
			playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
			playerAnimator.runtimeAnimatorController = newPlayerAnimatorController;
			List<RuntimeAnimatorController> list = new List<RuntimeAnimatorController>();
			list.Add(Resources.Load<RuntimeAnimatorController>("prefabs/show_crowV1_animator"));
			list.Add(Resources.Load<RuntimeAnimatorController>("prefabs/show_crowV2_animator"));
			list.Add(Resources.Load<RuntimeAnimatorController>("prefabs/show_crowV3_animator"));
			mobsOldAnimatorControllers = new Dictionary<Mob, RuntimeAnimatorController>();
			foreach (HumanMob mob in mobs)
			{
				mobsOldAnimatorControllers.Add(mob, mob.animator.runtimeAnimatorController);
				mob.animator.runtimeAnimatorController = list[Random.Range(0, list.Count)];
				mob.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
				Vector3 eulerAngles = Quaternion.LookRotation(playerAnimator.transform.position - mob.transform.position).eulerAngles;
				Transform transform = mob.transform;
				Vector3 eulerAngles2 = mob.transform.eulerAngles;
				float x = eulerAngles2.x;
				float y = eulerAngles.y;
				Vector3 eulerAngles3 = mob.transform.eulerAngles;
				transform.eulerAngles = new Vector3(x, y, eulerAngles3.z);
			}
			baseCameraPosition = mainCamera.transform.parent.position;
			baseCameraRotation = mainCamera.transform.parent.eulerAngles;
		}

		private void ReplacedAnimatorsOff()
		{
			playerAnimator.runtimeAnimatorController = playerOldAnimatorController;
			playerAnimator.updateMode = AnimatorUpdateMode.Normal;
			playerAnimator.SetTrigger("locomotionChange");
			foreach (HumanMob mob in mobs)
			{
				mob.animator.runtimeAnimatorController = mobsOldAnimatorControllers[mob];
				mob.animator.updateMode = AnimatorUpdateMode.Normal;
			}
			mainCamera.transform.parent.position = baseCameraPosition;
			mainCamera.transform.parent.eulerAngles = baseCameraRotation;
			playerRepresentation.UIModeOff();
		}

		private void SpawnInstruments(InstrumentConfig[] configs)
		{
			spawnedInstruments = new List<GameObject>();
			if (configs != null)
			{
				foreach (InstrumentConfig instrumentConfig in configs)
				{
					Transform parent = playerAnimator.transform.FindChildRecursively(instrumentConfig.boneName);
					GameObject gameObject = Object.Instantiate(instrumentConfig.prefab);
					spawnedInstruments.Add(gameObject);
					gameObject.transform.SetParent(parent, worldPositionStays: false);
				}
			}
			playerAnimator.gameObject.SetActive(value: false);
			playerAnimator.gameObject.SetActive(value: true);
		}

		private void CameraOnPlayer()
		{
			CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
			LookAtTargetMotor lookAtTargetMotor = (LookAtTargetMotor)CameraController.instance.ActiveMotor;
			lookAtTargetMotor.Target = playerAnimator.transform;
			lookAtTargetMotor.Offset = 1.75f * playerAnimator.transform.forward + new Vector3(0f, 0f, -0.2f);
			lookAtTargetMotor.TargetOffset = new Vector3(0f, 0f, 0f);
			lookAtTargetMotor.UseCurrentPosition = false;
		}

		private void CameraOnMobs()
		{
			CameraController.instance.SetCameraPreset(CameraController.CameraPresets.LookAt);
			LookAtTargetMotor lookAtTargetMotor = (LookAtTargetMotor)CameraController.instance.ActiveMotor;
			lookAtTargetMotor.Target = playerAnimator.transform;
			lookAtTargetMotor.Offset = -2f * playerAnimator.transform.forward + new Vector3(0f, 0f, 0.2f);
			lookAtTargetMotor.TargetOffset = new Vector3(0f, 0f, 0f);
			lookAtTargetMotor.UseCurrentPosition = false;
		}

		public void Update()
		{
			mainCamera.transform.parent.position = Vector3.Lerp(mainCamera.transform.parent.position, destCameraPosition, 0.2f);
			mainCamera.transform.parent.rotation = Quaternion.Lerp(mainCamera.transform.parent.rotation, destCameraRotation, 0.2f);
			rememberedOverlayCamera.transform.position = mainCamera.transform.position;
			rememberedOverlayCamera.transform.rotation = mainCamera.transform.rotation;
		}

		public void SetPosition(Camera camera)
		{
			rememberedOverlayCamera = camera.gameObject;
			rememberedOverlayCamera.SetActive(value: false);
		}

		public void OnCorrectHit(int beatType)
		{
			playerAnimator.ResetTrigger(beatTrigger);
			playerAnimator.SetTrigger(correctTriggers[(beatType - 1) % correctTriggers.Length]);
			foreach (HumanMob mob in mobs)
			{
				mob.animator.ResetTrigger(beatTrigger);
				mob.animator.SetTrigger(correctTriggers[(beatType - 1) % correctTriggers.Length]);
				if (mob.pettable != null)
				{
					(mob.pettable as PettableHuman).DanceTame(mob.transform, heartsParticlesOffset, "Mobs");
				}
			}
			PlaySound(successClips[beatType % successClips.Length], BeatTypeToPitch(beatType));
		}

		private float BeatTypeToPitch(int beatType)
		{
			return 0.85f + 0.1f * (float)beatType;
		}

		public void OnIncorrectHit(int beatType)
		{
			playerAnimator.SetTrigger(wrongTrigger);
			foreach (HumanMob mob in mobs)
			{
				mob.animator.SetTrigger(wrongTrigger);
				if (mob.pettable != null)
				{
					(mob.pettable as PettableHuman).DanceFail();
				}
			}
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_WRONG));
		}

		public void OnBlobFall(int beatType)
		{
			OnIncorrectHit(beatType);
		}

		public void OnCorrectSequence()
		{
			for (int i = 0; i < botHeartComboParticles.Length; i++)
			{
				botHeartComboParticles[i].Stop();
				botHeartComboParticles[i].startDelay = 0.4f;
				botHeartComboParticles[i].Play();
				if (mobs[i].pettable != null)
				{
					(mobs[i].pettable as PettableHuman).DanceTame(mobs[i].transform, heartsParticlesOffset, "Mobs");
				}
			}
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_CHEER));
		}

		public void OnSpawnBlob(int beatType)
		{
			foreach (HumanMob mob in mobs)
			{
				mob.animator.ResetTrigger(beatTrigger);
				mob.animator.SetTrigger(correctTriggers[(beatType - 1) % correctTriggers.Length]);
			}
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_GOOD), BeatTypeToPitch(beatType));
		}

		public void OnBeat(int beatIndex)
		{
			if (IsDanceAnimatorIdle(playerAnimator))
			{
				playerAnimator.SetTrigger(beatTrigger);
			}
			foreach (HumanMob mob in mobs)
			{
				if (IsDanceAnimatorIdle(mob.animator))
				{
					mob.animator.SetTrigger(beatTrigger);
				}
			}
			UnityEngine.Debug.LogWarning("Beat Index: " + beatIndex);
			if (!lastCameraLookOnPlayer && beatIndex >= 8)
			{
				CameraOnPlayer();
				lastCameraLookOnPlayer = true;
			}
			else if (lastCameraLookOnPlayer && beatIndex >= 15)
			{
				CameraOnMobs();
				lastCameraLookOnPlayer = false;
			}
		}

		private bool IsDanceAnimatorIdle(Animator animator)
		{
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			return !animator.IsInTransition(0) && currentAnimatorStateInfo.normalizedTime > 0.9f;
		}

		private string PickRandomString(string[] array)
		{
			return array[Random.Range(0, array.Length)];
		}

		private void PlaySound(AudioClip clip, float pitch = 1f)
		{
			Sound sound = new Sound();
			sound.clip = clip;
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().musicMixerGroup;
			sound.pitch = pitch;
			sound.volume = 1f;
			Sound sound2 = sound;
			sound2.Play();
		}

		public void SetTempo(float tempo)
		{
			playerAnimator.speed = tempo;
			foreach (HumanMob mob in mobs)
			{
				mob.animator.speed = tempo;
			}
		}

		public void OnSuccessScene()
		{
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_CHEER));
			foreach (HumanMob mob in mobs)
			{
				mob.animator.SetTrigger("success");
			}
			rememberedOverlayCamera.SetActive(value: false);
		}

		public void OnFailureScene()
		{
			foreach (HumanMob mob in mobs)
			{
				mob.animator.SetTrigger("failure");
			}
			rememberedOverlayCamera.SetActive(value: false);
		}

		public void OnBlobInShootZone(int beatType)
		{
		}

		public StatsManager.MinigameType GetGameType()
		{
			return StatsManager.MinigameType.INSTRUMENTS;
		}
	}
}
