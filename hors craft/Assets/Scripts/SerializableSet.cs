// DecompilerFi decompiler from Assembly-CSharp.dll class: SerializableSet
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableSet<T> : ISerializationCallbackReceiver
{
	public List<T> setItems;

	[NonSerialized]
	public HashSet<T> hashSet;

	public SerializableSet()
	{
		hashSet = new HashSet<T>();
	}

	public void OnBeforeSerialize()
	{
		setItems = new List<T>();
		foreach (T item in hashSet)
		{
			setItems.Add(item);
		}
	}

	public void OnAfterDeserialize()
	{
		setItems.ForEach(delegate(T item)
		{
			hashSet.Add(item);
		});
		setItems.Clear();
	}

	public HashSet<T> GetSet()
	{
		return hashSet;
	}
}
