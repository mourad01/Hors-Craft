// DecompilerFi decompiler from Assembly-CSharp.dll class: Inventory
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	[Serializable]
	public class InitialInventoryItem
	{
		public ushort id;

		public int quantity;
	}

	public const int ITEM_CAP = 64;

	public InitialInventoryItem[] initialItems;

	public string inventoryPath = "inventory.";

	private Dictionary<ushort, int> inventory;

	private ushort currentItemId = 599;

	private void Start()
	{
		inventory = new Dictionary<ushort, int>();
	}

	public void AddItem(ushort id)
	{
		AddItem(id, 1);
	}

	public void AddItem(ushort id, int quantity)
	{
		int value = 0;
		if (!inventory.TryGetValue(id, out value))
		{
			inventory.Add(id, 0);
		}
		value = Mathf.Min(64, value + quantity);
		inventory[id] = value;
	}

	public void RemoveItem(ushort id)
	{
		RemoveItem(id, 1);
	}

	public void RemoveItem(ushort id, int quantity)
	{
		int value = 0;
		if (!inventory.TryGetValue(id, out value))
		{
			inventory.Add(id, 0);
		}
		value = Mathf.Max(0, value - quantity);
		inventory[id] = value;
	}

	public int GetQuantity(ushort id)
	{
		int value = 0;
		inventory.TryGetValue(id, out value);
		return value;
	}

	public void SetCurrentItem(ushort id)
	{
		currentItemId = id;
	}

	public int GetCurrentItemQuantity()
	{
		return GetQuantity(currentItemId);
	}

	public void Save()
	{
		string value = inventory.Keys.ToStringPretty(" ");
		PlayerPrefs.SetString(inventoryPath + "keys", value);
		foreach (KeyValuePair<ushort, int> item in inventory)
		{
			PlayerPrefs.SetInt(inventoryPath + item.Key.ToString(), item.Value);
		}
	}

	public void Load()
	{
		inventory = new Dictionary<ushort, int>();
		string[] array = PlayerPrefs.GetString(inventoryPath + "keys").Split(' ');
		for (int i = 0; i < array.Length; i++)
		{
			ushort result = 0;
			if (ushort.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				int @int = PlayerPrefs.GetInt(inventoryPath + result.ToString(), 0);
				inventory.Add(result, @int);
			}
		}
		InitialInventoryItem[] array2 = initialItems;
		foreach (InitialInventoryItem initialInventoryItem in array2)
		{
			if (!inventory.ContainsKey(initialInventoryItem.id))
			{
				inventory.Add(initialInventoryItem.id, initialInventoryItem.quantity);
			}
		}
	}

	public void Restart()
	{
		inventory = new Dictionary<ushort, int>();
		PlayerPrefs.DeleteKey(inventoryPath + "keys");
	}

	public void Dispose()
	{
	}
}
