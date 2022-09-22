// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.StateMachineBehaviours.RandomFrameOnStart
using UnityEngine;

namespace Common.Behaviours.StateMachineBehaviours
{
	public class RandomFrameOnStart : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.ForceStateNormalizedTime(Random.value);
		}
	}
}
