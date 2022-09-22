// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestListData
using System;
using System.Collections.Generic;

[Serializable]
public class QuestListData
{
	public List<Quest> quests;

	public int questListNumber = -1;

	public bool IsActiveQuestOfType(QuestType type)
	{
		return quests.Find((Quest quest) => quest.type == type) != null;
	}

	public Quest GetQuestById(int id)
	{
		return quests.Find((Quest quest) => quest.id == id);
	}

	public int GetMaxQueueNumber()
	{
		return questListNumber;
	}

	public void AddQuest(Quest quest)
	{
		quests.Add(quest);
		questListNumber++;
	}
}
