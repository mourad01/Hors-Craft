// DecompilerFi decompiler from Assembly-CSharp.dll class: Spawner
using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviourSingleton<Spawner>
{
	private static readonly Dictionary<string, SpawnerPool> PoolList = new Dictionary<string, SpawnerPool>();

	public static GameObject CreateSpawn(GameObject go, Vector3 position, Quaternion rotation, string poolName)
	{
		SpawnerPool spawnerPool;
		if (PoolList.ContainsKey(poolName))
		{
			spawnerPool = PoolList[poolName];
		}
		else
		{
			spawnerPool = (SpawnerBreeder.Get(go.name) ?? SpawnerBreeder.Create(poolName, go));
			PoolList.Add(poolName, spawnerPool);
		}
		return spawnerPool.Spawn(position, rotation);
	}

	public static GameObject CreateSpawn(GameObject go, Vector3 position, Quaternion rotation)
	{
		return CreateSpawn(go, position, rotation, go.name);
	}

	public static GameObject CreateSpawn(GameObject go, Vector3 position, Quaternion rotation, string poolName, Action<GameObject> spawnedAction)
	{
		SpawnerPool spawnerPool;
		if (PoolList.ContainsKey(poolName))
		{
			spawnerPool = PoolList[poolName];
		}
		else
		{
			spawnerPool = (SpawnerBreeder.Get(go.name) ?? SpawnerBreeder.Create(poolName, go));
			PoolList.Add(poolName, spawnerPool);
		}
		return spawnerPool.Spawn(position, rotation, spawnedAction);
	}

	public static GameObject CreateSpawn(GameObject go, Vector3 position, Quaternion rotation, Action<GameObject> spawnedAction)
	{
		return CreateSpawn(go, position, rotation, go.name, spawnedAction);
	}

	public static void Despawn(GameObject go)
	{
		string key = go.name.Substring(0, go.name.LastIndexOf('(') - 1);
		if (PoolList.ContainsKey(key))
		{
			PoolList[key].Despawn(go);
		}
		else
		{
			UnityEngine.Object.Destroy(go);
		}
	}

	public static void Prespawn(GameObject go, int count)
	{
		string name = go.name;
		SpawnerPool spawnerPool;
		if (PoolList.ContainsKey(name))
		{
			spawnerPool = PoolList[name];
		}
		else
		{
			spawnerPool = (SpawnerBreeder.Get(name) ?? SpawnerBreeder.Create(name, go));
			PoolList.Add(name, spawnerPool);
		}
		spawnerPool.Prespawn(count);
	}
}
