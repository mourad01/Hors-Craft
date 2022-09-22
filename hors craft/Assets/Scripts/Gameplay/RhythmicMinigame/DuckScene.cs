// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RhythmicMinigame.DuckScene
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using Uniblocks;
using UnityEngine;

namespace Gameplay.RhythmicMinigame
{
	public class DuckScene : RhythmReactor
	{
		public struct DucksConfiguration
		{
			public GameObject shotgun;

			public GameObject shootRange;

			public GameObject[] ducks;

			public GameObject hitParticles;

			public GameObject missParticles;
		}

		private DucksConfiguration config;

		private Vector3 missParticlesOffset = new Vector3(0f, 1.4f, 0f);

		private Vector3 hitParticlesOffset = new Vector3(-0.05f, 0.5f, -0.05f);

		private Vector3 shootRangeOffset = new Vector3(0f, -1.08f, 3.7f);

		private Vector3 shotgunOffset = new Vector3(0f, -0.33f, 0.44f);

		private Vector3 shotgunRotation = new Vector3(8.23f, 8.35f, 0f);

		private Vector3 oldPosition;

		private Quaternion oldRotation;

		private int oldLayer;

		public DuckScene(DucksConfiguration config)
		{
			this.config = config;
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: false);
			oldPosition = config.shootRange.transform.position;
			oldRotation = config.shootRange.transform.rotation;
			oldLayer = config.shootRange.layer;
			GameObject[] ducks = config.ducks;
			foreach (GameObject gameObject in ducks)
			{
				ResetTriggers(gameObject);
				gameObject.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
				gameObject.GetComponent<Animator>().SetTrigger("shoot");
			}
		}

		public void Update()
		{
		}

		public void SetPosition(Camera camera)
		{
			config.shootRange.transform.position = camera.transform.position + shootRangeOffset;
			config.shootRange.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			config.shootRange.transform.SetLayerRecursively(LayerMask.NameToLayer("ClothesPreview"));
			config.shotgun = Object.Instantiate(config.shotgun);
			config.shotgun.transform.SetLayerRecursively(LayerMask.NameToLayer("ClothesPreview"));
			config.shotgun.GetComponentInChildren<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
			config.shotgun.transform.position = camera.transform.position + shotgunOffset;
			config.shotgun.transform.rotation = Quaternion.Euler(shotgunRotation);
		}

		public void OnCorrectHit(int beatType)
		{
			ResetTriggers(config.ducks[beatType - 1]);
			config.ducks[beatType - 1].GetComponent<Animator>().SetTrigger("shoot");
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.RIFLE_SHOOT), BeatTypeToPitch(beatType), 0.8f);
			SpawnHitParticles(beatType);
			config.shotgun.GetComponentInChildren<Animator>().SetTrigger("shot");
			config.shotgun.transform.rotation = Quaternion.Euler(shotgunRotation + (-9f + (float)(beatType - 1) * 9f) * Vector3.up);
		}

		private float BeatTypeToPitch(int beatType)
		{
			return 0.85f + 0.1f * (float)beatType;
		}

		public void OnIncorrectHit(int beatType)
		{
			ResetTriggers(config.ducks[beatType - 1]);
			config.ducks[beatType - 1].GetComponent<Animator>().SetTrigger("hide");
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.RIFLE_SHOOT), 1f, 0.8f);
			SpawnMissParticles(beatType);
			config.shotgun.GetComponentInChildren<Animator>().SetTrigger("shot");
		}

		public void OnBlobFall(int beatType)
		{
			ResetTriggers(config.ducks[beatType - 1]);
			config.ducks[beatType - 1].GetComponent<Animator>().SetTrigger("hide");
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_WRONG));
		}

		public void OnCorrectSequence()
		{
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_CHEER));
		}

		public void OnSpawnBlob(int beatType)
		{
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DUCK_SOUND), BeatTypeToPitch(beatType));
		}

		public void OnBeat(int beatIndex)
		{
		}

		public void OnFinish()
		{
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().EnableMovement(enable: true);
			config.shootRange.transform.position = oldPosition;
			config.shootRange.transform.rotation = oldRotation;
			config.shootRange.transform.SetLayerRecursively(oldLayer);
			GameObject[] ducks = config.ducks;
			foreach (GameObject gameObject in ducks)
			{
				ResetTriggers(gameObject);
				gameObject.GetComponent<Animator>().updateMode = AnimatorUpdateMode.Normal;
				gameObject.GetComponent<Animator>().SetTrigger("reset");
			}
			UnityEngine.Object.Destroy(config.shotgun);
		}

		private void PlaySound(AudioClip clip, float pitch = 1f, float volume = 1f)
		{
			Sound sound = new Sound();
			sound.clip = clip;
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().musicMixerGroup;
			sound.pitch = pitch;
			sound.volume = volume;
			Sound sound2 = sound;
			sound2.Play();
		}

		public void SetTempo(float tempo)
		{
			GameObject[] ducks = config.ducks;
			foreach (GameObject gameObject in ducks)
			{
				gameObject.GetComponent<Animator>().speed = tempo;
			}
			config.shotgun.GetComponentInChildren<Animator>().speed = tempo;
		}

		public void OnSuccessScene()
		{
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_CHEER));
			GetRewarded();
		}

		public void OnFailureScene()
		{
		}

		public void OnBlobInShootZone(int beatType)
		{
			ResetTriggers(config.ducks[beatType - 1]);
			config.ducks[beatType - 1].GetComponent<Animator>().SetTrigger("reset");
		}

		private void SpawnHitParticles(int beatType)
		{
			GameObject gameObject = Object.Instantiate(config.hitParticles);
			gameObject.transform.SetParent(config.ducks[beatType - 1].transform);
			gameObject.transform.SetLayerRecursively(LayerMask.NameToLayer("ClothesPreview"));
			gameObject.transform.localPosition = hitParticlesOffset;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.GetComponent<ParticleSystem>().Emit(1);
			gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Emit(10);
		}

		private void SpawnMissParticles(int beatType)
		{
			GameObject gameObject = Object.Instantiate(config.missParticles);
			gameObject.transform.SetParent(config.shootRange.transform);
			gameObject.transform.SetLayerRecursively(LayerMask.NameToLayer("ClothesPreview"));
			gameObject.transform.localPosition = missParticlesOffset;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.GetComponent<ParticleSystem>().Emit(1);
			gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Emit(10);
		}

		private void GetRewarded()
		{
			for (int i = 0; i < 2; i++)
			{
				int num = (!(Random.value < 0.5f)) ? 1 : (-1);
				Manager.Get<CraftingManager>().SpawnRandomResource(oldPosition + oldRotation * (num * Vector3.right + Vector3.back * 2f));
			}
		}

		private void ResetTriggers(GameObject duck)
		{
			duck.GetComponent<Animator>().ResetTrigger("reset");
			duck.GetComponent<Animator>().ResetTrigger("hide");
			duck.GetComponent<Animator>().ResetTrigger("shoot");
		}

		public StatsManager.MinigameType GetGameType()
		{
			return StatsManager.MinigameType.DUCKS;
		}
	}
}
