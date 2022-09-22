// DecompilerFi decompiler from Assembly-CSharp.dll class: BlocksController
using Common.Managers;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TsgCommon;
using Uniblocks;
using UnityEngine;

public class BlocksController : Singleton<BlocksController>
{
	private BlocksUnlockingModule blocksUnlockingModule;

	private bool categoryBasedAds;

	private List<Sprite> voxelSpritesToUnlockNext;

	private Dictionary<Voxel.Category, List<Voxel>> categoriesWithVoxels = new Dictionary<Voxel.Category, List<Voxel>>();

	public bool enableRarityBlocks;

	public bool enableRarityBlocksNoAds;

	public bool enableLimitedBlocks;

	public int singleAdUnlockMode;

	private Dictionary<Voxel.RarityCategory, List<Voxel>> rarityCategoriesWithVoxels = new Dictionary<Voxel.RarityCategory, List<Voxel>>();

	private PlayerItems playerItems;

	public BlocksUnlockingModule BlocksUnlockingModule => blocksUnlockingModule;

	public bool CategoryBasedAds => categoryBasedAds;

	public List<Sprite> VoxelsSpritesToNextUnlock
	{
		get
		{
			InitVoxelsSpritesToNextUnlock();
			return voxelSpritesToUnlockNext;
		}
	}

	public BlocksController()
	{
		blocksUnlockingModule = Manager.Get<ModelManager>().blocksUnlocking;
		categoryBasedAds = blocksUnlockingModule.CategoryBasedAds();
		enableLimitedBlocks = blocksUnlockingModule.IsBlocksLimitedEnabled();
		enableRarityBlocks = blocksUnlockingModule.IsRarityBlocksEnabled();
		enableRarityBlocksNoAds = blocksUnlockingModule.IsRarityBlocksNoAdsEnabled();
		singleAdUnlockMode = blocksUnlockingModule.GetSingleAdUnlockMode();
		playerItems = Singleton<PlayerData>.get.playerItems;
		LoadBlocksToDict();
		if (enableRarityBlocks)
		{
			InitStartingBlocks();
		}
	}

	public int NoOfWatchedAds(Voxel.Category category)
	{
		return PlayerPrefs.GetInt("numberOfWatchedRewardedAds" + category.ToString().ToLower(), 0);
	}

	public int NoOfWatchedBlocksAds()
	{
		if (PlayerPrefs.GetInt("overrideblocksadsnumber", 0) == 1)
		{
			return 9999;
		}
		return PlayerPrefs.GetInt("numberOfWatchedRewardedAdsBlocks", 0);
	}

	public int NoOfBlocksToUnlockPerAd(Voxel.Category category)
	{
		return blocksUnlockingModule.GetNoOfBlocksToUnlockPerAd(category);
	}

	public int NoOfFreeBlocks(Voxel.Category category)
	{
		return blocksUnlockingModule.GetNoOfFreeBlocks(category);
	}

	public int NoOfUnlockedBlocks(Voxel.Category category)
	{
		return blocksUnlockingModule.GetNoOfFreeBlocks(category) + ((!categoryBasedAds) ? NoOfWatchedBlocksAds() : NoOfWatchedAds(category)) * NoOfBlocksToUnlockPerAd(category);
	}

	public int NoOfUnlockedBlocks(Voxel.RarityCategory category)
	{
		return blocksUnlockingModule.GetNoOfFreeRarityBlocks(category) + NoOfWatchedBlocksAds() * NoOfRarityBlocksToUnlockPerAd(category);
	}

	public int NoOfFreeRarityBlocks(Voxel.RarityCategory category)
	{
		return blocksUnlockingModule.GetNoOfFreeRarityBlocks(category);
	}

	public int NoOfRarityBlocksToUnlockPerAd(Voxel.RarityCategory category)
	{
		return blocksUnlockingModule.GetNoOfRarityBlocksToUnlockPerAd(category);
	}

	public int NoOfMaxRarityBlocksCount(Voxel.RarityCategory category)
	{
		return blocksUnlockingModule.GetNoOfMaxRarityBlocks(category) + PlayerPrefs.GetInt("Max.blocks.increase" + category.ToString(), 0);
	}

	public int CalculateCostOfCategoryUnlock(Voxel.Category category, Voxel.Category currentCategory)
	{
		int num = 0;
		float num2 = 1f - blocksUnlockingModule.GetCategoryUnlockPercent(currentCategory);
		foreach (Voxel item in categoriesWithVoxels[category])
		{
			if (!Singleton<PlayerData>.get.playerItems.IsBlockUnlocked(item.GetUniqueID()))
			{
				num += Manager.Get<AbstractSoftCurrencyManager>().BlockCost(item.GetUniqueID());
			}
		}
		return Mathf.RoundToInt((float)num * num2);
	}

	public List<Voxel> GetVoxelsForCategory(Voxel.Category category)
	{
		if (!categoriesWithVoxels.ContainsKey(category))
		{
			return new List<Voxel>();
		}
		return categoriesWithVoxels[category];
	}

	public List<Voxel> GetVoxelsForRarityCategory(Voxel.RarityCategory category)
	{
		if (!rarityCategoriesWithVoxels.ContainsKey(category))
		{
			return new List<Voxel>();
		}
		return rarityCategoriesWithVoxels[category];
	}

	public List<Voxel> GetUnlockedVoxelsForRarityCategory(Voxel.RarityCategory category)
	{
		if (!rarityCategoriesWithVoxels.ContainsKey(category))
		{
			return new List<Voxel>();
		}
		List<Voxel> list = new List<Voxel>();
		foreach (Voxel item in rarityCategoriesWithVoxels[category])
		{
			if (IsBlocked(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public Voxel GetVoxel(Voxel.Category category, int index)
	{
		return categoriesWithVoxels[category][index];
	}

	public int GetVoxelInListIndex(Voxel voxel)
	{
		if (enableRarityBlocks)
		{
			return rarityCategoriesWithVoxels[voxel.rarityCategory].IndexOf(voxel);
		}
		return categoriesWithVoxels[voxel.blockCategory].IndexOf(voxel);
	}

	public List<Voxel.Category> GetCategories()
	{
		return new List<Voxel.Category>(categoriesWithVoxels.Keys);
	}

	public List<Voxel.RarityCategory> GetRarityCategories()
	{
		return new List<Voxel.RarityCategory>(rarityCategoriesWithVoxels.Keys);
	}

	public int GetCategryLength(Voxel.Category category)
	{
		return categoriesWithVoxels[category].Count;
	}

	public bool IsBlocked(Voxel voxel)
	{
		if (enableRarityBlocksNoAds)
		{
			return !playerItems.IsBlockUnlocked(voxel.GetUniqueID());
		}
		if (enableRarityBlocks)
		{
			if (TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.IsAdsFree)
			{
				return false;
			}
			return GetVoxelInListIndex(voxel) >= NoOfUnlockedBlocks(voxel.rarityCategory);
		}
		return GetVoxelInListIndex(voxel) >= NoOfUnlockedBlocks(voxel.blockCategory);
	}

	public bool IsBlocked(int id)
	{
		if (enableRarityBlocksNoAds)
		{
			return !playerItems.IsBlockUnlocked(id);
		}
		return false;
	}

	public int AdsToUnlock(Voxel voxel)
	{
		if (IsBlocked(voxel))
		{
			if (enableRarityBlocks)
			{
				Voxel.RarityCategory rarityCategory = voxel.rarityCategory;
				int voxelInListIndex = GetVoxelInListIndex(voxel);
				voxelInListIndex -= NoOfUnlockedBlocks(rarityCategory);
				return voxelInListIndex / NoOfRarityBlocksToUnlockPerAd(rarityCategory) + 1;
			}
			Voxel.Category blockCategory = voxel.blockCategory;
			int voxelInListIndex2 = GetVoxelInListIndex(voxel);
			voxelInListIndex2 -= NoOfUnlockedBlocks(blockCategory);
			return voxelInListIndex2 / NoOfBlocksToUnlockPerAd(blockCategory) + 1;
		}
		return 0;
	}

	public bool IsCategoryFullyUnlocked(Voxel.Category category)
	{
		if (categoriesWithVoxels.ContainsKey(category))
		{
			return NoOfUnlockedBlocks(category) > categoriesWithVoxels[category].Count;
		}
		return true;
	}

	public bool IsCategoryFullyUnlocked(Voxel.RarityCategory category)
	{
		if (!rarityCategoriesWithVoxels.ContainsKey(category))
		{
			return true;
		}
		return NoOfUnlockedBlocks(category) > rarityCategoriesWithVoxels[category].Count;
	}

	private void LoadBlocksToDict()
	{
		if (enableRarityBlocks)
		{
			rarityCategoriesWithVoxels = new Dictionary<Voxel.RarityCategory, List<Voxel>>();
		}
		categoriesWithVoxels = new Dictionary<Voxel.Category, List<Voxel>>();
		for (int i = 0; i < Engine.Blocks.Length; i++)
		{
			Voxel voxel = Engine.Blocks[i];
			if (voxel.editorOnly || voxel.blockCategory == Voxel.Category.none)
			{
				continue;
			}
			if (!categoriesWithVoxels.ContainsKey(voxel.blockCategory))
			{
				categoriesWithVoxels.Add(voxel.blockCategory, new List<Voxel>());
			}
			categoriesWithVoxels[voxel.blockCategory].Add(voxel);
			if (enableRarityBlocks && voxel.blockCategory != 0 && voxel.blockCategory != Voxel.Category.craftable)
			{
				if (!rarityCategoriesWithVoxels.ContainsKey(voxel.rarityCategory))
				{
					rarityCategoriesWithVoxels.Add(voxel.rarityCategory, new List<Voxel>());
				}
				rarityCategoriesWithVoxels[voxel.rarityCategory].Add(voxel);
			}
		}
		SortDictionary();
	}

	private void SortDictionary()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(Voxel.Category)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Voxel.Category category = (Voxel.Category)enumerator.Current;
				if (categoriesWithVoxels.ContainsKey(category))
				{
					if (category == Voxel.Category.craftable)
					{
						categoriesWithVoxels[category].Sort(delegate(Voxel alice, Voxel bob)
						{
							Craftable craftableFromBlock = Manager.Get<CraftingManager>().GetCraftableFromBlock(alice.GetUniqueID());
							Craftable craftableFromBlock2 = Manager.Get<CraftingManager>().GetCraftableFromBlock(bob.GetUniqueID());
							if (craftableFromBlock == null || craftableFromBlock2 == null)
							{
								return 0;
							}
							int num = craftableFromBlock.GetStatusValue() - craftableFromBlock2.GetStatusValue();
							return (num == 0) ? (craftableFromBlock.id - craftableFromBlock2.id) : num;
						});
					}
					else
					{
						categoriesWithVoxels[category] = (from v in categoriesWithVoxels[category]
							orderby Engine.EngineInstance.voxelToPriority[v]
							select v).ToList();
					}
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		if (enableRarityBlocks)
		{
			IEnumerator enumerator2 = Enum.GetValues(typeof(Voxel.RarityCategory)).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Voxel.RarityCategory key = (Voxel.RarityCategory)enumerator2.Current;
					if (rarityCategoriesWithVoxels.ContainsKey(key))
					{
						rarityCategoriesWithVoxels[key] = (from v in rarityCategoriesWithVoxels[key]
							orderby Engine.EngineInstance.voxelToRarityPriority[v]
							select v).ToList();
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}
	}

	private void InitVoxelsSpritesToNextUnlock()
	{
		if (voxelSpritesToUnlockNext != null)
		{
			voxelSpritesToUnlockNext.Clear();
		}
		voxelSpritesToUnlockNext = new List<Sprite>();
		if (enableRarityBlocks)
		{
			List<Voxel.RarityCategory> rarityCategories = GetRarityCategories();
			for (int i = 0; i < rarityCategories.Count; i++)
			{
				List<Voxel> voxelsForRarityCategory = GetVoxelsForRarityCategory(rarityCategories[i]);
				for (int j = 0; j < voxelsForRarityCategory.Count; j++)
				{
					if (AdsToUnlock(voxelsForRarityCategory[j]) != 0)
					{
						if (AdsToUnlock(voxelsForRarityCategory[j]) == 1)
						{
							voxelSpritesToUnlockNext.Add(VoxelSprite.GetVoxelSprite(voxelsForRarityCategory[j]));
						}
						if (AdsToUnlock(voxelsForRarityCategory[j]) > 1)
						{
							break;
						}
					}
				}
			}
			return;
		}
		List<Voxel.Category> categories = GetCategories();
		for (int k = 0; k < categories.Count; k++)
		{
			List<Voxel> voxelsForCategory = GetVoxelsForCategory(categories[k]);
			for (int l = 0; l < voxelsForCategory.Count; l++)
			{
				if (AdsToUnlock(voxelsForCategory[l]) != 0)
				{
					if (AdsToUnlock(voxelsForCategory[l]) == 1)
					{
						voxelSpritesToUnlockNext.Add(VoxelSprite.GetVoxelSprite(voxelsForCategory[l]));
					}
					if (AdsToUnlock(voxelsForCategory[l]) > 1)
					{
						break;
					}
				}
			}
		}
	}

	private void AddBlock(int id, int count)
	{
		playerItems.AddBlock(id, count);
	}

	public void DecrementBlockCount(Voxel voxel)
	{
		AddBlock(voxel.GetUniqueID(), -1);
	}

	public void IncrementBlockCount(Voxel voxel)
	{
		AddBlock(voxel.GetUniqueID(), 1);
	}

	public void FillBlockMaxCount(Voxel voxel)
	{
		AddBlock(voxel.GetUniqueID(), NoOfMaxRarityBlocksCount(voxel.rarityCategory));
	}

	public void FillCategoryBlocksMaxCount(Voxel voxel)
	{
		foreach (Voxel item in GetVoxelsForRarityCategory(voxel.rarityCategory))
		{
			if (!IsBlocked(item))
			{
				FillBlockMaxCount(item);
			}
		}
	}

	public void FillAllBlocksMaxCount()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(Voxel.RarityCategory)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Voxel.RarityCategory rarityCategory = (Voxel.RarityCategory)enumerator.Current;
				if (rarityCategoriesWithVoxels.ContainsKey(rarityCategory))
				{
					foreach (Voxel item in GetVoxelsForRarityCategory(rarityCategory))
					{
						if (!IsBlocked(item))
						{
							FillBlockMaxCount(item);
						}
					}
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void FillBlock(Voxel voxel)
	{
		switch (blocksUnlockingModule.GetRarityBlocksRefillType())
		{
		case RefillType.SINGLE:
			FillBlockMaxCount(voxel);
			break;
		case RefillType.CATEGORY:
			FillCategoryBlocksMaxCount(voxel);
			break;
		case RefillType.ALL:
			FillAllBlocksMaxCount();
			break;
		}
	}

	public void UnlockBlock(Voxel voxel)
	{
		if (!playerItems.IsBlockUnlocked(voxel.GetUniqueID()))
		{
			AddBlock(voxel.GetUniqueID(), NoOfMaxRarityBlocksCount(voxel.rarityCategory));
		}
	}

	public bool WasBlockShown(Voxel voxel)
	{
		return playerItems.WasBlocksShown(voxel.GetUniqueID());
	}

	public void SeeBlock(Voxel voxel)
	{
		if (!playerItems.WasBlocksShown(voxel.GetUniqueID()))
		{
			playerItems.OnBlockShow(voxel.GetUniqueID());
		}
	}

	public int GetBlockCount(Voxel voxel)
	{
		return GetBlockCount(voxel.GetUniqueID());
	}

	public int GetBlockCount(ushort id)
	{
		return playerItems.GetRealBlockCount(id);
	}

	public int GetMaxRarityBlocks(Voxel voxel)
	{
		return NoOfMaxRarityBlocksCount(voxel.rarityCategory);
	}

	public void InitStartingBlocks()
	{
		PlayerPrefs.SetInt("Max.blocks.Unlimited." + Voxel.RarityCategory.UNLIMITED.ToString(), 1);
		if (PlayerPrefs.GetInt("added.starting.blocks", 0) == 0)
		{
			List<Voxel.RarityCategory> rarityCategories = GetRarityCategories();
			foreach (Voxel.RarityCategory item in rarityCategories)
			{
				List<Voxel> voxelsForRarityCategory = GetVoxelsForRarityCategory(item);
				for (int i = 0; i < blocksUnlockingModule.GetNoOfFreeRarityBlocks(item) && i < voxelsForRarityCategory.Count; i++)
				{
					UnlockBlock(voxelsForRarityCategory[i]);
				}
			}
			PlayerPrefs.SetInt("added.starting.blocks", 1);
		}
	}

	public void CheckRemoveAds()
	{
		if (TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.IsAdsFree && enableRarityBlocks && enableLimitedBlocks && !CheckIfAllUnlimited())
		{
			foreach (Voxel.RarityCategory rarityCategory in GetRarityCategories())
			{
				PlayerPrefs.SetInt("Max.blocks.Unlimited." + rarityCategory.ToString(), 1);
			}
		}
	}

	private bool CheckIfAllUnlimited()
	{
		foreach (Voxel.RarityCategory rarityCategory in GetRarityCategories())
		{
			if (PlayerPrefs.GetInt("Max.blocks.Unlimited." + rarityCategory.ToString(), 0) == 0)
			{
				return false;
			}
		}
		return true;
	}
}
