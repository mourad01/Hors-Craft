// DecompilerFi decompiler from Assembly-CSharp.dll class: PortalData
using System;
using System.Collections.Generic;

[Serializable]
public class PortalData
{
	public List<Resource> neededResources = new List<Resource>();

	public int portalQuestId = -1;

	public bool CanPlayerUsePortal()
	{
		foreach (Resource neededResource in neededResources)
		{
			if (Singleton<PlayerData>.get.playerItems.GetResourcesCount(neededResource.id) <= 0)
			{
				return false;
			}
		}
		return true;
	}

	public void GivePlayerNeededItems()
	{
		foreach (Resource neededResource in neededResources)
		{
			Singleton<PlayerData>.get.playerItems.AddResource(neededResource.id, 1);
		}
	}
}
