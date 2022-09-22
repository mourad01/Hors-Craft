// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.UsableAnimation
using Common.Managers;
using States;
using System;
using System.Collections;
using UnityEngine;

namespace Uniblocks
{
	public class UsableAnimation : InteractiveObject
	{
		public RuntimeAnimatorController runtimeAnimator;

		public Avatar avatar;

		public GameObject animationParent;

		public float duration;

		private PlayerMovement playerMovement;

		public override void OnUse()
		{
			base.OnUse();
			StartAnimation();
		}

		private void StartAnimation()
		{
			Animator animator = GetComponentInChildren<Animator>();
			if (animator == null)
			{
				animator = base.gameObject.AddComponent<Animator>();
				animator.runtimeAnimatorController = runtimeAnimator;
				if (avatar != null)
				{
					animator.avatar = avatar;
				}
			}
			playerMovement = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<PlayerMovement>();
			playerMovement.StartCustomCutscene(animationParent.transform);
			animator.SetTrigger("play");
			(Manager.Get<StateMachineManager>().currentState as GameplayState).HideUI(useAction: true);
			InteractiveObject.isLocked = true;
			StartCoroutine(Wait(duration, delegate
			{
				End();
			}));
		}

		private IEnumerator Wait(float duration, Action onEnd)
		{
			yield return new WaitForSecondsRealtime(duration);
			while (!(Manager.Get<StateMachineManager>().currentState is GameplayState))
			{
				yield return null;
			}
			onEnd();
		}

		private void End()
		{
			Animator component = GetComponent<Animator>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			InteractiveObject.isLocked = false;
			playerMovement.EndCustomCutscene();
			(Manager.Get<StateMachineManager>().currentState as GameplayState).ShowUI();
			MonoBehaviourSingleton<ProgressCounter>.get.Increment(ProgressCounter.Countables.MINIGAMES_PLAYED);
		}
	}
}
