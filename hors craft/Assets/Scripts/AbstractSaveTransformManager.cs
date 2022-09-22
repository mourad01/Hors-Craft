// DecompilerFi decompiler from Assembly-CSharp.dll class: AbstractSaveTransformManager
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSaveTransformManager : Manager
{
	private const float WAIT_TIME = 2f;

	public GameObject[] additionals = new GameObject[0];

	public GameObject[] cores = new GameObject[0];

	public AbstractSTModule[] modules = new AbstractSTModule[0];

	protected Dictionary<string, GameObject> name2Prefab;

	protected List<GameObject> toActivate;

	protected float timer = 2f;

	protected HashSet<AbstractSTModule> dirtyModules = new HashSet<AbstractSTModule>();

	protected virtual void Awake()
	{
		PrepareDictionary();
		toActivate = new List<GameObject>();
		for (int i = 0; i < modules.Length; i++)
		{
			modules[i].Init(this);
		}
	}

	protected virtual void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			timer = 2f;
			TryActivate();
		}
	}

	protected virtual void LateUpdate()
	{
		foreach (AbstractSTModule dirtyModule in dirtyModules)
		{
			dirtyModule.SaveAll();
		}
		dirtyModules.Clear();
	}

	public void AddRuntimePrefab(GameObject prefab, bool overridePrefab = true)
	{
		if (name2Prefab == null)
		{
			name2Prefab = new Dictionary<string, GameObject>(cores.Length + additionals.Length + 1);
		}
		if (name2Prefab.ContainsKey(prefab.name))
		{
			UnityEngine.Debug.LogError("Can't add " + prefab.name + " to SaveTransformManager");
			if (overridePrefab)
			{
				name2Prefab[prefab.name] = prefab;
			}
		}
		else
		{
			name2Prefab.Add(prefab.name, prefab);
		}
	}

	public virtual void ChunkSpawned(IIndexable indexable)
	{
		for (int i = 0; i < modules.Length; i++)
		{
			modules[i].ChunkSpawned(indexable);
		}
	}

	public virtual void ChunkDespawned(IIndexable indexable)
	{
		for (int i = 0; i < modules.Length; i++)
		{
			modules[i].ChunkDespawned(indexable);
		}
	}

	public GameObject GetPrefab(string name)
	{
		if (name2Prefab.ContainsKey(name))
		{
			return name2Prefab[name];
		}
		return null;
	}

	public bool ContainsPrefabFor(string name)
	{
		return name2Prefab.ContainsKey(name);
	}

	public void DespawnAll()
	{
		for (int i = 0; i < modules.Length; i++)
		{
			modules[i].DeleteAll();
		}
	}

	public void EnqueueToActivate(GameObject spawned)
	{
		spawned.SetActive(value: false);
		toActivate.Add(spawned);
	}

	public void AddDirty(AbstractSTModule module)
	{
		dirtyModules.Add(module);
	}

	protected virtual void PrepareDictionary()
	{
		if (name2Prefab == null)
		{
			name2Prefab = new Dictionary<string, GameObject>(cores.Length + additionals.Length);
		}
		if (additionals.Length + cores.Length <= 0)
		{
			UnityEngine.Debug.LogError("ERROR ERROR: CLICK FIND PREFABS BUTTON IN SAVE TRANSFORM MANAGER ");
		}
		for (int i = 0; i < cores.Length; i++)
		{
			name2Prefab.AddIfNotExists(cores[i].GetComponent<AbstractSaveTransform>().PrefabName, cores[i]);
		}
		for (int j = 0; j < additionals.Length; j++)
		{
			name2Prefab.AddIfNotExists(additionals[j].GetComponent<AbstractSaveTransform>().PrefabName, additionals[j]);
		}
	}

	protected abstract void TryActivate();

	protected virtual void SaveAll()
	{
		for (int i = 0; i < modules.Length; i++)
		{
			modules[i].SaveAll();
		}
	}

	protected virtual void LoadAll()
	{
		for (int i = 0; i < modules.Length; i++)
		{
			modules[i].LoadSaved();
		}
	}

	protected virtual void DeleteAll()
	{
		for (int i = 0; i < modules.Length; i++)
		{
			modules[i].DeleteAll();
		}
	}
}
