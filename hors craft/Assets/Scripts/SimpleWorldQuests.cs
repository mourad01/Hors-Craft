// DecompilerFi decompiler from Assembly-CSharp.dll class: SimpleWorldQuests
using Common.Managers;
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWorldQuests : WorldsQuests
{
	public override void Init()
	{
		LoadQuestsForCurrentWorld();
	}

	public override void OnDownloadModel(Dictionary<string, object> baseModel)
	{
		LoadQuestsForCurrentWorld();
	}

	protected void LoadQuestsForCurrentWorld()
	{
		LoadQuestListForWorld(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId);
	}

	public override void AddRandomToQuestList(string worldId)
	{
		Quest quest = GenerateQuest();
		if (quest != null)
		{
			currentWorldQuestList.AddQuest(quest);
			RecreateNotificationInformation();
			Manager.Get<QuestManager>().CheckMobsIndicatorState();
			SaveQuestList(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId);
		}
	}

	private Quest GenerateQuest()
	{
		List<Quest> questList = Manager.Get<QuestManager>().GetQuestList();
		QuestType questType = GenerateType(questList);
		int prize = GetPrize(questList, questType);
		Quest quest = LoadQuestData(questType);
		int stepsNeeded = GetStepsNeeded(questList, questType, quest);
		int queueNumber = GetQueueNumber(questList, questType);
		Quest quest2 = new Quest(questType, prize, stepsNeeded, queueNumber);
		if (quest != null)
		{
			quest2.CurrentLevel = quest.CurrentLevel;
		}
		int maxLevel = GetMaxLevel(questList, questType);
		if (maxLevel > 0 && quest2.CurrentLevel >= maxLevel)
		{
			return null;
		}
		return quest2;
	}

	private int GetMaxLevel(List<Quest> premadeQuestList, QuestType newQuestType)
	{
		return GetPremadeQuest(premadeQuestList, newQuestType)?.maxLevel ?? 0;
	}

	private int GetQueueNumber(List<Quest> premadeQuestList, QuestType newQuestType)
	{
		return 0;
	}

	private int GetStepsNeeded(List<Quest> premadeQuestList, QuestType newQuestType, Quest savedQuest = null)
	{
		Quest premadeQuest = GetPremadeQuest(premadeQuestList, newQuestType);
		if (premadeQuest == null)
		{
			return 1;
		}
		int num = 0;
		Quest quest = currentWorldQuestList.GetQuestById(premadeQuest.id);
		if (quest == null)
		{
			quest = savedQuest;
		}
		if (quest != null)
		{
			num = quest.CurrentLevel;
		}
		return premadeQuest.stepsNeeded * (num + 1);
	}

	private Quest LoadQuestData(QuestType newQuestType)
	{
		string uniqueId = Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId;
		string @string = PlayerPrefs.GetString(questListWorldKey(uniqueId), string.Empty);
		if (string.IsNullOrEmpty(@string) || @string.Equals("null"))
		{
			return null;
		}
		QuestListData questListData = new QuestListData();
		questListData.quests = JSONHelper.Deserialize<List<Quest>>(@string);
		foreach (Quest quest in questListData.quests)
		{
			if (quest.type == newQuestType)
			{
				return quest;
			}
		}
		return null;
	}

	private int GetPrize(List<Quest> premadeQuestList, QuestType newQuestType)
	{
		Quest premadeQuest = GetPremadeQuest(premadeQuestList, newQuestType);
		if (premadeQuest == null)
		{
			return 0;
		}
		if (premadeQuest.specialPrizes == null)
		{
			return premadeQuest.Prize;
		}
		return 0;
	}

	private Quest GetPremadeQuest(List<Quest> premadeQuestList, QuestType newQuestType)
	{
		for (int i = 0; i < premadeQuestList.Count; i++)
		{
			if (premadeQuestList[i].type == newQuestType)
			{
				return premadeQuestList[i];
			}
		}
		return null;
	}

	private QuestType GenerateType(List<Quest> premadeQuestList)
	{
		foreach (Quest premadeQuest in premadeQuestList)
		{
			if (!currentWorldQuestList.IsActiveQuestOfType(premadeQuest.type))
			{
				return premadeQuest.type;
			}
		}
		return QuestType.None;
	}

	public override bool OnQuestClaim(string id)
	{
		int num = 0;
		foreach (Quest quest in currentWorldQuestList.quests)
		{
			if (id.Equals(quest.GenerateWorldId()))
			{
				Quest questFromPrefab = Manager.Get<QuestManager>().GetQuestFromPrefab(quest.type);
				if (questFromPrefab != null && questFromPrefab.specialPrizes != null)
				{
					if (quest.TryIncreaseLevel(id))
					{
						questFromPrefab.specialPrizes.Grant(null, quest.CurrentLevel - 1);
					}
					break;
				}
			}
			num++;
		}
		SaveQuestList(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId);
		currentWorldQuestList.quests.RemoveAt(num);
		string uniqueId = Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId;
		AddRandomToQuestList(uniqueId);
		return true;
	}
}
