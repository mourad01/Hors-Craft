// DecompilerFi decompiler from Assembly-CSharp.dll class: UnitySpawningExtensions
using System;
using UnityEngine;

public static class UnitySpawningExtensions
{
	public static GameObject Spawn(this GameObject go, Vector3 position, Quaternion rotation, string poolName)
	{
		return Spawner.CreateSpawn(go, position, rotation, poolName);
	}

	public static GameObject Spawn(this GameObject go, Vector3 position, Quaternion rotation)
	{
		return Spawner.CreateSpawn(go, position, rotation);
	}

	public static GameObject Spawn(this GameObject go, Vector3 position, Quaternion rotation, Action<GameObject> spawnedAction)
	{
		return Spawner.CreateSpawn(go, position, rotation, spawnedAction);
	}

	public static GameObject Spawn(this GameObject go, Vector3 position, Quaternion rotation, string poolName, Action<GameObject> spawnedAction)
	{
		return Spawner.CreateSpawn(go, position, rotation, poolName, spawnedAction);
	}

	public static void Despawn(this GameObject go)
	{
		Spawner.Despawn(go);
	}

	public static void PreSpawn(this GameObject go, int count)
	{
		Spawner.Prespawn(go, count);
	}
}
