// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RhythmicMinigame.PonyDanceScene
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
	public class PonyDanceScene : RhythmReactor
	{
		public struct DancePathes
		{
			public const string defaultPlayerAnimatorPath = "prefabs/dance_player_animator";

			public const string defaultBotAnimatorPath = "prefabs/dance_mob_animator";

			public const string defaultBallPath = "prefabs/dance_disco_ball";

			public const string defaultHeartsPath = "prefabs/dance_heart_1";

			public const string defaultHeartsComboPath = "prefabs/dance_heart_combo";

			public string playerAnimatorPath;

			public string botAnimatorPath;

			public string discoBallPath;

			public string heartsPath;

			public string heartComboPath;

			public static DancePathes Default => new DancePathes("prefabs/dance_mob_animator");

			public DancePathes(string bothPath)
			{
				playerAnimatorPath = bothPath;
				botAnimatorPath = bothPath;
				discoBallPath = "prefabs/dance_disco_ball";
				heartsPath = "prefabs/dance_heart_1";
				heartComboPath = "prefabs/dance_heart_combo";
			}

			public DancePathes(string playerPath, string botPath)
			{
				playerAnimatorPath = playerPath;
				botAnimatorPath = botPath;
				discoBallPath = "prefabs/dance_disco_ball";
				heartsPath = "prefabs/dance_heart_1";
				heartComboPath = "prefabs/dance_heart_combo";
			}

			public DancePathes(string playerPath, string botPath, string ballPath)
			{
				playerAnimatorPath = playerPath;
				botAnimatorPath = botPath;
				discoBallPath = ballPath;
				heartsPath = "prefabs/dance_heart_1";
				heartComboPath = "prefabs/dance_heart_combo";
			}

			public DancePathes(string playerPath, string botPath, string ballPath, string heatsPath)
			{
				playerAnimatorPath = playerPath;
				botAnimatorPath = botPath;
				discoBallPath = ballPath;
				heartsPath = heatsPath;
				heartComboPath = "prefabs/dance_heart_combo";
			}

			public DancePathes(string playerPath, string botPath, string ballPath, string heatsPath, string heartComboPath)
			{
				playerAnimatorPath = playerPath;
				botAnimatorPath = botPath;
				discoBallPath = ballPath;
				heartsPath = heatsPath;
				this.heartComboPath = heartComboPath;
			}
		}

		public struct DanceOffsets
		{
			public static readonly Vector3 defaultPlayerOffset = new Vector3(-1.4f, -0.2f, 3.4f);

			public static readonly Vector3 defaultMobOffset = new Vector3(1.4f, -0.2f, 3.4f);

			public static readonly Vector3 defaultDiscoBallOffset = new Vector3(0f, 1.2f, 3.5f);

			public Vector3 playerOffset;

			public Vector3 mobOffset;

			public Vector3 discoBallOffset;

			public static DanceOffsets Default => new DanceOffsets(defaultPlayerOffset);

			public DanceOffsets(Vector3 playerOffset)
			{
				this.playerOffset = playerOffset;
				mobOffset = defaultMobOffset;
				discoBallOffset = defaultDiscoBallOffset;
			}

			public DanceOffsets(Vector3 playerOffset, Vector3 mobOffset)
			{
				this.playerOffset = playerOffset;
				this.mobOffset = mobOffset;
				discoBallOffset = defaultDiscoBallOffset;
			}

			public DanceOffsets(Vector3 playerOffset, Vector3 mobOffset, Vector3 discoBallOffset)
			{
				this.playerOffset = playerOffset;
				this.mobOffset = mobOffset;
				this.discoBallOffset = discoBallOffset;
			}
		}

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

		private Vector3 playerOffset;

		private Vector3 mobOffset;

		private Vector3 discoBallOffset;

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

		public PonyDanceScene(PlayerGraphic playerGraphic, PlayerGraphic mobGraphic, Pettable mobPettable, DancePathes pathes, DanceOffsets offsets)
		{
			playerOffset = offsets.playerOffset;
			mobOffset = offsets.mobOffset;
			discoBallOffset = offsets.discoBallOffset;
			playerHumanRep = new HumanRepresentation(playerGraphic);
			playerGraphic.ShowBodyAndLegs();
			botHumanRep = new HumanRepresentation(mobGraphic, mobPettable);
			playerDanceAnimator = Resources.Load<RuntimeAnimatorController>(pathes.playerAnimatorPath);
			botDanceAnimator = Resources.Load<RuntimeAnimatorController>(pathes.botAnimatorPath);
			oldPlayerSpeed = playerAnimator.speed;
			oldMobSpeed = botAnimator.speed;
			discoBall = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(pathes.discoBallPath));
			discoBallAnimator = discoBall.GetComponentInChildren<Animator>();
			discoBallAnimator.enabled = true;
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(pathes.heartsPath));
			botHeartParticles = gameObject.GetComponent<ParticleSystem>();
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(pathes.heartComboPath));
			botHeartComboParticles = gameObject2.GetComponent<ParticleSystem>();
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: false);
		}

		public StatsManager.MinigameType GetGameType()
		{
			return StatsManager.MinigameType.DANCE;
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

		public void OnBlobFall(int beatType)
		{
			OnIncorrectHit(beatType);
		}

		public void OnBlobInShootZone(int beatType)
		{
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

		public void OnFailureScene()
		{
			playerAnimator.SetTrigger("failure");
			botAnimator.SetTrigger("failure");
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

		public void OnIncorrectHit(int beatType)
		{
			playerAnimator.SetTrigger(wrongTrigger);
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_WRONG));
			if (botHumanRep.pettable is PettableHuman)
			{
				(botHumanRep.pettable as PettableHuman).DanceFail();
			}
		}

		public void OnSpawnBlob(int beatType)
		{
			botAnimator.ResetTrigger(beatTrigger);
			botAnimator.SetTrigger(correctTriggers[(beatType - 1) % correctTriggers.Length]);
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_GOOD), BeatTypeToPitch(beatType));
		}

		private float BeatTypeToPitch(int beatType)
		{
			return 0.85f + 0.1f * (float)beatType;
		}

		public void OnSuccessScene()
		{
			playerAnimator.SetTrigger("success");
			botAnimator.SetTrigger("success");
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_CHEER));
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
			playerAnimator.runtimeAnimatorController = playerDanceAnimator;
			botAnimator.runtimeAnimatorController = botDanceAnimator;
			Animator animator = playerAnimator;
			AnimatorUpdateMode updateMode = AnimatorUpdateMode.UnscaledTime;
			botAnimator.updateMode = updateMode;
			animator.updateMode = updateMode;
			botScale = botHumanRep.graphic.graphicRepresentation.transform.localScale;
			playerScale = playerHumanRep.graphic.graphicRepresentation.transform.localScale;
		}

		public void SetTempo(float tempo)
		{
			botAnimator.speed = tempo;
			playerAnimator.speed = tempo;
			discoBallAnimator.speed = tempo;
		}

		public void Update()
		{
		}

		private void PlaySound(AudioClip clip, float pitch = 1f)
		{
			Sound sound = new Sound();
			sound.clip = clip;
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			sound.pitch = pitch;
			sound.volume = 1f;
			Sound sound2 = sound;
			sound2.Play();
		}
	}
}
