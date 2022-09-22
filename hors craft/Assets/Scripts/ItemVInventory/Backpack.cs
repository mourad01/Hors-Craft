// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.Backpack
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemVInventory
{
	public class Backpack : MonoBehaviour, IGameCallbacksListener
	{
		[Serializable]
		public struct BackpackSlot
		{
			public ItemType slotType;

			public int slotCapacity;

			public bool isInfinity;

			public bool Equals(ItemType itemType)
			{
				return slotType == itemType;
			}

			public bool Equals(BackpackSlot other)
			{
				return slotType == other.slotType;
			}
		}

		[Serializable]
		public struct JockerSlot
		{
			public int slotCapacity;

			public bool isInfinity;
		}

		[Serializable]
		public class BackpackItem
		{
			public ItemType type;

			public int level;

			public string itemId;

			public BackpackItem(ItemType itemType, int level, string itemId)
			{
				type = itemType;
				this.level = level;
				this.itemId = itemId;
			}

			public bool Equals(BackpackItem other)
			{
				return type == other.type && level == other.level && itemId == other.itemId;
			}

			public bool Equals(ItemType itemType, int level, string itemId)
			{
				return type == itemType && (this.level == level || level == -1 || this.level == -1) && this.itemId == itemId;
			}
		}

		[Serializable]
		public class BackpackItemWrapper
		{
			public List<BackpackItem> list;

			public BackpackItemWrapper(List<BackpackItem> l)
			{
				list = l;
			}
		}

		public const int NO_LEVEL = -1;

		[SerializeField]
		private BackpackSlot[] backpackSlots;

		[SerializeField]
		private JockerSlot jockerBackpackSlot;

		private readonly Dictionary<ItemType, BackpackSlot> itemType2Slot = new Dictionary<ItemType, BackpackSlot>();

		private readonly Dictionary<ItemType, List<BackpackItem>> typedSlots = new Dictionary<ItemType, List<BackpackItem>>();

		private List<BackpackItem> jockerSlot = new List<BackpackItem>();

		private bool isDirty;

		private void Awake()
		{
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
			Init();
		}

		public int GetAvailablePlace(ItemType itemType, bool withJockerSlot = true)
		{
			if (jockerBackpackSlot.isInfinity && withJockerSlot)
			{
				return int.MaxValue;
			}
			int num = withJockerSlot ? (jockerBackpackSlot.slotCapacity - jockerSlot.Count) : 0;
			if (!itemType2Slot.ContainsKey(itemType))
			{
				return num;
			}
			int num2 = num;
			BackpackSlot backpackSlot = itemType2Slot[itemType];
			return num2 + (backpackSlot.slotCapacity - typedSlots[itemType].Count);
		}

		public bool TryPlace(ItemDefinition itemDefinition)
		{
			ItemsManager.Item item = Manager.Get<ItemsManager>().GetItem(itemDefinition.gameObject);
			if (item == null)
			{
				UnityEngine.Debug.LogError("Can't find item: " + itemDefinition.gameObject.name + " in ItemManager");
				return false;
			}
			return TryPlace(itemDefinition.itemType, itemDefinition.level, item.id);
		}

		public bool TryPlace(ItemType itemType, int level, string itemId, bool useJocker = true)
		{
			if (string.IsNullOrEmpty(itemId))
			{
				return false;
			}
			if (GetAvailablePlace(itemType, useJocker) < 1)
			{
				return false;
			}
			if (!itemType2Slot.ContainsKey(itemType))
			{
				if (!useJocker)
				{
					return false;
				}
				PlaceToJocker(new BackpackItem(itemType, level, itemId));
				isDirty = true;
				return true;
			}
			if (GetAvailablePlace(itemType, withJockerSlot: false) > 0)
			{
				typedSlots[itemType].Add(new BackpackItem(itemType, level, itemId));
				isDirty = true;
				return true;
			}
			if (useJocker)
			{
				PlaceToJocker(new BackpackItem(itemType, level, itemId));
				isDirty = true;
				return true;
			}
			return false;
		}

		public int GetItemsCount()
		{
			return GetItemsCount((ItemType type) => true, (BackpackItem item) => true);
		}

		public int GetItemsCount(ItemType itemType)
		{
			return GetItemsCount((ItemType type) => itemType == type, (BackpackItem item) => true);
		}

		public int GetItemsCount(ItemType itemType, string id)
		{
			return GetItemsCount((ItemType type) => type == itemType, (BackpackItem item) => item.itemId.Equals(id));
		}

		public int GetItemCount(string id)
		{
			return GetItemsCount((ItemType type) => true, (BackpackItem item) => item.itemId.Equals(id));
		}

		public int GetItemsCount(Func<ItemType, bool> itemTypePredicate, Func<BackpackItem, bool> itemPredicate)
		{
			int num = 0;
			foreach (ItemType key in itemType2Slot.Keys)
			{
				if (itemTypePredicate(key))
				{
					num += typedSlots[key].Count((BackpackItem slot) => itemPredicate(slot));
				}
			}
			foreach (BackpackItem item in jockerSlot)
			{
				if (itemTypePredicate(item.type) && itemPredicate(item))
				{
					num++;
				}
			}
			return num;
		}

		public bool TryTakeItem(BackpackItem backpackItem)
		{
			return TryTakeItem(backpackItem.type, backpackItem.level, backpackItem.itemId);
		}

		public bool TryTakeItem(ItemType itemType, int level, string itemId)
		{
			int num = 0;
			if (itemType2Slot.ContainsKey(itemType))
			{
				List<BackpackItem> list = typedSlots[itemType];
				num = list.FindIndex((BackpackItem item) => item.Equals(itemType, level, itemId));
				if (num != -1)
				{
					list.RemoveAt(num);
					return true;
				}
			}
			num = jockerSlot.FindIndex((BackpackItem item) => item.Equals(itemType, level, itemId));
			if (num != -1)
			{
				jockerSlot.RemoveAt(num);
				return true;
			}
			return false;
		}

		public bool ConsumeRequirements(UpgradeRequirements requirements)
		{
			for (int i = 0; i < requirements.itemsIds.Count; i++)
			{
				ItemDefinition componentInChildren = Manager.Get<ItemsManager>().GetPrefab(requirements.itemsIds[i]).GetComponentInChildren<ItemDefinition>();
				for (int j = 0; j < requirements.itemsCount[i]; j++)
				{
					if (!TryTakeItem(componentInChildren.itemType, -1, requirements.itemsIds[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool MeetRequirements(UpgradeRequirements requirements)
		{
			for (int i = 0; i < requirements.itemsIds.Count; i++)
			{
				ItemDefinition componentInChildren = Manager.Get<ItemsManager>().GetPrefab(requirements.itemsIds[i]).GetComponentInChildren<ItemDefinition>();
				if (GetItemsCount(componentInChildren.itemType, requirements.itemsIds[i]) < requirements.itemsCount[i])
				{
					return false;
				}
			}
			return true;
		}

		private void Init()
		{
			PopulateDictonaries();
		}

		private void PopulateDictonaries()
		{
			itemType2Slot.Clear();
			typedSlots.Clear();
			jockerSlot.Clear();
			for (int i = 0; i < backpackSlots.Length; i++)
			{
				itemType2Slot.Add(backpackSlots[i].slotType, backpackSlots[i]);
			}
			if (!LoadData())
			{
				for (int j = 0; j < backpackSlots.Length; j++)
				{
					typedSlots.Add(backpackSlots[j].slotType, new List<BackpackItem>());
				}
			}
		}

		private void Update()
		{
			if (isDirty)
			{
				SaveData();
				isDirty = false;
			}
		}

		private bool LoadData()
		{
			if (PlayerPrefs.HasKey("jocker.json"))
			{
				jockerSlot = JsonUtility.FromJson<BackpackItemWrapper>(PlayerPrefs.GetString("jocker.json")).list;
			}
			return false;
		}

		private void SaveData()
		{
			string value = JsonUtility.ToJson(new BackpackItemWrapper(jockerSlot));
			PlayerPrefs.SetString("jocker.json", value);
			PlayerPrefs.Save();
		}

		private void PlaceToJocker(BackpackItem item)
		{
			jockerSlot.Add(item);
		}

		public void OnGameSavedFrequent()
		{
			SaveData();
		}

		public void OnGameSavedInfrequent()
		{
		}

		public void OnGameplayStarted()
		{
		}

		public void OnGameplayRestarted()
		{
			Init();
		}

		public Dictionary<ItemType, List<BackpackItem>> GetSlotsInfo()
		{
			return typedSlots;
		}

		public List<BackpackItem> GetJockers()
		{
			return jockerSlot;
		}
	}
}
