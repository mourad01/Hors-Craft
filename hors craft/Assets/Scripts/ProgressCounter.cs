// DecompilerFi decompiler from Assembly-CSharp.dll class: ProgressCounter
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressCounter : MonoBehaviourSingleton<ProgressCounter>
{
	public enum Countables
	{
		MINIGAMES_PLAYED,
		ITEMS_CRAFTED,
		BLUEPRINTS_FINISHED,
		BLOCK_PLACED,
		MATERIALS_USED_FOR_CRAFTING
	}

	[Serializable]
	public class CountedItem
	{
		public string itemType;

		[SerializeField]
		private int expPerIncrement;

		public Action<int> onAdd;

		public CountedItem(string key, int expPerInc, Action<int> onAdd = null)
		{
			itemType = key;
			expPerIncrement = expPerInc;
			this.onAdd = onAdd;
		}

		public int GetExpPerIncrement()
		{
			int experiencePer = Manager.Get<ModelManager>().progressSettings.GetExperiencePer(itemType, -1);
			return (experiencePer < 0) ? expPerIncrement : experiencePer;
		}
	}

	[SerializeField]
	private List<CountedItem> countedItems = new List<CountedItem>();

	private string GetKey(Countables item)
	{
		return GetKey(GetString(item));
	}

	private string GetKey(string item)
	{
		return "count." + item;
	}

	public void CreateOrUpdateCountedItem(string key, Action<int> onAdd)
	{
		CreateOrUpdateCountedItem(key, 0, onAdd);
	}

	public void CreateOrUpdateCountedItem(string key, int expPerInc = 0, Action<int> onAdd = null)
	{
		int num = countedItems.FindIndex((CountedItem i) => i.itemType == key);
		CountedItem countedItem = new CountedItem(key, expPerInc, onAdd);
		if (num == -1)
		{
			countedItems.Add(countedItem);
		}
		else
		{
			countedItems[num] = countedItem;
		}
	}

	public int GetCountFor(Countables item)
	{
		return GetCountFor(GetString(item));
	}

	public int GetCountFor(string item)
	{
		return PlayerPrefs.GetInt(GetKey(item), 0);
	}

	public void Increment(Countables item)
	{
		Increment(GetString(item));
	}

	public void Increment(string item)
	{
		PlayerPrefs.SetInt(GetKey(item), GetCountFor(item) + 1);
		AddProgress(item);
	}

	public void Add(Countables item, int count)
	{
		Add(GetString(item), count);
	}

	public void Add(string item, int count)
	{
		PlayerPrefs.SetInt(GetKey(item), GetCountFor(item) + count);
		AddProgress(item, count);
	}

	private void AddProgress(Countables item, int count = 1)
	{
		AddProgress(GetString(item), count);
	}

	private void AddProgress(string item, int count = 1)
	{
		CountedItem countedItem = countedItems.FirstOrDefault((CountedItem i) => i.itemType == item);
		if (countedItem != null)
		{
			if (countedItem.onAdd != null)
			{
				countedItem.onAdd(count);
			}
			if (Manager.Contains<ProgressManager>())
			{
				Manager.Get<ProgressManager>().IncreaseExperience(countedItem.GetExpPerIncrement() * count);
			}
		}
	}

	public void AddCallback(Countables countable, Action<int> callback)
	{
		AddCallback(GetString(countable), callback);
	}

	public void AddCallback(string countable, Action<int> callback)
	{
		CountedItem countedItem = countedItems.FirstOrDefault((CountedItem i) => i.itemType == countable);
		if (countedItem != null)
		{
			CountedItem countedItem2 = countedItem;
			countedItem2.onAdd = (Action<int>)Delegate.Combine(countedItem2.onAdd, callback);
		}
	}

	private string GetString(Countables item)
	{
		return item.ToString().ToLower();
	}
}
