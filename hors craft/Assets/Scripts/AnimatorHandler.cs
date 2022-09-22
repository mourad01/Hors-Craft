// DecompilerFi decompiler from Assembly-CSharp.dll class: AnimatorHandler
using Common.Managers;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
	public string key;

	public Animator animator;

	public bool reset;

	public void Awake()
	{
		if (Manager.Contains<CutScenesManager>())
		{
			CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
			if (reset)
			{
				animator.SetTrigger("reset");
			}
			if (!(cutScenesManager == null))
			{
				cutScenesManager.RegisterAnimator(key, this);
			}
		}
	}
}
