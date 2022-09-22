// DecompilerFi decompiler from Assembly-CSharp.dll class: MobsContainer
using Common.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobsContainer : MonoBehaviour
{
	public List<GameObject> mobList;

	private List<MobsManager.MobSpawnConfig> startingWorldMobs;

	private Dictionary<string, GameObject> namesToMobs;

	private void Awake()
	{
		namesToMobs = new Dictionary<string, GameObject>();
		foreach (GameObject mob in mobList)
		{
			namesToMobs[mob.name] = mob;
		}
	}

	public Sprite GetMobImage(string name)
	{
		if (namesToMobs.ContainsKey(name))
		{
			return namesToMobs[name].GetComponent<Mob>().mobSprite;
		}
		return null;
	}

	public void OnManagerInitialized(MobsManager.MobSpawnConfig[] currentConfigs)
	{
		if (startingWorldMobs == null)
		{
			startingWorldMobs = new List<MobsManager.MobSpawnConfig>();
			foreach (MobsManager.MobSpawnConfig item in currentConfigs)
			{
				startingWorldMobs.Add(item);
			}
		}
	}

	public void SetUpManagerForAnimals(List<SavedWorldManager.Animal> animals)
	{
		if (animals == null)
		{
			if (startingWorldMobs != null)
			{
				Manager.Get<MobsManager>().spawnConfigs = startingWorldMobs.ToArray();
			}
			else
			{
				Manager.Get<MobsManager>().spawnConfigs = new MobsManager.MobSpawnConfig[0];
			}
		}
		else
		{
			List<MobsManager.MobSpawnConfig> list = new List<MobsManager.MobSpawnConfig>();
			foreach (SavedWorldManager.Animal animal in animals)
			{
				GameObject mobByName = GetMobByName(animal.name);
				if (!(mobByName == null))
				{
					list.Add(animal.createSpawnConfig(mobByName));
				}
			}
			Manager.Get<MobsManager>().spawnConfigs = list.ToArray();
		}
		Manager.Get<MobsManager>().Init();
	}

	public GameObject GetMobByName(string name)
	{
		if (namesToMobs.ContainsKey(name))
		{
			return namesToMobs[name];
		}
		return null;
	}

	public void AddToList(GameObject mob)
	{
		if (mobList == null)
		{
			mobList = new List<GameObject>();
		}
		mobList.Add(mob);
	}

	public void AddToList(List<GameObject> mob)
	{
		if (mobList == null)
		{
			mobList = mob;
			return;
		}
		HashSet<GameObject> hashSet = new HashSet<GameObject>(mobList);
		hashSet.UnionWith(mob);
		mobList = hashSet.ToList();
	}
}
