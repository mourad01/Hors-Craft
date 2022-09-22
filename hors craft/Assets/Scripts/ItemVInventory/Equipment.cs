// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.Equipment
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ItemVInventory
{
	public class Equipment : MonoBehaviour, IGameCallbacksListener
	{
		[Serializable]
		public class EquipmentSlot
		{
			public ItemType itemType;

			public ItemDefinition itemDefinition;

			public string itemId;
		}

		private const string baseIdKey = "inventory.equipment.id.slot.";

		private const string baseLevelKey = "inventory.equipment.level.slot.";

		[SerializeField]
		private EquipmentSlot[] slots;

		private Backpack backpack;

		private Transform _itemParent;

		private ItemsManager _itemsManager;

		public Transform itemParent
		{
			get
			{
				if (_itemParent == null)
				{
					GameObject gameObject = new GameObject("ItemParent");
					gameObject.transform.SetParent(base.transform);
					_itemParent = gameObject.transform;
				}
				return _itemParent;
			}
		}

		private ItemsManager itemsManager => _itemsManager ?? (_itemsManager = Manager.Get<ItemsManager>());

		private void Start()
		{
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
			backpack = GetComponent<Backpack>();
			Load();
		}

		public bool Equip(ItemDefinition item, int level = 0)
		{
			if (item.CanEquip())
			{
				ItemType itemType = item.itemType;
				for (int i = 0; i < slots.Length; i++)
				{
					if (slots[i].itemType == itemType && slots[i].itemDefinition == null)
					{
						slots[i].itemDefinition = item;
						item.OnEquip(level);
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public bool EquipFromBackPack(ItemType itemType, int level, string itemId)
		{
			if (backpack == null)
			{
				return false;
			}
			if (!backpack.TryTakeItem(itemType, level, itemId))
			{
				return false;
			}
			return Equip(itemId, level);
		}

		public bool Equip(string itemId, int level = 0)
		{
			GameObject prefab = itemsManager.GetPrefab(itemId);
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, itemParent);
			ItemDefinition componentInChildren = gameObject.GetComponentInChildren<ItemDefinition>();
			componentInChildren.Init(itemId);
			if (!Equip(gameObject.GetComponentInChildren<ItemDefinition>(), level))
			{
				UnityEngine.Object.Destroy(gameObject);
				return false;
			}
			return true;
		}

		public bool Unequip(ItemDefinition item)
		{
			for (int i = 0; i < slots.Length; i++)
			{
				if (!(slots[i].itemDefinition == item))
				{
					continue;
				}
				slots[i].itemDefinition = null;
				item.OnUnequip();
				if (backpack != null)
				{
					if (!backpack.TryPlace(item.itemType, item.level, item.id))
					{
						item.OnDrop(1, base.transform.position);
					}
				}
				else
				{
					item.OnDrop(1, base.transform.position);
				}
				return true;
			}
			return false;
		}

		public ItemDefinition[] GetEquipment()
		{
			List<ItemDefinition> list = new List<ItemDefinition>();
			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i].itemDefinition != null)
				{
					list.Add(slots[i].itemDefinition);
				}
			}
			return list.ToArray();
		}

		public ItemDefinition[] GetEquipment(string id)
		{
			List<ItemDefinition> list = new List<ItemDefinition>();
			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i].itemDefinition != null && slots[i].itemDefinition.id.Equals(id))
				{
					list.Add(slots[i].itemDefinition);
				}
			}
			return list.ToArray();
		}

		public ItemDefinition GetFirstEquipment(string id)
		{
			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i].itemDefinition != null && slots[i].itemDefinition.id.Equals(id))
				{
					return slots[i].itemDefinition;
				}
			}
			return null;
		}

		private void Load()
		{
			for (int i = 0; i < slots.Length; i++)
			{
				if (PlayerPrefs.HasKey("inventory.equipment.id.slot." + i) && PlayerPrefs.HasKey("inventory.equipment.level.slot." + i))
				{
					string @string = PlayerPrefs.GetString("inventory.equipment.id.slot." + i);
					int @int = PlayerPrefs.GetInt("inventory.equipment.level.slot." + i);
					Equip(@string, @int);
				}
				else if (!string.IsNullOrEmpty(slots[i].itemId))
				{
					Equip(slots[i].itemId);
				}
			}
		}

		private void Save()
		{
			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i].itemDefinition != null)
				{
					PlayerPrefs.SetString("inventory.equipment.id.slot." + i, slots[i].itemDefinition.id);
					PlayerPrefs.SetInt("inventory.equipment.level.slot." + i, slots[i].itemDefinition.level);
				}
			}
			PlayerPrefs.Save();
		}

		private void OnDestroy()
		{
			Save();
		}

		public void OnGameSavedFrequent()
		{
			Save();
		}

		public void OnGameSavedInfrequent()
		{
		}

		public void OnGameplayStarted()
		{
		}

		public void OnGameplayRestarted()
		{
		}

		public List<ItemType> GetTypes()
		{
			List<ItemType> list = new List<ItemType>();
			for (int i = 0; i < slots.Length; i++)
			{
				if (!(slots[i].itemDefinition == null))
				{
					list.Add(slots[i].itemType);
				}
			}
			return list;
		}
	}
}
