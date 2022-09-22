// DecompilerFi decompiler from Assembly-CSharp.dll class: PortalWorldLoader
using Common.Managers;
using Common.Utils;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalWorldLoader : InteractiveObject
{
	public int sceneNumberDefault;

	public bool suppressLoadingLevel;

	public bool isLastLevel;

	private bool afterCutScene;

	public override void OnUse()
	{
		if (!Manager.Contains<AdventureQuestManager>())
		{
			return;
		}
		AdventureQuestManager adventureQuestManager = Manager.Get<AdventureQuestManager>();
		if (!(adventureQuestManager == null))
		{
			if (adventureQuestManager.portalDataHolder.CanPlayerUsePortal())
			{
				TryToShowCutScene(success: true);
			}
			else
			{
				TryToShowCutScene(success: false);
			}
		}
	}

	private void TryToShowCutScene(bool success)
	{
		if (!Manager.Contains<CutScenesManager>())
		{
			return;
		}
		CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
		if (!(cutScenesManager == null))
		{
			if (success)
			{
				CutScenesManager cutScenesManager2 = cutScenesManager;
				cutScenesManager2.onSceneEnded = (Action<int>)Delegate.Combine(cutScenesManager2.onSceneEnded, (Action<int>)delegate
				{
					if (!suppressLoadingLevel)
					{
						DeletePlayeritems();
						if (isLastLevel)
						{
							afterCutScene = true;
						}
						else
						{
							Manager.Get<StateMachineManager>().PushState<ChooseWorldState>();
						}
					}
				});
				cutScenesManager.StartIntroScene(outro: true, base.transform);
			}
			else
			{
				cutScenesManager.StartScene(sceneNumberDefault, base.transform);
			}
		}
	}

	private void DeletePlayeritems()
	{
		List<Resource> resourcesList = Singleton<PlayerData>.get.playerItems.GetResourcesList();
		foreach (Resource item in resourcesList)
		{
			Singleton<PlayerData>.get.playerItems.AddResource(item.id, -item.count);
		}
	}

	private void LateUpdate()
	{
		if (isLastLevel && afterCutScene)
		{
			afterCutScene = false;
			EndGame();
		}
	}

	[ContextMenu("tesdt end scene")]
	private void EndGame()
	{
		if (isLastLevel)
		{
			TimeScaleHelper.value = 0f;
			Manager.Get<CutScenesManager>().ResetProgress();
			Manager.Get<AdventureQuestManager>().ClearQuests();
			List<Resource> resourcesList = Singleton<PlayerData>.get.playerItems.GetResourcesList();
			foreach (Resource item in resourcesList)
			{
				Singleton<PlayerData>.get.playerItems.AddToResources(item.id, -item.count);
			}
			Manager.Get<SavedWorldManager>().ResetAll();
			Manager.Get<GameCallbacksManager>().Restart();
			MobsManager mobsManager = Manager.Get<MobsManager>();
			mobsManager.DespawnAll();
			SceneManager.UnloadScene("Gameplay");
			Manager.Get<StateMachineManager>().PushState<TitleState>();
			Manager.Get<CutScenesManager>().Init();
		}
	}
}
