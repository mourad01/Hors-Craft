// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftingManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbag;

public class CraftingManager : Manager, IGameCallbacksListener
{
	[Reorderable]
	public CraftableList[] craftableLists;

	public GameObject lootPrefab;

	public Action<int, Vector3> onResourceSpawn;

	private CraftableList craftableListInstance;

	private bool wasInitialized;

	public void Initialize()
	{
		if (!wasInitialized)
		{
			wasInitialized = true;
			LoadFactionAdditionalCraftableList();
			CreateList();
		}
	}

	public void LoadFactionAdditionalCraftableList()
	{
		if (Manager.Contains<SavedWorldManager>() && Manager.Get<StateMachineManager>().ContainsState(typeof(ChooseFactionState)))
		{
			string name = Manager.Get<SavedWorldManager>().GetCurrentWorld().name;
			string @string = PlayerPrefs.GetString(name + ".faction");
			Faction faction = Resources.Load("Factions/" + @string, typeof(Faction)) as Faction;
			if ((bool)faction)
			{
				CraftableList craftableList = faction.GetCraftableList();
				faction.LoadCraftableList(craftableList);
			}
		}
	}

	public override void Init()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		CraftingSettings craftingSettings = Manager.Get<ModelManager>().craftingSettings;
		craftingSettings.OnModelDownload = (Action)Delegate.Combine(craftingSettings.OnModelDownload, new Action(OnModelDownload));
	}

	public void OnModelDownload()
	{
		if (Manager.Get<ModelManager>().craftingSettings.AreBlueprintsFree())
		{
			foreach (Craftable craftable in craftableListInstance.craftableList)
			{
				if (craftable.recipeCategory == Craftable.RecipeCategory.BLUEPRINT)
				{
					craftable.requiredResourcesToCraft = new List<Resource>();
				}
			}
		}
		if (Manager.Get<ModelManager>().craftingSettings.AreCraftingFree())
		{
			foreach (Craftable craftable2 in craftableListInstance.craftableList)
			{
				if (craftable2.recipeCategory == Craftable.RecipeCategory.FURNITURE || craftable2.recipeCategory == Craftable.RecipeCategory.OTHER)
				{
					craftable2.requiredResourcesToCraft = new List<Resource>();
					craftable2.requiredCraftableToCraft = new List<Resource>();
					craftable2.unlockingQuestId = new List<int>();
				}
			}
		}
	}

	public override Type[] GetDependencies()
	{
		return new Type[1]
		{
			typeof(StateMachineManager)
		};
	}

	public CraftableList GetCraftableListInstance()
	{
		if (!wasInitialized)
		{
			Initialize();
		}
		return craftableListInstance;
	}

	public List<Craftable> GetCraftableList()
	{
		if (!wasInitialized)
		{
			Initialize();
		}
		return craftableListInstance.craftableList;
	}

	public int GetCraftableIdFromBlock(int blockId)
	{
		if (craftableListInstance.blockToCraftable.ContainsKey(blockId))
		{
			return craftableListInstance.blockToCraftable[blockId];
		}
		UnityEngine.Debug.LogWarning("Block set as Craftable but not available in craftableList: " + blockId);
		return -1;
	}

	public Craftable GetCraftableFromBlock(int blockId)
	{
		if (GetCraftableListInstance().blockToCraftable.ContainsKey(blockId))
		{
			return GetCraftable(craftableListInstance.blockToCraftable[blockId]);
		}
		return null;
	}

	public void CreateList()
	{
		GameObject gameObject = craftableLists[0].gameObject;
		craftableListInstance = UnityEngine.Object.Instantiate(gameObject, base.transform).GetComponent<CraftableList>();
		AttachAdditionalCraftableLists();
		craftableListInstance.GetComponent<CraftableList>().Init();
	}

	private void AttachAdditionalCraftableLists()
	{
		if (craftableLists != null)
		{
			craftableListInstance.craftableList = new HashSet<Craftable>(craftableLists.SelectMany((CraftableList craftableList) => craftableList.craftableList)).ToList();
			craftableListInstance.resourcesList = (from a in (from craftableList in craftableLists
					select craftableList.resourcesList).Aggregate((List<CraftableList.ResourceSpawn> left, List<CraftableList.ResourceSpawn> right) => left.Union(right, new CraftableList.ResourceSpawn.ResourceSpawnComparer()).ToList())
				orderby a.id
				select a).ToList();
		}
	}

	public bool IsBlockCraftable(int id)
	{
		return craftableListInstance.blockToCraftable.ContainsKey(id);
	}

	public bool CraftableIdsContains(int id)
	{
		if (!wasInitialized)
		{
			Initialize();
		}
		return craftableListInstance.CraftableIdsContains(id);
	}

	public Sprite GetCraftableGraphic(int id)
	{
		if (!wasInitialized)
		{
			Initialize();
		}
		return craftableListInstance.GetCraftable(id)?.GetGraphic();
	}

	public Craftable GetCraftable(int id)
	{
		if (!wasInitialized)
		{
			Initialize();
		}
		return craftableListInstance.GetCraftable(id);
	}

	public int GetResourceId(int id)
	{
		if (craftableListInstance.blockToResourceIds.ContainsKey(id))
		{
			return craftableListInstance.blockToResourceIds[id];
		}
		return craftableListInstance.GetSpriteResourceFromBlock(id);
	}

	public Sprite GetResourceImage(int id)
	{
		return GetResourceDefinition(id).GetImage();
	}

	public Sprite GetCraftableImage(int id)
	{
		Craftable craftable = GetCraftable(id);
		Sprite sprite = null;
		if (sprite != null)
		{
			sprite = craftable.GetGraphic();
		}
		return sprite;
	}

	public List<Resource> GetResourcesList(bool getOnlyVisibleOnList = false)
	{
		List<Resource> list = new List<Resource>();
		HashSet<int> hashSet = new HashSet<int>();
		foreach (CraftableList.ResourceSpawn resources in craftableListInstance.resourcesList)
		{
			if ((!getOnlyVisibleOnList || !resources.disableShowInResourcesPanel) && !hashSet.Contains(resources.id))
			{
				list.Add(new Resource(resources.id, 0));
				hashSet.Add(resources.id);
			}
		}
		return list;
	}

	public CraftableList.ResourceSpawn GetResourceDefinition(int id)
	{
		return craftableListInstance.GetResourceDefinition(id);
	}

	public int GetBlockId(int id)
	{
		return craftableListInstance.resourceIdsToBlocks[id];
	}

	public int ShouldSpawnEgg()
	{
		return craftableListInstance.EggId;
	}

	public CraftableStatus GetCraftableStatus(int id)
	{
		return craftableListInstance.GetCraftable(id)?.status ?? CraftableStatus.Undefined;
	}

	public int GetResourceToSpawn(int blockDestroyed)
	{
		return craftableListInstance.GetResourceToSpawn(blockDestroyed);
	}

	public void Spawn(ushort voxel, Vector3 lootPosition)
	{
		if (Manager.Get<ModelManager>().craftingSettings.IsCraftingEnabled())
		{
			if (IsBlockCraftable(voxel))
			{
				SpawnCraftable(lootPosition, voxel);
			}
			else
			{
				SpawnResource(lootPosition, GetResourceToSpawn(voxel));
			}
		}
	}

	public void Spawn(Pettable pet, Vector3 lootPosition)
	{
		SpawnResource(lootPosition, (ushort)pet.searchingForVoxel);
	}

	public GameObject SpawnRandomResource(Vector3 position, bool spawnWithRandomForce = false)
	{
		List<CraftableList.ResourceSpawn> array = (from r in craftableListInstance.resourcesList
			where !r.disableRandomSpawning
			select r).ToList();
		int id = array.Random().id;
		return SpawnResource(position, id, spawnWithRandomForce);
	}

	public GameObject SpawnResource(Vector3 lootPosition, int lootId, bool spawnWithRandomForce = false)
	{
		if (lootId < 0)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(lootPrefab);
		gameObject.GetComponent<ResourceSprite>().InitWithResourceId(lootPosition, lootId);
		if (spawnWithRandomForce)
		{
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.AddForce(new Vector3(UnityEngine.Random.Range(-0.4f, 0.4f), 1f, UnityEngine.Random.Range(-0.4f, 0.4f)) * 150f);
			rigidbody.useGravity = false;
			rigidbody.drag = 1f;
		}
		if (onResourceSpawn != null)
		{
			onResourceSpawn(lootId, lootPosition);
		}
		return gameObject;
	}

	private void SpawnCraftable(Vector3 lootPosition, ushort blockId)
	{
		if (blockId >= 0)
		{
			int craftableIdFromBlock = GetCraftableIdFromBlock(blockId);
			if (GetCraftable(craftableIdFromBlock).spawnResourceWhenDestroyed)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(lootPrefab);
				gameObject.GetComponent<ResourceSprite>().InitWithCraftableId(lootPosition, blockId, craftableIdFromBlock);
			}
		}
	}

	public void SpawnCustomCraftable(Vector3 position, Craftable craftable)
	{
		if (Manager.Get<ModelManager>().craftingSettings.IsCraftingEnabled())
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(lootPrefab);
			gameObject.GetComponent<ResourceSprite>().InitWithCustomCraftable(position, craftable);
		}
	}

	public void AddStartingResources()
	{
		if (PlayerPrefs.GetInt("added.bonus.resources", 0) == 0)
		{
			CraftingSettings craftingSettings = Manager.Get<ModelManager>().craftingSettings;
			CraftableList craftableList = GetCraftableListInstance();
			List<CraftableList.ResourceSpawn> resourcesList = craftableList.resourcesList;
			HashSet<int> hashSet = new HashSet<int>(from r in resourcesList
				select r.id);
			foreach (int item in hashSet)
			{
				Singleton<PlayerData>.get.playerItems.AddToResources(item, craftingSettings.GetResourceAmountOnStart(item));
			}
			PlayerPrefs.SetInt("added.bonus.resources", 1);
		}
	}

	public void OnGameplayStarted()
	{
		Initialize();
	}

	public void OnGameplayRestarted()
	{
	}

	public void OnGameSavedFrequent()
	{
	}

	public void OnGameSavedInfrequent()
	{
	}
}
