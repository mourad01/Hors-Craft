// DecompilerFi decompiler from Assembly-CSharp.dll class: Craftable
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

[Serializable]
public class Craftable
{
	public enum type
	{
		Block,
		Mob,
		Weapon,
		Upgradeable,
		Custom
	}

	public enum RecipeCategory
	{
		BLUEPRINT,
		FURNITURE,
		OTHER
	}

	public RecipeCategory recipeCategory = RecipeCategory.OTHER;

	public GameObject customCraftableObject;

	public GameObject craftableObject;

	public int id;

	public bool isPlacable = true;

	public Sprite sprite;

	public Craftable upgradeTo;

	public bool showInCraftableList = true;

	private int _blockId = -1;

	public type craftableType;

	public string connectedWorldID = string.Empty;

	public string mobName = string.Empty;

	public int worldQuestNeeded;

	public bool spawnResourceWhenDestroyed = true;

	public int mobId;

	public int weaponId;

	public int upgradeId = -1;

	public List<int> unlockingQuestId;

	public List<Resource> requiredResourcesToCraft;

	public List<Resource> requiredCraftableToCraft;

	public int blockId
	{
		get
		{
			if (craftableType == type.Block && _blockId < 0 && craftableObject != null)
			{
				_blockId = craftableObject.GetComponent<Voxel>().GetUniqueID();
			}
			else if (craftableType == type.Mob && mobId >= 0)
			{
				int realMobId = GetRealMobId();
				_blockId = GetMobBlockId(realMobId);
			}
			return _blockId;
		}
	}

	public CraftableStatus status
	{
		get
		{
			if (ShouldBeHidden())
			{
				return CraftableStatus.Hidden;
			}
			if (Singleton<PlayerData>.get.playerItems.IsCraftableUnlockedByAd(id))
			{
				if (HaveEnoughResources() && HaveEnoughCraftable())
				{
					return (!IsFullyUpgraded()) ? CraftableStatus.Craftable : CraftableStatus.FullyUpgraded;
				}
				return CraftableStatus.NoResources;
			}
			if (Manager.Get<ModelManager>().craftingSettings.AreCraftingFree() && recipeCategory != 0)
			{
				return CraftableStatus.Locked;
			}
			if (!IsQuestPassed() || !IsEnoughWorldQuestsPassed())
			{
				return (PlayerPrefs.GetInt("overrideschematicsadsnumber", 0) == 1) ? CraftableStatus.Craftable : CraftableStatus.Locked;
			}
			if (HaveEnoughResources() && HaveEnoughCraftable())
			{
				return (!IsFullyUpgraded()) ? CraftableStatus.Craftable : CraftableStatus.FullyUpgraded;
			}
			return CraftableStatus.NoResources;
		}
	}

	public void SetCustomCraftableObject(GameObject obj)
	{
		if (!(obj == null) && obj.GetComponent<ICustomCraftingItem>() != null)
		{
			customCraftableObject = obj;
		}
	}

	private int GetMobBlockId(int realId)
	{
		if (realId == -1 || Manager.Get<MobsManager>().mobsContainer.mobList[realId].GetComponent<HumanMob>() != null)
		{
			return -1;
		}
		if (Manager.Get<MobsManager>().mobIndexToEngineIndex != null && Manager.Get<MobsManager>().mobIndexToEngineIndex.ContainsKey(realId))
		{
			return Manager.Get<MobsManager>().mobIndexToEngineIndex[realId];
		}
		return _blockId;
	}

	public bool ShouldBeHidden()
	{
		if (!Manager.Get<ModelManager>().worldsSettings.GetCraftRecipesPerWorldEnabled())
		{
			return false;
		}
		if (string.IsNullOrEmpty(connectedWorldID))
		{
			return Manager.Get<ModelManager>().worldsSettings.IsThisGameUltimate();
		}
		if (worldQuestNeeded <= Singleton<PlayerData>.get.playerQuests.GetQuestCountPassedInWorld(connectedWorldID))
		{
			return false;
		}
		if (Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId.Equals(connectedWorldID))
		{
			return false;
		}
		return true;
	}

	public void SetCraftableObject(GameObject obj)
	{
		if (!(obj == null) && !(obj.GetComponent<Voxel>() == null))
		{
			_blockId = -1;
			craftableObject = obj;
		}
	}

	public Sprite GetGraphic()
	{
		if (craftableType == type.Block)
		{
			return (!(sprite == null)) ? sprite : VoxelSprite.GetVoxelSprite((ushort)blockId);
		}
		if (craftableType == type.Mob)
		{
			int num = -1;
			if (mobId >= 0)
			{
				num = GetRealMobId();
			}
			if (num < 0)
			{
				return Engine.Blocks[Manager.Get<MobsManager>().mobNameToEngineIndex[mobName]].voxelSprite;
			}
			return Engine.Blocks[Manager.Get<MobsManager>().mobIndexToEngineIndex[num]].voxelSprite;
		}
		return sprite;
	}

	public void ClearCraftable()
	{
		_blockId = -1;
	}

	public bool ShouldBeActive()
	{
		return status != CraftableStatus.Hidden && showInCraftableList;
	}

	public int GetStatusValue()
	{
		switch (status)
		{
		case CraftableStatus.NoResources:
			return 0;
		case CraftableStatus.Craftable:
			return 0;
		case CraftableStatus.Locked:
			return 1;
		case CraftableStatus.Undefined:
			return 2;
		case CraftableStatus.FullyUpgraded:
			return 3;
		default:
			return 0;
		}
	}

	public string GetName()
	{
		if (craftableObject == null)
		{
			return mobName;
		}
		return craftableObject.name;
	}

	public bool IsQuestPassed()
	{
		return Singleton<PlayerData>.get.playerQuests.IsQuestPassed(unlockingQuestId);
	}

	public bool IsEnoughWorldQuestsPassed()
	{
		return Singleton<PlayerData>.get.playerQuests.GetQuestCountPassedInWorld(connectedWorldID) >= worldQuestNeeded;
	}

	public bool isQuestOnList(int questId)
	{
		return unlockingQuestId.FindIndex((int x) => x == questId) != -1;
	}

	public bool HaveEnoughCraftable()
	{
		if (requiredCraftableToCraft == null)
		{
			return true;
		}
		foreach (Resource item in requiredCraftableToCraft)
		{
			int num = 0;
			num = Singleton<PlayerData>.get.playerItems.GetCraftableCount(item.id);
			if (num < item.count)
			{
				return false;
			}
		}
		return true;
	}

	public bool HaveEnoughResources()
	{
		if (requiredResourcesToCraft == null)
		{
			return true;
		}
		foreach (Resource item in requiredResourcesToCraft)
		{
			int num = 0;
			num = Singleton<PlayerData>.get.playerItems.GetResourcesCount(item.id);
			if (num < item.count)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsFullyUpgraded()
	{
		if (craftableType == type.Upgradeable && upgradeId < 0 && Singleton<PlayerData>.get.playerItems.GetCraftableCount(id) > 0)
		{
			return true;
		}
		return false;
	}

	public ushort TryGetMobBlockId()
	{
		if (!string.IsNullOrEmpty(mobName))
		{
			return (ushort)Manager.Get<MobsManager>().GetEngineIndexByName(mobName);
		}
		return (ushort)blockId;
	}

	public int GetRealMobId()
	{
		if (mobId >= Manager.Get<MobsManager>().spawnConfigs.Length)
		{
			return -1;
		}
		GameObject mobPrefab = Manager.Get<MobsManager>().spawnConfigs[mobId].prefab;
		List<GameObject> mobList = Manager.Get<MobsManager>().mobsContainer.mobList;
		return mobList.FindIndex((GameObject p) => p == mobPrefab);
	}

	public string GetStatsCategory()
	{
		return $"{craftableType.ToString().ToLower()}";
	}

	public string GetStatsName()
	{
		return $"{((craftableType != 0) ? sprite.name.ToLower() : blockId.ToString())}";
	}
}
