// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftableList
using Common.Managers;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class CraftableList : MonoBehaviour
{
	[Serializable]
	public class ResourceSpawn
	{
		public class ResourceSpawnComparer : IEqualityComparer<ResourceSpawn>
		{
			public bool Equals(ResourceSpawn x, ResourceSpawn y)
			{
				if (object.ReferenceEquals(x, y))
				{
					return true;
				}
				if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
				{
					return false;
				}
				return x.id == y.id && x.spawnBy == y.spawnBy;
			}

			public int GetHashCode(ResourceSpawn resource)
			{
				if (object.ReferenceEquals(resource, null))
				{
					return 0;
				}
				int num = 0;
				if (resource.spawnBy != null)
				{
					num = resource.spawnBy.GetHashCode();
				}
				return resource.id + num;
			}
		}

		private int _spawnById = -1;

		private int _resourceToSpawnId = -1;

		public int id;

		public Sprite image;

		public GameObject spawnBy;

		public GameObject resourceToSpawn;

		public float probability;

		public bool isSpawnByMob;

		public bool disableRandomSpawning;

		public bool disableShowInResourcesPanel;

		public int timeLimit = 30;

		public int spawnById
		{
			get
			{
				if (spawnBy != null)
				{
					_spawnById = spawnBy.GetComponent<Voxel>().GetUniqueID();
				}
				return _spawnById;
			}
		}

		public int resourceToSpawnId
		{
			get
			{
				if (resourceToSpawn != null)
				{
					_resourceToSpawnId = resourceToSpawn.GetComponent<Voxel>().GetUniqueID();
				}
				else if (image != null)
				{
					_resourceToSpawnId = id;
				}
				return _resourceToSpawnId;
			}
		}

		public ResourceSpawn(int id)
		{
			this.id = id;
		}

		public void SetSpawnBy(GameObject toSpawn)
		{
			if (!(toSpawn == null) && (bool)toSpawn.GetComponent<Voxel>())
			{
				_spawnById = -1;
				spawnBy = toSpawn;
			}
		}

		public void ClearResourceToSpawn()
		{
			_resourceToSpawnId = -1;
			resourceToSpawn = null;
		}

		public void SetResourceToSpawn(GameObject toSpawn)
		{
			if (!(toSpawn == null) && (bool)toSpawn.GetComponent<Voxel>())
			{
				image = null;
				_resourceToSpawnId = -1;
				resourceToSpawn = toSpawn;
			}
		}

		public Sprite GetImage()
		{
			if (image == null)
			{
				Sprite voxelSprite = VoxelSprite.GetVoxelSprite((ushort)resourceToSpawnId);
				if (voxelSprite == null && resourceToSpawn != null)
				{
					voxelSprite = VoxelSprite.GetVoxelSprite(resourceToSpawn.GetComponent<Voxel>());
				}
				return voxelSprite;
			}
			return image;
		}

		public Sprite GetSpawnerImage()
		{
			if (spawnById < 0)
			{
				return null;
			}
			Sprite voxelSprite = VoxelSprite.GetVoxelSprite((ushort)spawnById);
			if (voxelSprite == null && spawnBy != null)
			{
				voxelSprite = VoxelSprite.GetVoxelSprite(spawnBy.GetComponent<Voxel>());
			}
			return voxelSprite;
		}
	}

	public List<Craftable> craftableList;

	public Sprite arrow;

	public QuestList questList;

	private bool isInitialized;

	public List<ResourceSpawn> resourcesList;

	public Dictionary<int, int> blockToResourceIds;

	public Dictionary<int, int> resourceIdsToBlocks;

	private Dictionary<int, List<ResourceSpawn>> spawnForChecking;

	private Dictionary<int, int> resourcesIndexesOnList;

	private int resourceSpawnByMobs = -1;

	public Dictionary<int, int> blockToCraftable;

	private Dictionary<int, Craftable> craftableId;

	private HashSet<int> crafableBlocks;

	private List<Craftable> weapons = new List<Craftable>();

	private List<Craftable> upgradeables = new List<Craftable>();

	public int EggId => resourceSpawnByMobs;

	public void Init()
	{
		InitDictionaries();
		InitResources();
		InitCraftables();
		isInitialized = true;
	}

	private void InitDictionaries()
	{
		spawnForChecking = new Dictionary<int, List<ResourceSpawn>>();
		craftableId = new Dictionary<int, Craftable>();
		blockToResourceIds = new Dictionary<int, int>();
		resourceIdsToBlocks = new Dictionary<int, int>();
		blockToCraftable = new Dictionary<int, int>();
		crafableBlocks = new HashSet<int>();
		resourcesIndexesOnList = new Dictionary<int, int>();
	}

	private void InitResources()
	{
		int num = 0;
		foreach (ResourceSpawn resources in resourcesList)
		{
			if (!spawnForChecking.ContainsKey(resources.spawnById))
			{
				spawnForChecking.Add(resources.spawnById, new List<ResourceSpawn>());
			}
			spawnForChecking[resources.spawnById].Add(resources);
			resourceIdsToBlocks[num] = resources.resourceToSpawnId;
			resourcesIndexesOnList[resources.id] = num;
			if (resources.isSpawnByMob)
			{
				resourceSpawnByMobs = resources.id;
			}
			blockToResourceIds[resources.spawnById] = resources.id;
			num++;
		}
	}

	private void InitCraftables()
	{
		int num = 0;
		List<Craftable> toRemove = new List<Craftable>();
		foreach (Craftable craftable in craftableList)
		{
			craftable.ClearCraftable();
			craftableId[num] = craftable;
			craftableId[num].id = num;
			if (craftable.blockId != -1)
			{
				crafableBlocks.Add(craftable.blockId);
				blockToCraftable[craftable.blockId] = craftable.id;
				if (craftable.craftableType == Craftable.type.Block)
				{
					craftable.craftableObject.GetComponent<Voxel>().blockCategory = Voxel.Category.craftable;
				}
			}
			else if (craftable.craftableType == Craftable.type.Weapon)
			{
				weapons.Add(craftable);
			}
			else if (craftable.craftableType == Craftable.type.Upgradeable)
			{
				upgradeables.Add(craftable);
			}
			else if (craftable.craftableType != Craftable.type.Custom)
			{
				if (craftable.craftableType == Craftable.type.Mob && !string.IsNullOrEmpty(craftable.mobName))
				{
					int engineIndexByName = Manager.Get<MobsManager>().GetEngineIndexByName(craftable.mobName);
					if (engineIndexByName < 0)
					{
						craftableId.Remove(craftable.id);
						toRemove.Add(craftable);
						num++;
						continue;
					}
					Engine.Blocks[engineIndexByName].GetComponent<Voxel>().blockCategory = Voxel.Category.craftable;
					blockToCraftable[engineIndexByName] = craftable.id;
				}
				else
				{
					craftableId.Remove(num);
					toRemove.Add(craftable);
				}
			}
			num++;
		}
		craftableList.RemoveAll((Craftable craftItem) => toRemove.Contains(craftItem));
	}

	public Craftable GetWeaponById(int weaponId)
	{
		foreach (Craftable weapon in weapons)
		{
			if (weapon.weaponId == weaponId)
			{
				return weapon;
			}
		}
		return null;
	}

	public Craftable GetUpgradeableById(int upgradeId)
	{
		foreach (Craftable upgradeable in upgradeables)
		{
			if (upgradeable.upgradeId == upgradeId)
			{
				return upgradeable;
			}
		}
		return null;
	}

	public ResourceSpawn GetResourceDefinition(int id)
	{
		return resourcesList[resourcesIndexesOnList[id]];
	}

	public int GetResourceToSpawn(int blockDestroyed)
	{
		if (!isInitialized)
		{
			Init();
		}
		return DecideWhatToSpawn(blockDestroyed);
	}

	private void TestPropabilites(int blockDestroyed)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int i = 0; i < 1000; i++)
		{
			dictionary.AddToValueOrCreate(DecideWhatToSpawn(blockDestroyed), 1);
		}
		foreach (KeyValuePair<int, int> item in dictionary)
		{
			UnityEngine.Debug.LogError(item.Key + " " + (float)item.Value / 1000f);
		}
	}

	private int DecideWhatToSpawn(int blockDestroyed)
	{
		if (!spawnForChecking.ContainsKey(blockDestroyed))
		{
			return -1;
		}
		float value = UnityEngine.Random.value;
		float[] array = new float[spawnForChecking[blockDestroyed].Count];
		float num = 0f;
		int num2 = 0;
		foreach (ResourceSpawn item in spawnForChecking[blockDestroyed])
		{
			num = (array[num2] = num + item.probability);
			num2++;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (value < array[i])
			{
				return spawnForChecking[blockDestroyed][i].id;
			}
		}
		return -1;
	}

	public int GetSpriteResourceFromBlock(int block)
	{
		return spawnForChecking[block].Find((ResourceSpawn x) => x.image != null).id;
	}

	public bool IsBlockCraftable(int id)
	{
		if (!isInitialized)
		{
			Init();
		}
		return crafableBlocks.Contains(id);
	}

	public bool CraftableIdsContains(int id)
	{
		if (!isInitialized)
		{
			Init();
		}
		return craftableId.ContainsKey(id);
	}

	public Craftable GetCraftable(int id)
	{
		if (!isInitialized)
		{
			Init();
		}
		if (craftableId.ContainsKey(id))
		{
			return craftableId[id];
		}
		return null;
	}

	public Craftable GetRandom()
	{
		return craftableList[UnityEngine.Random.Range(0, craftableList.Count)];
	}
}
