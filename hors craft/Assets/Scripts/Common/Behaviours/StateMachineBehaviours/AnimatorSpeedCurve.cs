// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.StateMachineBehaviours.AnimatorSpeedCurve
using UnityEngine;

namespace Common.Behaviours.StateMachineBehaviours
{
	public class AnimatorSpeedCurve : StateMachineBehaviour
	{
		[Header("Speed = curve (from 0 to 1 ) * base animator speed")]
		public AnimationCurve speedCurve = AnimationCurve.Linear(0f, 2f, 1f, 1f);

		private float baseAnimatorSpeed;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			baseAnimatorSpeed = animator.speed;
			UpdateSpeed(animator, stateInfo.normalizedTime);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			UpdateSpeed(animator, stateInfo.normalizedTime);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			UpdateSpeed(animator, stateInfo.normalizedTime);
		}

		private void UpdateSpeed(Animator animator, float normalizedTime)
		{
			animator.speed = baseAnimatorSpeed * speedCurve.Evaluate(normalizedTime % 1f);
		}
	}
}
