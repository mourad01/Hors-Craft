// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.BlocksUnlockReward
using Common.Gameplay;
using Common.Managers;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class BlocksUnlockReward : Reward
	{
		public Voxel.RarityCategory voxelCategory;

		public LootChestManager.Rarity chestCategory;

		private List<Voxel> voxels;

		public int minAmount = 1;

		public int maxAmount = 3;

		public override void ClaimReward()
		{
			if (voxels == null)
			{
				voxels = new List<Voxel>();
			}
			else
			{
				voxels.Clear();
			}
			minAmount = Manager.Get<ModelManager>().lootSettings.GetBlocksAmountMin(voxelCategory, chestCategory);
			maxAmount = Manager.Get<ModelManager>().lootSettings.GetBlocksAmountMax(voxelCategory, chestCategory);
			float blocksPercentChance = Manager.Get<ModelManager>().lootSettings.GetBlocksPercentChance(voxelCategory, chestCategory);
			List<Voxel> unlockedVoxelsForRarityCategory = Singleton<BlocksController>.get.GetUnlockedVoxelsForRarityCategory(voxelCategory);
			List<Voxel> voxelsForRarityCategory = Singleton<BlocksController>.get.GetVoxelsForRarityCategory(voxelCategory);
			int num = Random.Range(minAmount, maxAmount + 1);
			for (int i = 0; i < num; i++)
			{
				if (!(Random.Range(0f, 1f) > blocksPercentChance))
				{
					Voxel randomItem;
					if (!unlockedVoxelsForRarityCategory.IsNullOrEmpty())
					{
						randomItem = unlockedVoxelsForRarityCategory.GetRandomItem();
						Singleton<BlocksController>.get.UnlockBlock(randomItem);
						unlockedVoxelsForRarityCategory.Remove(randomItem);
					}
					else
					{
						randomItem = voxelsForRarityCategory.GetRandomItem();
						Singleton<BlocksController>.get.FillBlockMaxCount(randomItem);
					}
					voxels.Add(randomItem);
				}
			}
			if (voxels.IsNullOrEmpty())
			{
				amount = 0;
			}
		}

		public override List<Sprite> GetSprites()
		{
			List<Sprite> list = new List<Sprite>();
			foreach (Voxel voxel in voxels)
			{
				list.Add(VoxelSprite.GetVoxelSprite(voxel));
			}
			return list;
		}
	}
}
