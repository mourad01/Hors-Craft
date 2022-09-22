// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectPoolManager
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPoolInventory))]
public class ObjectPoolManager : Manager, IObjectPool
{
	[SerializeField]
	protected ObjectPoolInventory inventory;

	[SerializeField]
	protected ObjectPoolDataItem[] items = new ObjectPoolDataItem[0];

	protected Dictionary<int, List<ObjectPoolItem>> objects = new Dictionary<int, List<ObjectPoolItem>>();

	public static ObjectPoolManager instance;

	[ContextMenu("init")]
	public override void Init()
	{
		if (inventory == null)
		{
			inventory = GetComponent<ObjectPoolInventory>();
			if (inventory == null)
			{
				throw new NullReferenceException("Add ObjectPoolInventory to this Manager!");
			}
		}
		instance = this;
		PrepeareObjectsInPool();
	}

	private void PrepeareObjectsInPool()
	{
		GameObject gameObject = null;
		objects = new Dictionary<int, List<ObjectPoolItem>>();
		for (int i = 0; i < items.Length; i++)
		{
			ObjectPoolItemBase itemFromInventory = inventory.GetItemFromInventory<ObjectPoolItemBase>(items[i].id);
			if (itemFromInventory == null || itemFromInventory.Prefab == null)
			{
				continue;
			}
			items[i].spawnedObjects = new List<ObjectPoolItem>();
			for (int j = 0; j < items[i].maxObjectsToSpawn; j++)
			{
				gameObject = UnityEngine.Object.Instantiate(itemFromInventory.Prefab);
				if (!(gameObject == null))
				{
					ObjectPoolItem objectPoolItem = gameObject.GetComponent<ObjectPoolItem>();
					if (objectPoolItem == null)
					{
						objectPoolItem = gameObject.AddComponent<ObjectPoolItem>();
					}
					items[i].spawnedObjects.Add(objectPoolItem);
					gameObject.transform.parent = base.transform;
				}
			}
			objects.Add(items[i].id, items[i].spawnedObjects);
		}
	}

	[ContextMenu("Remove item")]
	public bool GetObject(int id, out ObjectPoolItem newObject)
	{
		newObject = null;
		if (!objects.ContainsKey(id))
		{
			return false;
		}
		if (objects[id] == null)
		{
			return false;
		}
		if (objects[id].Count <= 0)
		{
			return false;
		}
		newObject = objects[id][0];
		newObject.Init(id);
		objects[id].RemoveAt(0);
		return true;
	}

	[ContextMenu("Return item")]
	public bool ReturnObject(int id, ObjectPoolItem objectToReturn)
	{
		if (!objects.ContainsKey(id))
		{
			return false;
		}
		if (objects[id] == null)
		{
			return false;
		}
		objects[id].Add(objectToReturn);
		return true;
	}
}
