// DecompilerFi decompiler from Assembly-CSharp.dll class: SpawnerPool
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnerPool
{
	public string PoolName = string.Empty;

	public GameObject SpawnObj;

	private readonly Stack AvailableObjects = new Stack();

	private readonly List<GameObject> SpawnedObjects = new List<GameObject>();

	[CompilerGenerated]
	private static Func<GameObject, bool> _003C_003Ef__mg_0024cache0;

	public SpawnerPool(string poolName, GameObject objToSpawn)
	{
		PoolName = poolName;
		SpawnObj = objToSpawn;
	}

	public GameObject Spawn(Vector3 position, Quaternion rotation)
	{
		return Spawn(position, rotation, null);
	}

	public GameObject Spawn(Vector3 position, Quaternion rotation, Action<GameObject> spawnedAction)
	{
		GameObject gameObject;
		if (AvailableObjects.Count == 0)
		{
			gameObject = UnityEngine.Object.Instantiate(SpawnObj, position, rotation);
			gameObject.name = $"{PoolName} ({SpawnedObjects.Count})";
			SpawnedObjects.Add(gameObject);
		}
		else
		{
			gameObject = (AvailableObjects.Pop() as GameObject);
			if (!(gameObject != null))
			{
				return Spawn(position, rotation, spawnedAction);
			}
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
		gameObject.SetActive(value: true);
		spawnedAction?.Invoke(gameObject);
		return gameObject;
	}

	public void Prespawn(int count)
	{
		GameObject[] array = new GameObject[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = Spawn(Vector3.zero, Quaternion.identity);
		}
		for (int j = 0; j < count; j++)
		{
			array[j].Despawn();
		}
	}

	public void Despawn(GameObject go)
	{
		if (!(go == null) && !AvailableObjects.Contains(go))
		{
			AvailableObjects.Push(go);
			go.SetActive(value: false);
		}
	}

	public void DespawnList(List<GameObject> goObjList)
	{
		if (!goObjList.IsNullOrEmpty())
		{
			foreach (GameObject goObj in goObjList)
			{
				Despawn(goObj);
			}
		}
	}

	public void DespawnAll()
	{
		int count = SpawnedObjects.Count;
		if (count != 0)
		{
			for (int i = 0; i < count; i++)
			{
				Despawn(SpawnedObjects[i]);
			}
		}
	}

	public void ClearPool()
	{
		DespawnAll();
		AvailableObjects.Clear();
		SpawnedObjects.Clear();
	}

	public List<GameObject> GetActiveSpawns()
	{
		return SpawnedObjects.Where(UnityGoExtensions.IsActive).ToList();
	}
}
