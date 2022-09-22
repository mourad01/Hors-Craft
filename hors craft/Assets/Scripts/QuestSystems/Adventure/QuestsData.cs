// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestSystems.Adventure.QuestsData
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystems.Adventure
{
	[Serializable]
	public class QuestsData
	{
		protected Dictionary<int, QuestDataItem> quests = new Dictionary<int, QuestDataItem>();

		protected QuestLoader loader;

		public int QuestCount
		{
			get
			{
				if (quests == null)
				{
					return 0;
				}
				return quests.Count;
			}
		}

		public int[] QuestIds
		{
			get
			{
				if (quests == null)
				{
					return new int[0];
				}
				int[] array = new int[quests.Keys.Count];
				quests.Keys.CopyTo(array, 0);
				return array;
			}
		}

		public QuestsData(QuestLoader loader)
		{
			this.loader = loader;
			if (!loader.TryToLoad(ref quests))
			{
				quests = new Dictionary<int, QuestDataItem>();
			}
		}

		public bool AddNewQuest(QuestDataItem newQuest)
		{
			if (newQuest.QuestId == -1)
			{
				newQuest.OverrideId(GetNewId());
			}
			if (quests.ContainsKey(newQuest.QuestId))
			{
				UnityEngine.Debug.LogError($"[QuestData] : Cannot add quest (id:{newQuest.QuestId})! Duplicate id!");
				return false;
			}
			quests.Add(newQuest.QuestId, newQuest);
			return true;
		}

		public void Merge(List<QuestDataItem> otherQuests)
		{
			foreach (QuestDataItem otherQuest in otherQuests)
			{
				if (quests.ContainsKey(otherQuest.QuestId))
				{
					quests[otherQuest.QuestId].Edit(otherQuest);
				}
				else
				{
					quests.Add(otherQuest.QuestId, otherQuest);
				}
			}
		}

		public bool HasQuest(int questId)
		{
			return quests.ContainsKey(questId);
		}

		public QuestDataItem GetQuest(int questId)
		{
			if (!HasQuest(questId))
			{
				return null;
			}
			return quests[questId];
		}

		public void EditQuest(int questId, QuestDataItem newQuest)
		{
			if (HasQuest(questId))
			{
				quests[questId].Edit(newQuest);
			}
		}

		internal void DeleteQuest(int questId)
		{
			if (HasQuest(questId))
			{
				quests.Remove(questId);
			}
		}

		public void Save()
		{
			Save(loader);
		}

		public void Save(QuestLoader loader)
		{
			loader.Save(quests);
		}

		[ContextMenu("Debug quest data")]
		public void DebugQuestData()
		{
			foreach (KeyValuePair<int, QuestDataItem> quest in quests)
			{
				UnityEngine.Debug.Log(quest.Value);
			}
		}

		private int GetNewId()
		{
			if (quests == null || quests.Count <= 0)
			{
				return 0;
			}
			List<int> list = new List<int>(quests.Keys);
			list.Sort((int x, int y) => x.CompareTo(y));
			int num = -1;
			for (int i = 0; i < list.Count; i++)
			{
				if (i != list[i] && list[i] != -1)
				{
					return i;
				}
			}
			return (num != -1) ? num : list.Count;
		}

		internal List<QuestDataItem> GetActiveQuests()
		{
			List<QuestDataItem> list = new List<QuestDataItem>();
			foreach (KeyValuePair<int, QuestDataItem> quest in quests)
			{
				if (!quest.Value.QuestIsNonQuestable() && quest.Value.QuestState != 0 && quest.Value.QuestState != EQuestState.notStarted)
				{
					QuestDataItem item = quest.Value.DeepCopy();
					list.Add(item);
				}
			}
			return list;
		}
	}
}
