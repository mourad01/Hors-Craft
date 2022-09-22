// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.ItemsManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemVInventory
{
	public class ItemsManager : Manager
	{
		[Serializable]
		public class Item
		{
			public GameObject prefab;

			public string id;
		}

		public class DropConfig
		{
			public string itemId;

			public int amount;
		}

		[Serializable]
		public struct DropTableBase
		{
			public int itemDrop;

			public int currencyDrop;
		}

		[Serializable]
		public struct DropTableModifier
		{
			public float itemTierModifier;

			public float currencyTierModifier;
		}

		[SerializeField]
		private ItemType testDropType;

		[SerializeField]
		private Item[] items;

		[SerializeField]
		private GameObject dropPrefab;

		private List<DropTableBase> baseDrop = new List<DropTableBase>();

		private List<DropTableModifier> tierModifier = new List<DropTableModifier>();

		private Dictionary<string, GameObject> id2Prefab = new Dictionary<string, GameObject>();

		private Dictionary<ItemType, List<GameObject>> type2Prefabs = new Dictionary<ItemType, List<GameObject>>();

		public override void Init()
		{
			InitDictionary();
			InitPrefabs();
		}

		public void InitDropTable()
		{
			ItemsDropModule itemsDropSettings = Manager.Get<ModelManager>().itemsDropSettings;
			for (int i = 0; itemsDropSettings.HasBaseIndex(i); i++)
			{
				DropTableBase item = default(DropTableBase);
				item.itemDrop = itemsDropSettings.GetBaseDrop(i, 0);
				item.currencyDrop = itemsDropSettings.GetBaseCurrencyDrop(i, 0);
				baseDrop.Add(item);
			}
			for (int i = 0; itemsDropSettings.HasTierIndex(i); i++)
			{
				DropTableModifier item2 = default(DropTableModifier);
				item2.itemTierModifier = itemsDropSettings.GetTierModifier(i, 0f);
				item2.currencyTierModifier = itemsDropSettings.GetTierCurrencyModifier(i, 0f);
				tierModifier.Add(item2);
			}
		}

		private void InitDictionary()
		{
			id2Prefab.Clear();
			for (int i = 0; i < items.Length; i++)
			{
				if (id2Prefab.ContainsKey(items[i].id))
				{
					UnityEngine.Debug.LogError("ITEMS MANAGER HAS DUPLICATED IDS!!! FIX IT NOW NOW NOW!");
				}
				else
				{
					id2Prefab.Add(items[i].id, items[i].prefab);
				}
				ItemType itemType = items[i].prefab.GetComponent<ItemDefinition>().itemType;
				if (type2Prefabs.ContainsKey(itemType))
				{
					type2Prefabs[itemType].Add(items[i].prefab);
				}
				else
				{
					type2Prefabs.Add(itemType, new List<GameObject>
					{
						items[i].prefab
					});
				}
			}
		}

		public GameObject GetDropPrefab()
		{
			return dropPrefab;
		}

		public GameObject GetPrefab(string name)
		{
			if (id2Prefab.ContainsKey(name))
			{
				return id2Prefab[name];
			}
			UnityEngine.Debug.LogError($"ITEMS MANAGER YOU ASK FOR PREFAB FOR {name} BUT IT DOES NOT EXIST");
			return null;
		}

		private void InitPrefabs()
		{
			for (int i = 0; i < items.Length; i++)
			{
				items[i].prefab.GetComponent<ItemDefinition>().Init(items[i].id);
			}
		}

		public List<GameObject> GetPrefabsByType(ItemType type)
		{
			List<GameObject> value = new List<GameObject>();
			type2Prefabs.TryGetValue(type, out value);
			return value;
		}

		public List<string> GetIdsByType(ItemType type)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, GameObject> item in id2Prefab)
			{
				if (item.Value.GetComponentInChildren<ItemDefinition>().itemType == type)
				{
					list.Add(item.Key);
				}
			}
			return list;
		}

		public Item[] GetAllItems()
		{
			return items;
		}

		public Item GetItem(GameObject prefab)
		{
			return GetAllItems().FirstOrDefault((Item i) => i.prefab == prefab);
		}

		public Item GetItem(string id)
		{
			return GetAllItems().FirstOrDefault((Item i) => i.id == id);
		}

		public List<DropConfig> TryGetDropConfig(GameObject lootable, int monsterIndex, int tier)
		{
			DropTableBase dropTableBase = baseDrop[monsterIndex];
			float num = dropTableBase.itemDrop;
			DropTableModifier dropTableModifier = tierModifier[tier];
			int num2 = (int)(num * dropTableModifier.itemTierModifier);
			List<string> idsByType = GetIdsByType(testDropType);
			List<DropConfig> list = new List<DropConfig>();
			for (int i = 0; i < num2; i++)
			{
				list.Add(new DropConfig
				{
					amount = 1,
					itemId = idsByType.Random()
				});
			}
			List<DropConfig> list2 = list;
			DropConfig dropConfig = new DropConfig();
			DropConfig dropConfig2 = dropConfig;
			DropTableBase dropTableBase2 = baseDrop[monsterIndex];
			float num3 = dropTableBase2.currencyDrop;
			DropTableModifier dropTableModifier2 = tierModifier[tier];
			dropConfig2.amount = (int)(num3 * dropTableModifier2.currencyTierModifier);
			dropConfig.itemId = "Currency";
			list2.Add(dropConfig);
			return list;
		}

		public void Drop(List<DropConfig> dropConfigs, Vector3 position)
		{
			for (int i = 0; i < dropConfigs.Count; i++)
			{
				GameObject prefab = GetPrefab(dropConfigs[i].itemId);
				prefab.GetComponentInChildren<ItemDefinition>().OnDrop(dropConfigs[i].amount, position);
			}
		}

		public int GetUpgradeCurrencyRequirements(ItemDefinition itemDefinition)
		{
			return Manager.Get<ModelManager>().itemsUpgradeRequirementsSettings.GetCurrencyRequired(itemDefinition.level + 1, (int)Mathf.Pow(64f, itemDefinition.level + 1));
		}
	}
}
