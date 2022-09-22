// DecompilerFi decompiler from Assembly-CSharp.dll class: HumanMobWithGender
using Common.Managers;
using InteractiveAnimations;
using UnityEngine;

public class HumanMobWithGender : HumanMob
{
	[SerializeField]
	protected Skin.Gender forceGender;

	public override Skin.Gender currentGender => forceGender;

	protected override void Start()
	{
		base.Start();
		OverrideGenderWalkingAnimator();
	}

	private void OverrideGenderWalkingAnimator()
	{
		if (base.animator == null)
		{
			return;
		}
		InteractiveAnimationsManager interactiveAnimationsManager = Manager.Get<InteractiveAnimationsManager>();
		if (!(interactiveAnimationsManager == null))
		{
			RuntimeAnimatorController runtimeAnimatorController = base.animator.runtimeAnimatorController;
			AnimatorOverrideController animatorOverrideController = runtimeAnimatorController as AnimatorOverrideController;
			if (animatorOverrideController != null)
			{
				runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;
			}
			AnimatorOverrideController animatorOverrideController2 = new AnimatorOverrideController();
			animatorOverrideController2.runtimeAnimatorController = runtimeAnimatorController;
			AnimatorOverrideController animatorOverrideController3 = animatorOverrideController2;
			animatorOverrideController3["walking"] = interactiveAnimationsManager.GetGenderWalkingAnimation(currentGender).GetClip();
			base.animator.runtimeAnimatorController = animatorOverrideController3;
		}
	}
}
