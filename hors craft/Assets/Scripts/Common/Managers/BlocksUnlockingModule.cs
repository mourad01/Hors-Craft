// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.BlocksUnlockingModule
using Common.Model;
using System;
using System.Collections;
using Uniblocks;

namespace Common.Managers
{
	public class BlocksUnlockingModule : ModelModule
	{
		private string keyNoOfFreeBlocks(Voxel.Category category)
		{
			return "number.free." + category.ToString().ToLower() + ".blocks";
		}

		private string keyNoOfBlocksToUnlockPerAd(Voxel.Category category)
		{
			return "number." + category.ToString().ToLower() + ".unlock.per.ad";
		}

		private string keyCategoryBasedAds()
		{
			return "blocks.unlocking.per.category";
		}

		private string keySCCostOfBlock(ushort uniqueId)
		{
			return $"currency.block.{uniqueId}.unlock";
		}

		private string keySCCategorySale(Voxel.Category category)
		{
			return "currency.block.category." + category.ToString().ToLower();
		}

		private string keyModelOfBlockUnlock()
		{
			return "blocks.unlocking.type";
		}

		private string keyBlocksPlacementRestricted()
		{
			return "blocks.placement.restricted";
		}

		private string keyMaxBlocksToPlace()
		{
			return "max.blocks.to.place";
		}

		private string keyBlocksRefillEnabled()
		{
			return "blocks.refill.enabled";
		}

		private string keyTimeForBlockRefill()
		{
			return "time.for.block.refill";
		}

		private string keyBlocksView3dEnabled()
		{
			return "blocks.view.3d.enabled";
		}

		private string keyBlocksViewColumnsCount()
		{
			return "blocks.view.columns.count";
		}

		private string keyCategoryBlocksViewColumnsCount(Voxel.RarityCategory category)
		{
			return "blocks.view.columns.count." + category.ToString().ToLower();
		}

		private string keyBlocksRarityEnabled()
		{
			return "blocks.rarity.enabled";
		}

		private string keyNoOfFreeRarityBlocks(Voxel.RarityCategory category)
		{
			return "number.free." + category.ToString().ToLower() + ".blocks";
		}

		private string keyNoOfMaxRarityBlocks(Voxel.RarityCategory category)
		{
			return "number.free.max." + category.ToString().ToLower() + ".blocks";
		}

		private string keyNoOfRarityBlocksToUnlockPerAd(Voxel.RarityCategory category)
		{
			return "number." + category.ToString().ToLower() + ".unlock.per.ad";
		}

		private string keyBlocksRarityRefillType()
		{
			return "blocks.rarity.refill.type";
		}

		private string keyBlocksRarityNoAds()
		{
			return "blocks.rarity.no.ads";
		}

		private string keyBlocksLimited()
		{
			return "blocks.limited";
		}

		private string keyBlocksUnlockSingleAd()
		{
			return "blocks.unlock.single.ad";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			IEnumerator enumerator = Enum.GetValues(typeof(Voxel.Category)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Voxel.Category category = (Voxel.Category)enumerator.Current;
					int defaultValue = 4;
					int defaultValue2 = 1;
					if (category == Voxel.Category.basic)
					{
						defaultValue = 16;
						defaultValue2 = 5;
					}
					descriptions.AddDescription(keyNoOfFreeBlocks(category), defaultValue);
					descriptions.AddDescription(keyNoOfBlocksToUnlockPerAd(category), defaultValue2);
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
			descriptions.AddDescription(keyCategoryBasedAds(), 0);
			descriptions.AddDescription(keyModelOfBlockUnlock(), 1);
			descriptions.AddDescription(keyBlocksPlacementRestricted(), 0);
			descriptions.AddDescription(keyMaxBlocksToPlace(), 50);
			descriptions.AddDescription(keyTimeForBlockRefill(), 10f);
			descriptions.AddDescription(keyBlocksRefillEnabled(), 0);
			descriptions.AddDescription(keyBlocksViewColumnsCount(), 7);
			descriptions.AddDescription(keyBlocksView3dEnabled(), 1);
			descriptions.AddDescription(keyBlocksRarityEnabled(), 0);
			descriptions.AddDescription(keyNoOfFreeRarityBlocks(Voxel.RarityCategory.COMMON), 7);
			descriptions.AddDescription(keyNoOfFreeRarityBlocks(Voxel.RarityCategory.RARE), 2);
			descriptions.AddDescription(keyNoOfFreeRarityBlocks(Voxel.RarityCategory.LEGENDARY), 0);
			descriptions.AddDescription(keyNoOfFreeRarityBlocks(Voxel.RarityCategory.UNLIMITED), int.MaxValue);
			descriptions.AddDescription(keyCategoryBlocksViewColumnsCount(Voxel.RarityCategory.COMMON), 7);
			descriptions.AddDescription(keyCategoryBlocksViewColumnsCount(Voxel.RarityCategory.RARE), 6);
			descriptions.AddDescription(keyCategoryBlocksViewColumnsCount(Voxel.RarityCategory.LEGENDARY), 4);
			descriptions.AddDescription(keyCategoryBlocksViewColumnsCount(Voxel.RarityCategory.UNLIMITED), 6);
			descriptions.AddDescription(keyBlocksLimited(), 0);
			descriptions.AddDescription(keyBlocksRarityRefillType(), 0);
			descriptions.AddDescription(keyNoOfMaxRarityBlocks(Voxel.RarityCategory.COMMON), 20);
			descriptions.AddDescription(keyNoOfMaxRarityBlocks(Voxel.RarityCategory.RARE), 10);
			descriptions.AddDescription(keyNoOfMaxRarityBlocks(Voxel.RarityCategory.LEGENDARY), 1);
			descriptions.AddDescription(keyNoOfMaxRarityBlocks(Voxel.RarityCategory.UNLIMITED), int.MaxValue);
			descriptions.AddDescription(keyNoOfRarityBlocksToUnlockPerAd(Voxel.RarityCategory.COMMON), 3);
			descriptions.AddDescription(keyNoOfRarityBlocksToUnlockPerAd(Voxel.RarityCategory.RARE), 2);
			descriptions.AddDescription(keyNoOfRarityBlocksToUnlockPerAd(Voxel.RarityCategory.LEGENDARY), 1);
			descriptions.AddDescription(keyNoOfRarityBlocksToUnlockPerAd(Voxel.RarityCategory.UNLIMITED), int.MaxValue);
			descriptions.AddDescription(keyBlocksRarityNoAds(), 0);
			descriptions.AddDescription(keyBlocksUnlockSingleAd(), 0);
		}

		public override void OnModelDownloaded()
		{
		}

		public bool IsUnlockedByCurrency()
		{
			return GetUnlockType() == ItemsUnlockModel.SoftCurrency;
		}

		public ItemsUnlockModel GetUnlockType()
		{
			return (ItemsUnlockModel)base.settings.GetInt(keyModelOfBlockUnlock());
		}

		public float GetCategoryUnlockPercent(Voxel.Category category)
		{
			if (base.contextIAP.isAdsFree)
			{
				return 1f;
			}
			return base.settings.GetFloat(keySCCategorySale(category), 0.1f);
		}

		public int GetBlockCost(ushort blockUniqueId)
		{
			if (base.contextIAP.isAdsFree)
			{
				return 0;
			}
			return base.settings.GetInt(keySCCostOfBlock(blockUniqueId), 10);
		}

		public int GetNoOfFreeBlocks(Voxel.Category category)
		{
			if (base.contextIAP.isAdsFree)
			{
				return 9999;
			}
			if (category == Voxel.Category.craftable)
			{
				return 9999;
			}
			return base.settings.GetInt(keyNoOfFreeBlocks(category));
		}

		public int GetNoOfBlocksToUnlockPerAd(Voxel.Category category)
		{
			if (base.contextIAP.isAdsFree)
			{
				return 9999;
			}
			if (category == Voxel.Category.craftable)
			{
				return 9999;
			}
			return base.settings.GetInt(keyNoOfBlocksToUnlockPerAd(category));
		}

		public bool CategoryBasedAds()
		{
			return base.settings.GetInt(keyCategoryBasedAds()) == 1;
		}

		public bool BlocksPlacementRestricted()
		{
			return base.settings.GetInt(keyBlocksPlacementRestricted()) == 1;
		}

		public int MaxBlocksToPlace()
		{
			return base.settings.GetInt(keyMaxBlocksToPlace());
		}

		public bool BlocksRefillEnabled()
		{
			return base.settings.GetInt(keyBlocksRefillEnabled()) == 1;
		}

		public float GetTimeForBlockRefill()
		{
			return base.settings.GetFloat(keyTimeForBlockRefill());
		}

		public bool Is3dBlocksViewEnabled()
		{
			return base.settings.GetInt(keyBlocksView3dEnabled()) == 1;
		}

		public bool IsRarityBlocksEnabled()
		{
			return base.settings.GetInt(keyBlocksRarityEnabled()) == 1;
		}

		public int GetBlocksViewColumnsCount()
		{
			return base.settings.GetInt(keyBlocksViewColumnsCount());
		}

		public int GetNoOfFreeRarityBlocks(Voxel.RarityCategory category)
		{
			return base.settings.GetInt(keyNoOfFreeRarityBlocks(category));
		}

		public int GetNoOfMaxRarityBlocks(Voxel.RarityCategory category)
		{
			return base.settings.GetInt(keyNoOfMaxRarityBlocks(category));
		}

		public int GetNoOfRarityBlocksToUnlockPerAd(Voxel.RarityCategory category)
		{
			return base.settings.GetInt(keyNoOfRarityBlocksToUnlockPerAd(category));
		}

		public int GetRarityBlocksViewColumnsCount(Voxel.RarityCategory category)
		{
			return base.settings.GetInt(keyCategoryBlocksViewColumnsCount(category));
		}

		public RefillType GetRarityBlocksRefillType()
		{
			return (RefillType)base.settings.GetInt(keyBlocksRarityRefillType());
		}

		public bool IsRarityBlocksNoAdsEnabled()
		{
			return base.settings.GetInt(keyBlocksRarityNoAds()) == 1;
		}

		public bool IsBlocksLimitedEnabled()
		{
			return base.settings.GetInt(keyBlocksLimited()) == 1;
		}

		public int GetSingleAdUnlockMode()
		{
			return base.settings.GetInt(keyBlocksUnlockSingleAd());
		}
	}
}
