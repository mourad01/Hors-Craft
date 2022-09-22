// DecompilerFi decompiler from Assembly-CSharp.dll class: DailyChestAnimBehavior
using Common.Managers;
using UnityEngine;

public class DailyChestAnimBehavior : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.IsName("End"))
		{
			Manager.Get<DailyChestManager>().DestroyCurrentInstanceIfShould();
		}
	}
}
