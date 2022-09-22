// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.StateMachineBehaviours.AnimatorSpeed
using UnityEngine;

namespace Common.Behaviours.StateMachineBehaviours
{
	public class AnimatorSpeed : StateMachineBehaviour
	{
		public float speed = 1f;

		private float baseAnimatorSpeed;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			baseAnimatorSpeed = animator.speed;
			UpdateSpeed(animator);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			UpdateSpeed(animator);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.speed = baseAnimatorSpeed;
		}

		private void UpdateSpeed(Animator animator)
		{
			animator.speed = speed;
		}
	}
}
