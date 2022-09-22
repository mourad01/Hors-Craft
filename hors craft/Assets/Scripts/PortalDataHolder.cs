// DecompilerFi decompiler from Assembly-CSharp.dll class: PortalDataHolder
using Common.Managers;
using QuestSystems.Adventure;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PortalDataHolder : MonoBehaviour
{
	public List<PortalData> portalData = new List<PortalData>();

	private void OnEnable()
	{
		PlayerItems playerItems = Singleton<PlayerData>.get.playerItems;
		playerItems.onResourceChanged = (Action<int, int>)Delegate.Remove(playerItems.onResourceChanged, new Action<int, int>(OnPlayerResourcesChanged));
		PlayerItems playerItems2 = Singleton<PlayerData>.get.playerItems;
		playerItems2.onResourceChanged = (Action<int, int>)Delegate.Combine(playerItems2.onResourceChanged, new Action<int, int>(OnPlayerResourcesChanged));
	}

	private void OnDisable()
	{
		PlayerItems playerItems = Singleton<PlayerData>.get.playerItems;
		playerItems.onResourceChanged = (Action<int, int>)Delegate.Remove(playerItems.onResourceChanged, new Action<int, int>(OnPlayerResourcesChanged));
	}

	private void OnPlayerResourcesChanged(int id, int count)
	{
		int currentWorldDataIndex = Manager.Get<SavedWorldManager>().currentWorldDataIndex;
		if (portalData != null && currentWorldDataIndex < portalData.Count && portalData[currentWorldDataIndex] != null)
		{
			AdventureQuestManager adventureQuestManager = Manager.Get<AdventureQuestManager>();
			QuestDataItem quest = adventureQuestManager.GetQuest(portalData[currentWorldDataIndex].portalQuestId);
			if (quest != null && portalData[currentWorldDataIndex].CanPlayerUsePortal() && quest.QuestState < EQuestState.done)
			{
				quest.SetState(EQuestState.done);
				quest.GrantReward();
				adventureQuestManager.Save();
			}
		}
	}

	public bool IsItemAPortalItem(int id)
	{
		foreach (PortalData portalDatum in portalData)
		{
			foreach (Resource neededResource in portalDatum.neededResources)
			{
				if (neededResource.id == id)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanPlayerUsePortal()
	{
		int currentWorldDataIndex = Manager.Get<SavedWorldManager>().currentWorldDataIndex;
		if (portalData == null || currentWorldDataIndex >= portalData.Count)
		{
			return false;
		}
		if (portalData[currentWorldDataIndex] == null)
		{
			return false;
		}
		return portalData[currentWorldDataIndex].CanPlayerUsePortal();
	}

	[ContextMenu("Give player needed items")]
	public void GiveNeededItems()
	{
		int currentWorldDataIndex = Manager.Get<SavedWorldManager>().currentWorldDataIndex;
		if (portalData != null && currentWorldDataIndex < portalData.Count && portalData[currentWorldDataIndex] != null)
		{
			portalData[currentWorldDataIndex].GivePlayerNeededItems();
		}
	}
}
