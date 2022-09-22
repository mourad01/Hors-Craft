// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RhythmicMinigame.DanceScene
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using GameUI;
using System;
using Uniblocks;
using UnityEngine;

namespace Gameplay.RhythmicMinigame
{
	public class DanceScene : RhythmReactor
	{
		private RuntimeAnimatorController playerDanceAnimator;

		private RuntimeAnimatorController botDanceAnimator;

		private RuntimeAnimatorController playerOldAnimator;

		private RuntimeAnimatorController botOldAnimator;

		private HumanRepresentation playerHumanRep;

		private HumanRepresentation botHumanRep;

		private Animator playerAnimator;

		private Animator botAnimator;

		private float oldPlayerSpeed;

		private float oldMobSpeed;

		private Vector3 playerOffset = new Vector3(-1.4f, -0.2f, 3.4f);

		private Vector3 mobOffset = new Vector3(1.4f, -0.2f, 3.4f);

		private Vector3 discoBallOffset = new Vector3(0f, 1.2f, 3.5f);

		private Vector3 botScale;

		private Vector3 playerScale;

		private GameObject discoBall;

		private Animator discoBallAnimator;

		private ParticleSystem botHeartParticles;

		private ParticleSystem botHeartComboParticles;

		private string[] correctTriggers = new string[3]
		{
			"correct1",
			"correct2",
			"correct3"
		};

		private string wrongTrigger = "fail";

		private string beatTrigger = "rhythm";

		public DanceScene(PlayerGraphic playerGraphic, PlayerGraphic mobGraphic, Pettable mobPettable)
		{
			playerHumanRep = new HumanRepresentation(playerGraphic);
			playerGraphic.ShowBodyAndLegs();
			botHumanRep = new HumanRepresentation(mobGraphic, mobPettable);
			botDanceAnimator = Resources.Load<RuntimeAnimatorController>("prefabs/dance_mob_animator");
			playerDanceAnimator = Resources.Load<RuntimeAnimatorController>("prefabs/dance_player_animator");
			discoBall = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/dance_disco_ball"));
			discoBallAnimator = discoBall.GetComponentInChildren<Animator>();
			discoBallAnimator.enabled = true;
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/dance_heart_1"));
			botHeartParticles = gameObject.GetComponent<ParticleSystem>();
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/dance_heart_combo"));
			botHeartComboParticles = gameObject2.GetComponent<ParticleSystem>();
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: false);
		}

		public void Update()
		{
		}

		public void SetPosition(Camera camera)
		{
			UIRepresentationsOn(camera);
			botHeartParticles.transform.position = botHumanRep.graphic.transform.position + Vector3.up * 1.25f;
			botHeartComboParticles.transform.position = botHumanRep.graphic.transform.position + Vector3.up * 1.25f;
			discoBall.transform.position = camera.transform.position + discoBallOffset;
			discoBall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			discoBall.transform.SetLayerRecursively(LayerMask.NameToLayer("ClothesPreview"));
			discoBall.GetComponentInChildren<DiscoLight>().enabled = false;
		}

		private void UIRepresentationsOn(Camera camera)
		{
			Action<GameObject> setMobRepresentationPlace = delegate(GameObject go)
			{
				go.transform.position = camera.transform.position + playerOffset;
				go.transform.localRotation = Quaternion.Euler(0f, 143f, 0f);
			};
			Action<GameObject> setMobRepresentationPlace2 = delegate(GameObject go)
			{
				go.transform.position = camera.transform.position + mobOffset;
				go.transform.localRotation = Quaternion.Euler(0f, 217f, 0f);
			};
			playerHumanRep.UIModeOn(setMobRepresentationPlace);
			botHumanRep.UIModeOn(setMobRepresentationPlace2);
			playerAnimator = playerHumanRep.graphic.graphicRepresentation.GetComponent<Animator>();
			botAnimator = botHumanRep.graphic.graphicRepresentation.GetComponent<Animator>();
			playerOldAnimator = playerAnimator.runtimeAnimatorController;
			botOldAnimator = botAnimator.runtimeAnimatorController;
			oldPlayerSpeed = playerAnimator.speed;
			oldMobSpeed = botAnimator.speed;
			playerAnimator.runtimeAnimatorController = playerDanceAnimator;
			botAnimator.runtimeAnimatorController = botDanceAnimator;
			Animator animator = playerAnimator;
			AnimatorUpdateMode updateMode = AnimatorUpdateMode.UnscaledTime;
			botAnimator.updateMode = updateMode;
			animator.updateMode = updateMode;
			botScale = botHumanRep.graphic.graphicRepresentation.transform.localScale;
			playerScale = playerHumanRep.graphic.graphicRepresentation.transform.localScale;
		}

		private void UIRepresentationsOff()
		{
			Animator animator = playerAnimator;
			AnimatorUpdateMode updateMode = AnimatorUpdateMode.Normal;
			botAnimator.updateMode = updateMode;
			animator.updateMode = updateMode;
			playerAnimator.runtimeAnimatorController = playerOldAnimator;
			botAnimator.runtimeAnimatorController = botOldAnimator;
			botHumanRep.graphic.graphicRepresentation.transform.localScale = botScale;
			playerHumanRep.graphic.graphicRepresentation.transform.localScale = playerScale;
			playerHumanRep.UIModeOff();
			botHumanRep.UIModeOff();
		}

		public void OnCorrectHit(int beatType)
		{
			playerAnimator.ResetTrigger(beatTrigger);
			playerAnimator.SetTrigger(correctTriggers[(beatType - 1) % correctTriggers.Length]);
			botAnimator.ResetTrigger(beatTrigger);
			botAnimator.SetTrigger(correctTriggers[(beatType - 1) % correctTriggers.Length]);
			botHeartParticles.Stop();
			botHeartParticles.Play();
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_GOOD), BeatTypeToPitch(beatType));
		}

		private float BeatTypeToPitch(int beatType)
		{
			return 0.85f + 0.1f * (float)beatType;
		}

		public void OnIncorrectHit(int beatType)
		{
			playerAnimator.SetTrigger(wrongTrigger);
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_WRONG));
			if (botHumanRep.pettable is PettableHuman)
			{
				(botHumanRep.pettable as PettableHuman).DanceFail();
			}
		}

		public void OnBlobFall(int beatType)
		{
			OnIncorrectHit(beatType);
		}

		public void OnCorrectSequence()
		{
			if (botHumanRep.pettable is PettableHuman)
			{
				(botHumanRep.pettable as PettableHuman).DanceTame(botHumanRep.graphic.transform.parent, new Vector3(0f, 0.8f, 0f), "ClothesPreview");
			}
			botHeartComboParticles.Stop();
			botHeartComboParticles.Play();
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_CHEER));
		}

		public void OnSpawnBlob(int beatType)
		{
			botAnimator.ResetTrigger(beatTrigger);
			botAnimator.SetTrigger(correctTriggers[(beatType - 1) % correctTriggers.Length]);
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_GOOD), BeatTypeToPitch(beatType));
		}

		public void OnBeat(int beatIndex)
		{
			if (IsDanceAnimatorIdle(playerAnimator))
			{
				playerAnimator.SetTrigger(beatTrigger);
			}
			if (IsDanceAnimatorIdle(botAnimator))
			{
				botAnimator.SetTrigger(beatTrigger);
			}
			discoBallAnimator.SetTrigger("Bump");
		}

		private bool IsDanceAnimatorIdle(Animator animator)
		{
			return !animator.IsInTransition(0);
		}

		public void OnFinish()
		{
			UIRepresentationsOff();
			playerAnimator.speed = oldPlayerSpeed;
			botAnimator.speed = oldMobSpeed;
			UnityEngine.Object.Destroy(discoBall);
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<AbstractAchievementManager>().RegisterEvent("npc.dance");
			}
		}

		private string PickRandomString(string[] array)
		{
			return array[UnityEngine.Random.Range(0, array.Length)];
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
			botAnimator.speed = tempo;
			playerAnimator.speed = tempo;
			discoBallAnimator.speed = tempo;
		}

		public void OnSuccessScene()
		{
			playerAnimator.SetTrigger("success");
			botAnimator.SetTrigger("success");
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_CHEER));
		}

		public void OnFailureScene()
		{
			playerAnimator.SetTrigger("failure");
			botAnimator.SetTrigger("failure");
		}

		public void OnBlobInShootZone(int beatType)
		{
		}

		public StatsManager.MinigameType GetGameType()
		{
			return StatsManager.MinigameType.DANCE;
		}
	}
}
