// DecompilerFi decompiler from Assembly-CSharp.dll class: SpawnerBreeder
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBreeder : MonoBehaviourSingleton<SpawnerBreeder>
{
	private static readonly Dictionary<string, SpawnerPool> SpawnPoolDict = new Dictionary<string, SpawnerPool>();

	public static SpawnerPool Create(string poolName, GameObject obj)
	{
		SpawnerPool spawnerPool = (!SpawnPoolDict.ContainsKey(poolName)) ? new SpawnerPool(poolName, obj) : SpawnPoolDict[poolName];
		SpawnPoolDict.AddIfNotExists(poolName, spawnerPool);
		return spawnerPool;
	}

	public static void Remove(string poolName)
	{
		if (SpawnPoolDict.ContainsKey(poolName))
		{
			SpawnPoolDict.Remove(poolName);
		}
	}

	public static void RemoveAll()
	{
		SpawnPoolDict.Clear();
	}

	public static SpawnerPool Get(string poolName)
	{
		return (!SpawnPoolDict.ContainsKey(poolName)) ? null : SpawnPoolDict[poolName];
	}
}
