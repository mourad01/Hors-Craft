// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerItems
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

[Serializable]
public class PlayerItems
{
	[SerializeField]
	private Dictionary<int, int> ownedResources;

	[SerializeField]
	private Dictionary<int, int> ownedCraftable;

	[SerializeField]
	private Dictionary<int, int> ownedBlocks;

	public List<Resource> resources;

	public List<Block> blocks;

	public List<RealBlock> realBlocks;

	public HashSet<int> showedBlocks;

	public List<int> _showedBlocks;

	public HashSet<int> unlockedBlock;

	public List<int> _unlockedBlock;

	public HashSet<int> unlockedCraftablesByAd;

	public List<int> _unlockedCraftablesByAd;

	public HashSet<string> unlockedClothes;

	public List<string> _unlockedClothes;

	public Action<int, int> onResourceChanged;

	public static PlayerItems LoadFromPlayerPrefs()
	{
		string @string = PlayerPrefs.GetString("player.equipment", string.Empty);
		PlayerItems playerItems = (!string.IsNullOrEmpty(@string)) ? JsonUtility.FromJson<PlayerItems>(@string) : new PlayerItems();
		playerItems.initAfterSerialization();
		return playerItems;
	}

	public void SaveToPrefs()
	{
		resources = GetCurrentResources();
		blocks = GetCurrentBlocks();
		realBlocks = GetCurrentRealBlocks();
		_unlockedBlock = new List<int>(unlockedBlock);
		_showedBlocks = new List<int>(showedBlocks);
		_unlockedClothes = new List<string>(unlockedClothes);
		_unlockedCraftablesByAd = new List<int>(unlockedCraftablesByAd);
		PlayerPrefs.SetString("player.equipment", JsonUtility.ToJson(this));
	}

	private void initAfterSerialization()
	{
		ownedResources = new Dictionary<int, int>();
		ownedCraftable = new Dictionary<int, int>();
		ownedBlocks = new Dictionary<int, int>();
		unlockedBlock = new HashSet<int>();
		showedBlocks = new HashSet<int>();
		unlockedClothes = new HashSet<string>();
		unlockedCraftablesByAd = new HashSet<int>();
		if (resources != null)
		{
			foreach (Resource resource in resources)
			{
				ownedResources[resource.id] = resource.count;
			}
		}
		if (realBlocks != null)
		{
			foreach (RealBlock realBlock in realBlocks)
			{
				ownedBlocks[realBlock.id] = realBlock.count;
			}
			MonoBehaviourSingleton<GameplayFacts>.get.AddSignalFact(Fact.BLOCK_COUNT_CHANGED);
		}
		if (blocks != null)
		{
			foreach (Block block in blocks)
			{
				ownedCraftable[block.id] = block.count;
			}
		}
		if (_unlockedBlock != null)
		{
			foreach (int item in _unlockedBlock)
			{
				unlockedBlock.Add(item);
			}
		}
		if (_showedBlocks != null)
		{
			foreach (int showedBlock in _showedBlocks)
			{
				showedBlocks.Add(showedBlock);
			}
		}
		if (_unlockedCraftablesByAd != null)
		{
			foreach (int item2 in _unlockedCraftablesByAd)
			{
				unlockedCraftablesByAd.Add(item2);
			}
		}
		if (_unlockedClothes != null)
		{
			foreach (string unlockedClothe in _unlockedClothes)
			{
				unlockedClothes.Add(unlockedClothe);
			}
		}
	}

	public void Craft(int itemId)
	{
		Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(itemId);
		if (craftable.status == CraftableStatus.Craftable)
		{
			foreach (Resource item in craftable.requiredResourcesToCraft)
			{
				Dictionary<int, int> dictionary;
				int id;
				(dictionary = ownedResources)[id = item.id] = dictionary[id] - item.count;
			}
			foreach (Resource item2 in craftable.requiredCraftableToCraft)
			{
				Dictionary<int, int> dictionary;
				int id2;
				(dictionary = ownedCraftable)[id2 = item2.id] = dictionary[id2] - item2.count;
			}
			ownedCraftable.AddToValueOrCreate(itemId, 1);
			if (Manager.Contains<ProgressManager>())
			{
				foreach (int item3 in craftable.unlockingQuestId)
				{
					Quest quest = Manager.Get<QuestManager>().questById[item3];
					if (quest.type == QuestType.GetXLevel)
					{
						Manager.Get<ProgressManager>().IncreaseExperience(quest.prize);
					}
				}
			}
			if ((bool)craftable.craftableObject && (bool)craftable.craftableObject.GetComponent<AchievementReporter>())
			{
				craftable.craftableObject.GetComponent<AchievementReporter>().Report();
			}
			if ((bool)craftable.craftableObject && (bool)craftable.craftableObject.GetComponent<FishingUpgrade>())
			{
				craftable.craftableObject.GetComponent<FishingUpgrade>().EnableUpgrade();
			}
			if ((bool)craftable.craftableObject && (bool)craftable.craftableObject.GetComponent<IncreaseHospitalPrestige>())
			{
				craftable.craftableObject.GetComponent<IncreaseHospitalPrestige>().Increase();
			}
			int count = craftable.requiredResourcesToCraft.Sum((Resource res) => res.count);
			MonoBehaviourSingleton<ProgressCounter>.get.Add(ProgressCounter.Countables.MATERIALS_USED_FOR_CRAFTING, count);
			MonoBehaviourSingleton<ProgressCounter>.get.Increment(ProgressCounter.Countables.ITEMS_CRAFTED);
			Manager.Get<StatsManager>().ItemCrafted();
			Manager.Get<QuestManager>().OnItemCraft(itemId);
			SaveToPrefs();
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<AbstractAchievementManager>().RegisterEvent("crafted.items");
			}
		}
	}

	public List<Block> GetCurrentBlocks()
	{
		List<Block> list = new List<Block>();
		foreach (KeyValuePair<int, int> item in ownedCraftable)
		{
			list.Add(new Block(item.Key, item.Value));
		}
		return list;
	}

	public List<RealBlock> GetCurrentRealBlocks()
	{
		List<RealBlock> list = new List<RealBlock>();
		foreach (KeyValuePair<int, int> ownedBlock in ownedBlocks)
		{
			list.Add(new RealBlock(ownedBlock.Key, ownedBlock.Value));
		}
		return list;
	}

	public int GetRealBlockCount(int blockId)
	{
		if (ownedBlocks.ContainsKey(blockId))
		{
			return ownedBlocks[blockId];
		}
		return 0;
	}

	public List<Resource> GetCurrentResources()
	{
		List<Resource> list = new List<Resource>();
		foreach (KeyValuePair<int, int> ownedResource in ownedResources)
		{
			list.Add(new Resource(ownedResource.Key, ownedResource.Value));
		}
		return list;
	}

	public int GetResourcesCount(int resourceId)
	{
		if (ownedResources.ContainsKey(resourceId))
		{
			return ownedResources[resourceId];
		}
		return 0;
	}

	public bool IsBlockUnlocked(int blockId)
	{
		if (ownedBlocks.ContainsKey(blockId))
		{
			return true;
		}
		if (Engine.GetVoxelType((ushort)blockId).blockCategory == Voxel.Category.craftable)
		{
			return true;
		}
		if (unlockedBlock != null && unlockedBlock.Contains(blockId))
		{
			return true;
		}
		if (Manager.Get<ModelManager>().blocksUnlocking.IsUnlockedByCurrency() && Manager.Get<AbstractSoftCurrencyManager>().BlockCost((ushort)blockId) <= 0)
		{
			return true;
		}
		return false;
	}

	public bool IsCraftableUnlockedByAd(int craftableId)
	{
		if (unlockedCraftablesByAd != null && unlockedCraftablesByAd.Contains(craftableId))
		{
			return true;
		}
		return false;
	}

	public bool IsItemUnlocked(string skinId)
	{
		if (unlockedClothes != null)
		{
			return unlockedClothes.Contains(skinId);
		}
		return false;
	}

	public void UnlockItemById(string skinId, bool save = true)
	{
		if (unlockedClothes == null)
		{
			unlockedClothes = new HashSet<string>();
		}
		unlockedClothes.Add(skinId);
		if (save)
		{
			SaveToPrefs();
		}
	}

	public string OnItemUnlock(BodyPart part, int skinId, bool save = true)
	{
		if (unlockedClothes == null)
		{
			unlockedClothes = new HashSet<string>();
		}
		string text = $"{skinId}.{part}";
		unlockedClothes.Add(text);
		Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.BuyXClothes);
		if (save)
		{
			SaveToPrefs();
		}
		return text;
	}

	public string OnItemUnlock(int skinId, bool save = true)
	{
		if (unlockedClothes == null)
		{
			unlockedClothes = new HashSet<string>();
		}
		string text = $"{skinId}";
		unlockedClothes.Add(text);
		Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.BuyXClothes);
		if (save)
		{
			SaveToPrefs();
		}
		return text;
	}

	public void OnBlockUnlock(int blockId, bool save = true)
	{
		if (unlockedBlock == null)
		{
			unlockedBlock = new HashSet<int>();
		}
		unlockedBlock.Add(blockId);
		if (save)
		{
			SaveToPrefs();
		}
	}

	public void OnBlockShow(int blockId, bool save = true)
	{
		if (showedBlocks == null)
		{
			showedBlocks = new HashSet<int>();
		}
		showedBlocks.Add(blockId);
		if (save)
		{
			SaveToPrefs();
		}
	}

	public bool WasBlocksShown(int blockId)
	{
		return showedBlocks.Contains(blockId);
	}

	public void OnCraftableUnlockByAd(int craftableId, bool save = true)
	{
		if (unlockedCraftablesByAd == null)
		{
			unlockedCraftablesByAd = new HashSet<int>();
		}
		unlockedCraftablesByAd.Add(craftableId);
		if (save)
		{
			SaveToPrefs();
		}
	}

	public int GetCraftableCountByBlock(int blockId)
	{
		return GetCraftableCount(Manager.Get<CraftingManager>().GetCraftableIdFromBlock(blockId));
	}

	public int GetCraftableCount(int craftableId)
	{
		if (ownedCraftable.ContainsKey(craftableId))
		{
			return ownedCraftable[craftableId];
		}
		if (Manager.Get<CraftingManager>().IsBlockCraftable(craftableId))
		{
			return 0;
		}
		return -1;
	}

	public List<Resource> GetResourcesList()
	{
		List<Resource> list = new List<Resource>();
		foreach (KeyValuePair<int, int> ownedResource in ownedResources)
		{
			if (ownedResource.Value > 0)
			{
				list.Add(new Resource(ownedResource.Key, ownedResource.Value));
			}
		}
		return list;
	}

	public void AddToResources(int resourceId, int count)
	{
		ownedResources.AddToValueOrCreate(resourceId, count);
		SaveToPrefs();
		if (onResourceChanged != null && count != 0)
		{
			onResourceChanged(resourceId, count);
		}
	}

	public void AddToBlocks(int blockId, int count)
	{
		AddCraftable(Manager.Get<CraftingManager>().GetCraftableIdFromBlock(blockId), count);
		SaveToPrefs();
	}

	public void AddCraftable(int id, int count)
	{
		if (count < 0)
		{
			Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(id);
			if (craftable.recipeCategory == Craftable.RecipeCategory.BLUEPRINT)
			{
				Manager.Get<StatsManager>().ItemPlaced("Blueprint", craftable.customCraftableObject.GetComponent<BlueprintCraftableObject>()?.blueprintResourceName ?? "Invalid_Blueprint");
			}
			else
			{
				Manager.Get<StatsManager>().ItemPlaced(craftable.GetStatsCategory(), craftable.GetStatsName());
			}
		}
		ownedCraftable.AddToValueOrCreate(id, count);
		SaveToPrefs();
	}

	public void AddBlock(int id, int count)
	{
		ownedBlocks.AddToValueOrCreate(id, count);
		SaveToPrefs();
		MonoBehaviourSingleton<GameplayFacts>.get.AddSignalFact(Fact.BLOCK_COUNT_CHANGED);
	}

	public void AddResource(int id, int count)
	{
		ownedResources.AddToValueOrCreate(id, count);
		SaveToPrefs();
		if (onResourceChanged != null && count != 0)
		{
			onResourceChanged(id, count);
		}
	}

	public bool TryGetCraftableValue(int key, out int value)
	{
		return ownedCraftable.TryGetValue(key, out value);
	}

	public void UseCraftableBlock(int blockId, int count)
	{
		AddCraftable(Manager.Get<CraftingManager>().GetCraftableIdFromBlock(blockId), -count);
		SaveToPrefs();
	}

	private void Awake()
	{
	}
}
