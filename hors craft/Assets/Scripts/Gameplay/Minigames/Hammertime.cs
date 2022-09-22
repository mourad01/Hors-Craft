// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Minigames.Hammertime
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using States;
using System.Collections;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Minigames
{
	public class Hammertime : UsableAnimation
	{
		public Renderer hammerLights;

		public Renderer hammerProgress;

		public float minSpeed;

		public float maxSpeed;

		private const float HIT_RESULT_TIME = 0.5f;

		private const float AFTER_HIT_RESET_TIME = 2.5f;

		private const float WIN_CONDITION = 0.832f;

		private float[] valueSteps = new float[16]
		{
			0.0625f,
			0.125f,
			0.1875f,
			0.251f,
			0.3255f,
			0.401f,
			0.467f,
			0.534f,
			0.604f,
			0.663f,
			0.726f,
			0.781f,
			0.832f,
			0.879f,
			0.918f,
			1f
		};

		private float hitResultTimer;

		private float speed;

		private float currentProgress;

		private bool isGoingUp;

		private bool minigameActive;

		private bool hitAnimation;

		private bool wasRewarded;

		private ReturnButtonContext returnContext;

		protected override void Awake()
		{
			base.Awake();
			Reset();
		}

		private void Reset()
		{
			speed = 0.2f;
			currentProgress = 0f;
			isGoingUp = true;
			minigameActive = false;
			hitAnimation = false;
			wasRewarded = false;
			hammerProgress.material.SetFloat("_Progress", 0f);
		}

		private void Update()
		{
			if (hitAnimation)
			{
				UpdateHitAnimation();
			}
			else
			{
				UpdateLights();
			}
		}

		private void UpdateHitAnimation()
		{
			if (hitResultTimer > 2.5f)
			{
				Reset();
				Quit();
			}
			else if (hitResultTimer < 0.5f)
			{
				hammerProgress.material.SetFloat("_Progress", Mathf.Lerp(0f, SnapValue(currentProgress), hitResultTimer / 0.5f));
			}
			else
			{
				if (currentProgress >= 0.832f)
				{
					hammerLights.material.SetFloat("_Progress", ((int)(hitResultTimer / 0.13f) % 2 != 0) ? 0f : 1f);
				}
				if (!wasRewarded)
				{
					if (currentProgress >= 0.832f)
					{
						GetComponent<Animator>().SetBool("WinLight", value: true);
						PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.DANCE_CHEER));
					}
					hammerProgress.material.SetFloat("_Progress", SnapValue(currentProgress));
					RewardBasedOnProgress();
					wasRewarded = true;
				}
			}
			hitResultTimer += Time.deltaTime;
		}

		private void UpdateLights()
		{
			if (isGoingUp)
			{
				currentProgress += speed * Time.deltaTime;
				if (currentProgress >= 1f)
				{
					isGoingUp = false;
				}
			}
			else
			{
				currentProgress -= speed * Time.deltaTime;
				if (currentProgress <= 0f)
				{
					isGoingUp = true;
				}
			}
			hammerLights.material.SetFloat("_Progress", currentProgress);
		}

		public override void OnUse()
		{
			UsableIndicator component = GetComponent<UsableIndicator>();
			if (component != null)
			{
				component.Interact();
			}
			if (!minigameActive)
			{
				TicketsManager.TakeEntranceFeeIfPossible(SetupMinigame);
				return;
			}
			GetComponent<Animator>().SetTrigger("hit");
			InteractiveObject.isLocked = true;
			StartCoroutine(OnHit());
		}

		private void SetupMinigame()
		{
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().StartInteractiveCutscene(animationParent.transform);
			Animator animator = GetComponent<Animator>();
			if (animator == null)
			{
				animator = base.gameObject.AddComponent<Animator>();
				animator.runtimeAnimatorController = runtimeAnimator;
			}
			Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().SetSubstate(GameplayState.Substates.MINIGAME);
			returnContext = new ReturnButtonContext
			{
				onReturnButton = delegate
				{
					Reset();
					Quit();
				}
			};
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.RETURN_BUTTON_ACTIVATED, returnContext);
			animator.SetBool("Active", value: true);
			speed = Random.Range(minSpeed, maxSpeed);
			minigameActive = true;
		}

		private IEnumerator OnHit()
		{
			yield return new WaitForSeconds(0.1f);
			PlaySound(Manager.Get<ClipsManager>().GetClipFor(GameSound.HIT_DIRT));
			yield return new WaitForSeconds(0.1f);
			Animator animator = GetComponent<Animator>();
			while (!animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
			{
				yield return new WaitForEndOfFrame();
			}
			hammerLights.material.SetFloat("_Progress", SnapValue(currentProgress));
			hitResultTimer = 0f;
			hitAnimation = true;
		}

		private void RewardBasedOnProgress()
		{
			MonoBehaviourSingleton<ProgressCounter>.get.Increment(ProgressCounter.Countables.MINIGAMES_PLAYED);
			if (currentProgress >= 0.832f)
			{
				GetRewarded(3);
				Manager.Get<StatsManager>().MinigameFinished(StatsManager.MinigameType.HAMMERTIME, success: true);
			}
			else
			{
				Manager.Get<StatsManager>().MinigameFinished(StatsManager.MinigameType.HAMMERTIME, success: false);
			}
		}

		private void GetRewarded(int count)
		{
			for (int i = 0; i < count; i++)
			{
				int num = (!(Random.value < 0.5f)) ? 1 : (-1);
				Manager.Get<CraftingManager>().SpawnRandomResource(base.transform.position + base.transform.rotation * (num * Vector3.right));
			}
		}

		private float SnapValue(float value)
		{
			int i;
			for (i = 0; i < valueSteps.Length && valueSteps[i] < value; i++)
			{
			}
			if (i == valueSteps.Length)
			{
				i--;
			}
			return valueSteps[i];
		}

		private void Quit()
		{
			Animator component = GetComponent<Animator>();
			component.SetBool("Active", value: false);
			component.SetBool("WinLight", value: false);
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.RETURN_BUTTON_ACTIVATED, returnContext);
			returnContext = null;
			InteractiveObject.isLocked = false;
			CameraController.instance.Anchor.GetComponent<PlayerMovement>().EndCustomCutscene();
			Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().SetSubstate(GameplayState.Substates.WALKING);
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
	}
}
