// DecompilerFi decompiler from Assembly-CSharp.dll class: SerializableDictionary
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
	public List<TKey> keysItems;

	public List<TValue> valueItems;

	[NonSerialized]
	private Dictionary<TKey, TValue> dictionary;

	public TValue this[TKey key]
	{
		get
		{
			return dictionary[key];
		}
		set
		{
			dictionary[key] = value;
		}
	}

	public SerializableDictionary()
	{
		dictionary = new Dictionary<TKey, TValue>();
	}

	public void OnBeforeSerialize()
	{
		keysItems = new List<TKey>();
		valueItems = new List<TValue>();
		foreach (KeyValuePair<TKey, TValue> item in dictionary)
		{
			keysItems.Add(item.Key);
			valueItems.Add(item.Value);
		}
	}

	public void OnAfterDeserialize()
	{
		dictionary = new Dictionary<TKey, TValue>();
		if (keysItems != null && valueItems != null)
		{
			for (int i = 0; i < keysItems.Count && keysItems[i] != null; i++)
			{
				dictionary[keysItems[i]] = valueItems[i];
			}
		}
	}

	public Dictionary<TKey, TValue> GetDict()
	{
		return dictionary;
	}

	public void Add(TKey key, TValue value)
	{
		dictionary.Add(key, value);
	}

	public bool AddIfCan(TKey key, TValue value)
	{
		if (Contains(key))
		{
			return false;
		}
		Add(key, value);
		return true;
	}

	public bool Contains(TKey key)
	{
		return dictionary.ContainsKey(key);
	}
}
