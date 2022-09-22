// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestCleaner
using Common.Managers;
using QuestSystems.Adventure;
using System.Collections.Generic;
using Uniblocks;

public class QuestCleaner : InteractiveObject
{
	public override void OnUse()
	{
		base.OnUse();
		CleanQuests();
	}

	private void CleanQuests()
	{
		if (Manager.Contains<AdventureQuestManager>())
		{
			AdventureQuestManager adventureQuestManager = Manager.Get<AdventureQuestManager>();
			if (!(adventureQuestManager == null))
			{
				List<QuestDataItem> activeQuestsCopy = adventureQuestManager.GetActiveQuestsCopy();
				QuestDataItem questDataItem = null;
				foreach (QuestDataItem item in activeQuestsCopy)
				{
					questDataItem = adventureQuestManager.GetQuest(item.QuestId);
					if (questDataItem == null || questDataItem.QuestState == EQuestState.disabled)
					{
						break;
					}
					questDataItem.SetState(EQuestState.notStarted);
				}
			}
		}
	}
}
