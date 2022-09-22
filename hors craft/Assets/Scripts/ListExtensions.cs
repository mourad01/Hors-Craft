// DecompilerFi decompiler from Assembly-CSharp.dll class: ListExtensions
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListExtensions
{
	public static bool IsNullOrEmpty<T>(this T[] data)
	{
		return data == null || data.Length == 0;
	}

	public static bool IsNullOrEmpty<T>(this List<T> data)
	{
		return data == null || data.Count == 0;
	}

	public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1, T2> data)
	{
		return data == null || data.Count == 0;
	}

	public static IEnumerable<T> RemoveDuplicates<T>(this ICollection<T> list, Func<T, int> Predicate)
	{
		Dictionary<int, T> dictionary = new Dictionary<int, T>();
		foreach (T item in list)
		{
			if (!dictionary.ContainsKey(Predicate(item)))
			{
				dictionary.Add(Predicate(item), item);
			}
		}
		return dictionary.Values.AsEnumerable();
	}

	public static T DequeueOrNull<T>(this Queue<T> q)
	{
		try
		{
			return (q.Count <= 0) ? default(T) : q.Dequeue();
		}
		catch (Exception)
		{
			return default(T);
		}
	}

	public static T GetLastItem<T>(this List<T> list)
	{
		if (list.Count > 0)
		{
			return list[list.Count - 1];
		}
		return default(T);
	}

	public static T GetRandomItem<T>(this List<T> list)
	{
		if (list.Count == 0)
		{
			return default(T);
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T GetAndRemoveRandomItem<T>(this List<T> list)
	{
		int index = UnityEngine.Random.Range(0, list.Count);
		T result = list[index];
		list.RemoveAt(index);
		return result;
	}

	public static void AddMultiple<T>(this List<T> list, params T[] toAdd)
	{
		list.AddRange(toAdd);
	}
}
