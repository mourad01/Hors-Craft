// DecompilerFi decompiler from Assembly-CSharp.dll class: PettableAdventureCount
using QuestSystems.Adventure;
using System.Collections.Generic;

public class PettableAdventureCount : PettableAdventure
{
	public int amount = 1;

	public override void ShowHideMob()
	{
		List<Resource> resourcesList = Singleton<PlayerData>.get.playerItems.GetResourcesList();
		foreach (Resource item in resourcesList)
		{
			if (item.id == itemidToGive && item.count >= amount)
			{
				QuestWorldObjectBase component = GetComponent<QuestWorldObjectBase>();
				if (!(component == null))
				{
					component.OnQuestUpdate(EQuestState.done);
				}
				break;
			}
		}
	}
}
