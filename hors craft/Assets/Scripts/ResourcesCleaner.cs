// DecompilerFi decompiler from Assembly-CSharp.dll class: ResourcesCleaner
using Common.Managers;
using System.Collections.Generic;
using Uniblocks;

public class ResourcesCleaner : InteractiveObject
{
	public override void OnUse()
	{
		base.OnUse();
		CleanResources();
	}

	private void CleanResources()
	{
		List<Resource> resourcesList = Singleton<PlayerData>.get.playerItems.GetResourcesList();
		foreach (Resource item in resourcesList)
		{
			Singleton<PlayerData>.get.playerItems.AddToResources(item.id, -item.count);
			Manager.Get<ToastManager>().ShowToast("Resources destroyed", 4f);
		}
	}
}
