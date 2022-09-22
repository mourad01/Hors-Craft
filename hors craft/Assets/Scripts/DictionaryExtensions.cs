// DecompilerFi decompiler from Assembly-CSharp.dll class: DictionaryExtensions
using System.Collections.Generic;
using System.Linq;

public static class DictionaryExtensions
{
	public static bool AddIfNotExists<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
	{
		if (dict.ContainsKey(key))
		{
			return false;
		}
		dict.Add(key, value);
		return true;
	}

	public static string ToStringArray<TKey, TValue>(this Dictionary<TKey, TValue> dict)
	{
		string text = string.Empty;
		foreach (KeyValuePair<TKey, TValue> item in dict)
		{
			string text2 = text;
			text = text2 + item.Key + " " + item.Value;
		}
		return text;
	}

	public static void AddToValueOrCreate<TKey>(this Dictionary<TKey, int> dict, TKey key, int value)
	{
		if (dict.ContainsKey(key))
		{
			Dictionary<TKey, int> dictionary;
			TKey key2;
			(dictionary = dict)[key2 = key] = dictionary[key2] + value;
		}
		else
		{
			dict.Add(key, value);
		}
	}

	public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
	{
		TValue value;
		return (!dict.TryGetValue(key, out value)) ? defaultValue : value;
	}

	public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
	{
		if (dict.ContainsKey(key))
		{
			dict[key] = value;
		}
		else
		{
			dict.Add(key, value);
		}
	}

	public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<KeyValuePair<TKey, TValue>> kvpList)
	{
		foreach (KeyValuePair<TKey, TValue> kvp in kvpList)
		{
			dict.Add(kvp.Key, kvp.Value);
		}
	}

	public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings)
	{
		return groupings.ToDictionary((IGrouping<TKey, TValue> group) => group.Key, Enumerable.ToList<TValue>);
	}
}
