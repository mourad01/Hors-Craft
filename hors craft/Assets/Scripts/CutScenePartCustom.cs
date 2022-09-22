// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenePartCustom
using Common.Managers;
using UnityEngine;

public class CutScenePartCustom : CutScenePart
{
	public GameObject animatorObjectPrefab;

	private GameObject go;

	public void Awake()
	{
		if (!Manager.Contains<CutScenesManager>())
		{
			return;
		}
		CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
		if (!(cutScenesManager == null))
		{
			Animator animator = null;
			if (!cutScenesManager.GetAnimator(settings[0].animatorKey, out animator) || !animator.transform.root.gameObject.name.Contains(animatorObjectPrefab.name))
			{
				go = Object.Instantiate(animatorObjectPrefab);
			}
		}
	}

	public override void Init()
	{
		UnityEngine.Debug.Log($"{base.transform.name} : Init on custom");
		if (go == null)
		{
			Awake();
		}
		base.Init();
	}

	public override void CleanUp()
	{
		base.CleanUp();
	}
}
