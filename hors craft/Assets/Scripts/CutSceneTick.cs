// DecompilerFi decompiler from Assembly-CSharp.dll class: CutSceneTick
using Common.Managers;
using UnityEngine;

public class CutSceneTick : StateMachineBehaviour
{
	public float fadeTime = 1f;

	public float cutSceneActTime = 10f;

	public float timeNormalized;

	public bool inProgress;

	protected bool wasCorrectStart;

	public int debugIndex = -1;

	public bool isBaseAnimation;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		UnityEngine.Debug.Log($"State on ({animator.transform.name}) triggered.");
		if (Manager.Contains<CutScenesManager>())
		{
			CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
			if (!(cutScenesManager == null))
			{
				cutSceneActTime = cutScenesManager.CurrentActTime();
				animator.SetFloat("animationTime", 1f / cutSceneActTime);
				animator.SetTrigger("FadeTrigger");
				inProgress = false;
				wasCorrectStart = true;
				timeNormalized = 0f;
			}
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!wasCorrectStart)
		{
			OnStateEnter(animator, stateInfo, layerIndex);
		}
		if (stateInfo.IsName("base.ShowPart"))
		{
			timeNormalized = fadeTime / Manager.Get<CutScenesManager>().CurrentActTime();
			if (stateInfo.normalizedTime >= 1f - timeNormalized && !inProgress)
			{
				inProgress = true;
				animator.SetTrigger("FadeOutTrigger");
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		UnityEngine.Debug.Log($"State on ({animator.transform.name}) exiting.");
		if (!Manager.Contains<CutScenesManager>())
		{
			return;
		}
		CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
		if (!(cutScenesManager == null))
		{
			if (!stateInfo.IsName("base.ShowPart") && isBaseAnimation)
			{
				animator.SetTrigger("FadeOutTrigger");
			}
			wasCorrectStart = false;
			cutScenesManager.OnPartEnd();
		}
	}
}
