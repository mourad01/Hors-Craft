// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestSystems.Adventure.QuestDataItem
using Common.Managers;
using System;
using UnityEngine;

namespace QuestSystems.Adventure
{
	[Serializable]
	public class QuestDataItem : GenericObject
	{
		public const string QUEST_DESCRIPTION_KEY = "quest.description.";

		public const string QUEST_DESCRIPTION_DONE_KEY = "quest.description.ended.";

		public const string QUEST_NEWQUEST_KEY = "quest.newQuest";

		[SerializeField]
		protected int questId = -1;

		[SerializeField]
		protected string questName = "noName";

		[SerializeField]
		protected EQuestState questState = EQuestState.notStarted;

		[SerializeField]
		protected int maxSteps = 1;

		[SerializeField]
		protected RequirementBase requirement;

		[SerializeField]
		protected RewardBase reward;

		protected int currentLine;

		protected int lastChoosedOpion;

		public int QuestId => questId;

		public string QuestName => questName;

		public int MaxSteps => maxSteps;

		public EQuestState QuestState => questState;

		public int CurrentLine => currentLine;

		public int LastChoosedOption => lastChoosedOpion;

		public RequirementBase Requirement => requirement;

		public QuestDataItem()
		{
		}

		public QuestDataItem(int questId, string questName, int maxSteps = 1, RewardBase reward = null, RequirementBase requirement = null)
		{
			this.questId = questId;
			this.maxSteps = maxSteps;
			this.reward = reward;
			this.requirement = requirement;
			this.questName = questName;
		}

		public override string ToString()
		{
			return string.Format("questId: {0}({5}), current state: {1}, maxSteps: {2}\nReward:{3}, Requirements:{4}", questId, questState, maxSteps, reward.ToString(), requirement.ToString(), questName);
		}

		public void OverrideId(int newId)
		{
			questId = newId;
		}

		public void Start()
		{
			questState = EQuestState.started;
		}

		public void SetState(EQuestState newState)
		{
			questState = newState;
			currentLine = 0;
		}

		public void End()
		{
			questState = EQuestState.done;
		}

		public void UpdateState()
		{
			currentLine = 0;
			lastChoosedOpion = 0;
			if (questState == EQuestState.notStarted)
			{
				questState = EQuestState.started;
				Manager.Get<StatsManager>().AdventureQuestStarted(QuestId);
			}
			else
			{
				CheckRequirements();
				GrantReward();
			}
		}

		public void ForceNewState()
		{
			switch (questState)
			{
			case EQuestState.disabled:
				questState = EQuestState.notStarted;
				break;
			case EQuestState.notStarted:
				questState = EQuestState.started;
				break;
			case EQuestState.started:
				questState = EQuestState.notDone;
				break;
			case EQuestState.notDone:
				questState = EQuestState.partiallyDone;
				break;
			case EQuestState.partiallyDone:
				questState = EQuestState.done;
				break;
			case EQuestState.done:
				questState = EQuestState.afterDone;
				break;
			case EQuestState.afterDone:
				questState = EQuestState.disabled;
				break;
			}
		}

		public void CheckRequirements()
		{
			if (questState != EQuestState.done && questState != 0 && questState != EQuestState.notStarted && questState != EQuestState.afterDone)
			{
				switch (CheckProgress())
				{
				case RequirementBase.ECheckState.hasNone:
					questState = EQuestState.notDone;
					break;
				case RequirementBase.ECheckState.hasSome:
					questState = EQuestState.partiallyDone;
					break;
				case RequirementBase.ECheckState.hasAll:
					questState = EQuestState.done;
					break;
				}
			}
		}

		public void UpdateLine(int updateCount = 1)
		{
			currentLine += updateCount;
		}

		public void UpdateChoosedOption(int newOption)
		{
			lastChoosedOpion = newOption;
		}

		protected RequirementBase.ECheckState CheckProgress()
		{
			return requirement.CheckProgress();
		}

		public void GrantReward()
		{
			if (questState == EQuestState.done)
			{
				Manager.Get<StatsManager>().AdventureQuestFinished(QuestId);
				reward.Grant(requirement);
				questState = EQuestState.afterDone;
				string text = Manager.Get<TranslationsManager>().GetText(GenerateEndDescriptionTranslationKey(), "Quest Done!");
				Manager.Get<ToastManager>().ShowToast(text, 2f);
			}
		}

		internal void Edit(QuestDataItem newQuest)
		{
			questId = newQuest.questId;
			questName = newQuest.questName;
			maxSteps = newQuest.maxSteps;
			reward = newQuest.reward;
			requirement = newQuest.requirement;
		}

		public QuestDataItem DeepCopy()
		{
			QuestDataItem questDataItem = new QuestDataItem();
			questDataItem.questId = questId;
			questDataItem.maxSteps = maxSteps;
			questDataItem.reward = reward.DeepCopy();
			questDataItem.requirement = requirement.DeepCopy();
			questDataItem.questName = questName;
			questDataItem.questState = questState;
			return questDataItem;
		}

		public string GenerateDescriptionTranslationKey()
		{
			return string.Format("{0}{1}", "quest.description.", questName);
		}

		public string GenerateEndDescriptionTranslationKey()
		{
			return string.Format("{0}{1}", "quest.description.ended.", questName);
		}

		public bool QuestIsNonQuestable()
		{
			if (Requirement != null && Requirement.RequirementItemId == 999)
			{
				return true;
			}
			return false;
		}

		internal void ShowNewQuestToast()
		{
			if (!QuestIsNonQuestable())
			{
				string text = Manager.Get<TranslationsManager>().GetText("quest.newQuest", "You have got a new quest!");
				Manager.Get<ToastManager>().ShowToast(text, 2f);
			}
		}

		internal void GivePlayeritem(int itemToGive)
		{
			if (requirement.RequirementItemId == 999 || (itemToGive >= 400 && itemToGive < 600))
			{
				RewardBase rewardBase = new RewardBase(ERewardType.ItemReward, itemToGive, 1);
				rewardBase.Grant();
			}
			else
			{
				reward.InformPlayer(itemToGive);
			}
		}
	}
}
