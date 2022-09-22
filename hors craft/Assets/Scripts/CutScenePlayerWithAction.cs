// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenePlayerWithAction
using Common.Managers;
using System;

public class CutScenePlayerWithAction : CutScenePlayerBase
{
	public int questId = -1;

	private void OnEnable()
	{
		if (Manager.Contains<CutScenesManager>())
		{
			CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
			if (!(cutScenesManager == null))
			{
				CutScenesManager cutScenesManager2 = cutScenesManager;
				cutScenesManager2.onSceneEnded = (Action<int>)Delegate.Remove(cutScenesManager2.onSceneEnded, new Action<int>(((CutScenePlayerBase)this).SceneEnd));
				CutScenesManager cutScenesManager3 = cutScenesManager;
				cutScenesManager3.onSceneEnded = (Action<int>)Delegate.Combine(cutScenesManager3.onSceneEnded, new Action<int>(((CutScenePlayerBase)this).SceneEnd));
			}
		}
	}

	public override void OnUse()
	{
		if (Manager.Contains<CutScenesManager>())
		{
			CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
			if (!(cutScenesManager == null))
			{
				cutScenesManager.StartScene(sceneNumber, (!useTransform) ? null : base.transform);
			}
		}
	}

	private void OnDisable()
	{
		try
		{
			if (Manager.Contains<CutScenesManager>())
			{
				CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
				if (!(cutScenesManager == null))
				{
					CutScenesManager cutScenesManager2 = cutScenesManager;
					cutScenesManager2.onSceneEnded = (Action<int>)Delegate.Remove(cutScenesManager2.onSceneEnded, new Action<int>(((CutScenePlayerBase)this).SceneEnd));
				}
			}
		}
		catch
		{
		}
	}

	public override void SceneEnd(int sceneNumber)
	{
		if (base.sceneNumber == sceneNumber && Manager.Contains<AdventureQuestManager>())
		{
			AdventureQuestManager adventureQuestManager = Manager.Get<AdventureQuestManager>();
			adventureQuestManager.GetQuest(questId)?.UpdateState();
		}
	}
}
