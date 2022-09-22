// DecompilerFi decompiler from Assembly-CSharp.dll class: RandomWeights
using System.Collections.Generic;
using UnityEngine;

public class RandomWeights
{
	public static int GetWeightedRandom(List<float> weights)
	{
		float num = 0f;
		List<float> list = new List<float>(weights.Count);
		foreach (float weight in weights)
		{
			float num2 = weight;
			num += num2;
			list.Add(num);
		}
		float num3 = Random.Range(0f, num);
		for (int i = 0; i < list.Count; i++)
		{
			if (num3 < list[i])
			{
				return i;
			}
		}
		return list.Count;
	}

	public static void Test()
	{
		List<float> list = new List<float>();
		list.Add(10f);
		list.Add(20f);
		list.Add(30f);
		list.Add(40f);
		List<float> weights = list;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int i = 0; i < 10000; i++)
		{
			dictionary.AddToValueOrCreate(GetWeightedRandom(weights), 1);
		}
		foreach (KeyValuePair<int, int> item in dictionary)
		{
			UnityEngine.Debug.LogErrorFormat("{0} was {1}%", item.Key, (float)item.Value / 10000f);
		}
	}
}
