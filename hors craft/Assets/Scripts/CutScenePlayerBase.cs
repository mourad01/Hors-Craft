// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenePlayerBase
using Common.Managers;
using Uniblocks;

public class CutScenePlayerBase : InteractiveObject
{
	public int sceneNumber;

	public bool useTransform = true;

	public virtual void Start()
	{
	}

	public override void OnUse()
	{
		if (Manager.Contains<CutScenesManager>() && !CutScenesManager.IsCutScenePlaying)
		{
			CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
			cutScenesManager.StartScene(sceneNumber, (!useTransform) ? null : base.transform);
		}
	}

	public virtual void SceneEnd(int sceneNumber)
	{
	}
}
