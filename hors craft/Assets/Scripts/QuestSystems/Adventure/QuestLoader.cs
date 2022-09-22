// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestSystems.Adventure.QuestLoader
using MiniJSON;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystems.Adventure
{
	public class QuestLoader
	{
		private const string PREFS_ADVENTURE_QUESTS_KEY = "adventureQuestData";

		public virtual bool TryToLoad(ref Dictionary<int, QuestDataItem> data)
		{
			UnityEngine.Debug.Log("[QuestLaoder] : Trying to load saved data");
			if (!PlayerPrefs.HasKey("adventureQuestData"))
			{
				return false;
			}
			string @string = PlayerPrefs.GetString("adventureQuestData", string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				return false;
			}
			if (TryToParse(@string, ref data))
			{
				if (data == null)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		private bool TryToParse(string dataUnparsed, ref Dictionary<int, QuestDataItem> data)
		{
			List<object> list = Json.Deserialize(dataUnparsed) as List<object>;
			QuestDataItem questDataItem = null;
			foreach (string item in list)
			{
				questDataItem = JsonUtility.FromJson<QuestDataItem>(item);
				data.Add(questDataItem.QuestId, questDataItem);
				questDataItem = null;
			}
			return true;
		}

		internal void DeleteAll()
		{
			PlayerPrefs.DeleteKey("adventureQuestData");
			PlayerPrefs.Save();
		}

		public virtual void Save(Dictionary<int, QuestDataItem> data)
		{
			List<object> list = new List<object>();
			foreach (KeyValuePair<int, QuestDataItem> datum in data)
			{
				string text = JsonUtility.ToJson(datum.Value);
				list.Add(text);
				UnityEngine.Debug.Log($"SERIALIZED : quest id: {datum.Key}, data: {text}");
			}
			string value = Json.Serialize(list);
			PlayerPrefs.SetString("adventureQuestData", value);
			PlayerPrefs.Save();
		}

		public virtual bool UpdateQuestData(int questId)
		{
			return false;
		}
	}
}
