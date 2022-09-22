// DecompilerFi decompiler from Assembly-CSharp.dll class: HumanMobNavigator
using Common.Managers;
using InteractiveAnimations;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(HumanMob))]
public class HumanMobNavigator : MobNavigator
{
	public bool enableAnimations = true;

	private AnimationClip defaultClip;

	private AnimatorOverrideController animatorOverrideController;

	private InteractiveAnimationsManager interactiveAnimationsManager;

	protected override void Start()
	{
		interactiveAnimationsManager = Manager.Get<InteractiveAnimationsManager>();
	}

	protected override void EndMovement()
	{
		base.EndMovement();
		TryToOverrideIdleAnimation();
	}

	private void TryToOverrideIdleAnimation()
	{
		if (base.mob.animator == null || interactiveAnimationsManager == null)
		{
			return;
		}
		if (defaultClip == null)
		{
			defaultClip = (from c in base.mob.animator.runtimeAnimatorController.animationClips
				where c.name.ToString() == "idle"
				select c).FirstOrDefault();
		}
		if (this.animatorOverrideController == null)
		{
			RuntimeAnimatorController runtimeAnimatorController = base.mob.animator.runtimeAnimatorController;
			AnimatorOverrideController animatorOverrideController = runtimeAnimatorController as AnimatorOverrideController;
			if (animatorOverrideController != null)
			{
				runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;
			}
			this.animatorOverrideController = new AnimatorOverrideController
			{
				runtimeAnimatorController = runtimeAnimatorController
			};
			base.mob.animator.runtimeAnimatorController = this.animatorOverrideController;
		}
		if (!enableAnimations && defaultClip != null)
		{
			this.animatorOverrideController["idle"] = defaultClip;
		}
		else
		{
			this.animatorOverrideController["idle"] = interactiveAnimationsManager.GetRandomAnimation().GetClip();
		}
	}
}
