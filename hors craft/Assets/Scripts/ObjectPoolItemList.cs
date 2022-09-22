// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectPoolItemList
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolItemList : ScriptableObject
{
	[SerializeField]
	protected List<ObjectPoolItemBase> items = new List<ObjectPoolItemBase>();

	public bool AddItem(ObjectPoolItemBase newItem)
	{
		if (items.Contains(newItem))
		{
			UnityEngine.Debug.LogError($"Cannot add item. Items list already contains item {newItem.GetType().ToString()}");
			return false;
		}
		if (items.FindIndex((ObjectPoolItemBase f) => f.ItemId == newItem.ItemId) >= 0)
		{
			UnityEngine.Debug.LogError($"Cannot add item. Items list already contains id {newItem.ItemId}");
			return false;
		}
		items.Add(newItem);
		return true;
	}

	public T GetItem<T>(int id) where T : ScriptableObject
	{
		int num = items.FindIndex((ObjectPoolItemBase f) => f.ItemId == id);
		if (num < 0)
		{
			return (T)null;
		}
		if ((UnityEngine.Object)(items[num] as T) == (UnityEngine.Object)null)
		{
			UnityEngine.Debug.Log($"Found item but the type requested is not of the found item({items[num].GetType()})!");
			return (T)null;
		}
		return items[num] as T;
	}
}
