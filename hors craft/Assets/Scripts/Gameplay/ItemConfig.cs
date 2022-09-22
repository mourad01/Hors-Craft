// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ItemConfig
using Common.Gameplay;
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class ItemConfig : Manager, IGameCallbacksListener
	{
		[Serializable]
		public class Item
		{
			public string translationsKey;

			public ItemWrapper item;

			public List<PriceWrapper> prices;

			public List<RequirementWrapper> requirements;

			public List<RewardWrapper> rewards;

			public string key => (((object)item != null) ? item.GetName() : null) ?? string.Empty;
		}

		public List<Item> items = new List<Item>();

		public bool hideVoxelsFromEngine;

		public override void Init()
		{
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
		}

		public bool IsUnlocked(string id)
		{
			Item item = GetItem(id);
			return IsUnlocked(item);
		}

		public bool CanAfford(string id)
		{
			Item item = GetItem(id);
			return CanAfford(item, id);
		}

		public bool Buy(string id)
		{
			Item item = GetItem(id);
			return Buy(item, id);
		}

		public bool IsUnlocked(Item item)
		{
			return item.requirements.All((RequirementWrapper r) => r.CheckIfMet(item.key));
		}

		public bool CanAfford(Item item, string name)
		{
			return item.prices.All((PriceWrapper p) => p.currency.CanIBuyThis(name, p.defaultAmount));
		}

		public bool Buy(Item item, string name)
		{
			if (!IsUnlocked(item) || !CanAfford(item, name))
			{
				return false;
			}
			item.prices.ForEach(delegate(PriceWrapper p)
			{
				p.currency.TryToBuySomething(p.currency.GetPrice(name, p.defaultAmount), null);
			});
			return true;
		}

		public void OnItemClaimed(string name)
		{
			GetItem(name)?.rewards.ForEach(delegate(RewardWrapper r)
			{
				Reward reward = UnityEngine.Object.Instantiate(r.reward);
				reward.amount = r.amount;
				reward.ClaimReward();
				UnityEngine.Object.Destroy(reward);
			});
		}

		public Item GetItem(string name)
		{
			return items.FirstOrDefault((Item i) => i.key == name);
		}

		public void OnGameSavedFrequent()
		{
		}

		public void OnGameSavedInfrequent()
		{
		}

		public void OnGameplayStarted()
		{
			SetUnlimitedBlocks();
			SetHiddenBlocks();
		}

		public void OnGameplayRestarted()
		{
			SetUnlimitedBlocks();
			SetHiddenBlocks();
		}

		private void SetUnlimitedBlocks()
		{
			if (Manager.Get<ModelManager>().blocksUnlocking.IsBlocksLimitedEnabled())
			{
				List<ushort> voxels = (from i in items
					where i.item is ItemWrapperVoxel
					select (i.item as ItemWrapperVoxel).prefab.GetComponent<Voxel>().GetUniqueID()).ToList();
				Array.ForEach(Engine.Blocks, delegate(Voxel b)
				{
					if (voxels.Contains(b.GetUniqueID()))
					{
						b.rarityCategory = Voxel.RarityCategory.UNLIMITED;
					}
				});
			}
		}

		private void SetHiddenBlocks()
		{
			if (hideVoxelsFromEngine)
			{
				List<ushort> voxels = (from i in items
					where i.item is ItemWrapperVoxel
					select (i.item as ItemWrapperVoxel).prefab.GetComponent<Voxel>().GetUniqueID()).ToList();
				Array.ForEach(Engine.Blocks, delegate(Voxel b)
				{
					if (voxels.Contains(b.GetUniqueID()))
					{
						Engine.EngineInstance.hiddenVoxel.Add(b);
					}
				});
			}
		}
	}
}
