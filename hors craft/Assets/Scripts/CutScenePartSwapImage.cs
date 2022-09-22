// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenePartSwapImage
using Common.Managers;
using UnityEngine;

public class CutScenePartSwapImage : CutScenePart
{
	public Sprite newSprite;

	public int sceneIndex;

	public string fileName = string.Empty;

	public override void Init()
	{
		base.Init();
		CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
		if (cutScenesManager == null)
		{
			UnityEngine.Debug.LogError("[CutScenePartSwapImage] : No manager");
		}
		else if (newSprite == null)
		{
			cutScenesManager.GetImageGrabber.GetSprite(sceneIndex, out newSprite, fileName);
		}
	}

	public override void RunAnimators()
	{
		foreach (AnimatorSetting setting in settings)
		{
			Init();
			setting.DoJob(new ImageSwapWorkerParameter(newSprite));
			setting.SetOffTrigger();
		}
	}
}
