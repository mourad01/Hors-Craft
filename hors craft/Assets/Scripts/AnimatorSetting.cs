// DecompilerFi decompiler from Assembly-CSharp.dll class: AnimatorSetting
using System;
using UnityEngine;

[Serializable]
public class AnimatorSetting
{
	public string animatorKey;

	public string animatorTriggerKey = "StartTrigger";

	public string textTranslationKey = string.Empty;

	public Animator animator
	{
		get;
		private set;
	}

	protected CutSceneWorker worker
	{
		get;
		private set;
	}

	public void SetAnimator(Animator animator)
	{
		this.animator = animator;
	}

	public void SetWorker(CutSceneWorker worker)
	{
		this.worker = worker;
	}

	public void SetOffTrigger()
	{
		if (animator != null)
		{
			UnityEngine.Debug.Log("Setting off trigger " + animatorTriggerKey);
			animator.SetTrigger(animatorTriggerKey);
		}
		else
		{
			UnityEngine.Debug.LogError("No animator!");
		}
	}

	public void DoJob(WorkerParameter parameter = null)
	{
		if (!worker)
		{
			UnityEngine.Debug.Log("No worker!");
			return;
		}
		if (parameter == null)
		{
			parameter = new WorkerParameter(string.Empty);
		}
		parameter.text = textTranslationKey;
		worker.DoJob(parameter);
	}

	public void CleanUp()
	{
		if ((bool)worker)
		{
			worker.CleanUp();
		}
	}
}
