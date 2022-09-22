// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenePart
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CutScenePart : MonoBehaviour
{
	public string partName = "Scene part";

	public List<AnimatorSetting> settings = new List<AnimatorSetting>();

	public int AnimatorsCount
	{
		get
		{
			if (settings == null)
			{
				return 0;
			}
			return settings.Count;
		}
	}

	public virtual void Init()
	{
		CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
		if (!(cutScenesManager == null))
		{
			Animator animator = null;
			foreach (AnimatorSetting setting in settings)
			{
				if (cutScenesManager.GetAnimator(setting.animatorKey, out animator))
				{
					setting.SetAnimator(animator);
					setting.SetWorker(animator.transform.GetComponent<CutSceneWorker>());
				}
				else
				{
					UnityEngine.Debug.LogError($"tried to get animator ({setting.animatorKey}) but got nothing!({partName}) : {base.transform.parent.name}");
				}
			}
		}
	}

	public virtual void RunAnimators()
	{
		foreach (AnimatorSetting setting in settings)
		{
			UnityEngine.Debug.Log("Running animator settings.");
			Init();
			setting.DoJob();
			setting.SetOffTrigger();
		}
	}

	public virtual void CleanUp()
	{
		foreach (AnimatorSetting setting in settings)
		{
			setting.CleanUp();
		}
	}
}
