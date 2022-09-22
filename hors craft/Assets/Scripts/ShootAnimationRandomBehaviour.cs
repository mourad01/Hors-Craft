// DecompilerFi decompiler from Assembly-CSharp.dll class: ShootAnimationRandomBehaviour
using UnityEngine;

public class ShootAnimationRandomBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetTrigger((!(Random.value < 0.5f)) ? "shot2" : "shot1");
	}
}
