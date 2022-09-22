// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveAnimations.StaticAnimationUnit
using Common.Managers;
using System.Collections;
using UnityEngine;

namespace InteractiveAnimations
{
	public class StaticAnimationUnit : MonoBehaviour
	{
		[HideInInspector]
		public GameObject selection;

		public StaticAnimation animationDefinition;

		private Animator unitAnimator;

		private float timer;

		private bool staticAnimationPlaying;

		private const string STATIC_ANIMATION_TRIGGER_NAME = "play_static_animation";

		private const string IDLE_ANIMATION_TRIGGER_NAME = "play_idle_animation";

		private string animationClipName = "static_animation";

		private AnimatorOverrideController animatorOverrideController;

		private SaveTransform saveTransformComponent => GetComponent<SaveTransform>();

		private void OnEnable()
		{
			unitAnimator = GetComponentInChildren<Animator>();
			animatorOverrideController = new AnimatorOverrideController();
			animatorOverrideController.name = "NEW";
			if (!(animationDefinition == null))
			{
				SetupAnimatorOverrideController();
			}
		}

		private void Update()
		{
			if (!(animationDefinition == null))
			{
				if (timer <= 0f)
				{
					UpdateAnimatorState();
				}
				timer -= Time.deltaTime;
			}
		}

		private void OnDestroy()
		{
			if (saveTransformComponent != null)
			{
				UnityEngine.Object.Destroy(saveTransformComponent);
			}
			if (selection != null)
			{
				UnityEngine.Object.Destroy(selection);
			}
			if (unitAnimator != null)
			{
				unitAnimator.SetTrigger("play_idle_animation");
			}
		}

		private void UpdateAnimatorState()
		{
			if (staticAnimationPlaying)
			{
				PlayIdleAnimation();
			}
			else
			{
				PlayStaticAnimation();
			}
		}

		public void SetStaticAnimation(StaticAnimation sAnimation, int index)
		{
			animationDefinition = sAnimation;
			animationDefinition.animationIndex = index;
			ForceHumanMobToStayInPlace();
			SetupAnimatorOverrideController();
			TryToAddSaveTransformComponent();
			PlayStaticAnimation();
		}

		private void ForceHumanMobToStayInPlace()
		{
			HumanMob component = GetComponent<HumanMob>();
			component.logic = AnimalMob.AnimalLogic.STAY_IN_PLACE;
			component.ReconstructBehaviourTree();
		}

		private void SetupAnimatorOverrideController()
		{
			if (!(unitAnimator == null))
			{
				RuntimeAnimatorController runtimeAnimatorController = unitAnimator.runtimeAnimatorController;
				AnimatorOverrideController animatorOverrideController = runtimeAnimatorController as AnimatorOverrideController;
				if (animatorOverrideController != null)
				{
					runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;
				}
				this.animatorOverrideController.runtimeAnimatorController = runtimeAnimatorController;
				this.animatorOverrideController[animationClipName] = animationDefinition.GetClip();
				unitAnimator.runtimeAnimatorController = this.animatorOverrideController;
			}
		}

		private void PlayIdleAnimation()
		{
			staticAnimationPlaying = false;
			timer = animationDefinition.timeDelay;
			unitAnimator.SetTrigger("play_idle_animation");
		}

		private void PlayStaticAnimation()
		{
			timer = animationDefinition.timePlaying;
			if (animationDefinition is PredefinedListAnimation)
			{
				if (animatorOverrideController != null)
				{
					StartCoroutine(OverrideAnimatorClipEndOfFrame());
				}
			}
			else
			{
				OverrideAnimatorClip();
			}
		}

		private IEnumerator OverrideAnimatorClipEndOfFrame()
		{
			animatorOverrideController["static_animation"] = animationDefinition.GetClip();
			yield return new WaitForEndOfFrame();
			unitAnimator.SetTrigger("play_static_animation");
			staticAnimationPlaying = true;
		}

		private void OverrideAnimatorClip()
		{
			staticAnimationPlaying = true;
			if (!(unitAnimator == null))
			{
				unitAnimator.ResetTrigger("play_idle_animation");
				unitAnimator.ResetTrigger("play_static_animation");
				unitAnimator.SetTrigger("play_static_animation");
			}
		}

		private void TryToAddSaveTransformComponent()
		{
			if (!(saveTransformComponent != null))
			{
				base.gameObject.AddComponent<SaveTransform>();
				saveTransformComponent.module = Manager.Get<InteractiveAnimationsManager>().saveTransformModule;
				saveTransformComponent.PrefabName = base.gameObject.name.Replace("(Clone)", string.Empty);
				saveTransformComponent.ForceInit();
			}
		}
	}
}
