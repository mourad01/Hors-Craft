// DecompilerFi decompiler from Assembly-CSharp.dll class: AdditionalPrefabsResourcesSpawner
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalPrefabsResourcesSpawner : MonoBehaviour
{
	[Serializable]
	public class ResourceConfig
	{
		public int resourceId;

		public GameObject prefab;
	}

	public List<ResourceConfig> configs = new List<ResourceConfig>();

	private Dictionary<int, List<GameObject>> id2GameObject;

	private void Start()
	{
		if (Manager.Contains<CraftingManager>() && configs.Count > 0)
		{
			CraftingManager craftingManager = Manager.Get<CraftingManager>();
			CraftingManager craftingManager2 = craftingManager;
			craftingManager2.onResourceSpawn = (Action<int, Vector3>)Delegate.Remove(craftingManager2.onResourceSpawn, new Action<int, Vector3>(ResourceSpawn));
			CraftingManager craftingManager3 = craftingManager;
			craftingManager3.onResourceSpawn = (Action<int, Vector3>)Delegate.Combine(craftingManager3.onResourceSpawn, new Action<int, Vector3>(ResourceSpawn));
			PrepareDictionary();
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void PrepareDictionary()
	{
		id2GameObject = new Dictionary<int, List<GameObject>>();
		for (int i = 0; i < configs.Count; i++)
		{
			if (id2GameObject.ContainsKey(configs[i].resourceId))
			{
				id2GameObject[configs[i].resourceId].Add(configs[i].prefab);
			}
			else
			{
				id2GameObject.Add(configs[i].resourceId, new List<GameObject>
				{
					configs[i].prefab
				});
			}
		}
	}

	private void ResourceSpawn(int id, Vector3 position)
	{
		if (id2GameObject.ContainsKey(id))
		{
			List<GameObject> list = id2GameObject[id];
			for (int i = 0; i < list.Count; i++)
			{
				UnityEngine.Object.Instantiate(list[i], position + UnityEngine.Random.insideUnitSphere, Quaternion.identity);
			}
		}
	}
}
