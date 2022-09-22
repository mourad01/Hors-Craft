// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.SetBlockCategoriesBlocksAsUnlimited
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	[RequireComponent(typeof(BlocksCategories))]
	public class SetBlockCategoriesBlocksAsUnlimited : MonoBehaviour, IGameCallbacksListener
	{
		private void Awake()
		{
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
		}

		private void SetUnlimitedBlocks(List<GameObject> voxelsGO)
		{
			if (Manager.Get<ModelManager>().blocksUnlocking.IsBlocksLimitedEnabled())
			{
				List<ushort> voxels = (from v in voxelsGO
					select v.GetComponent<Voxel>().GetUniqueID()).ToList();
				Array.ForEach(Engine.Blocks, delegate(Voxel b)
				{
					if (voxels.Contains(b.GetUniqueID()))
					{
						b.rarityCategory = Voxel.RarityCategory.UNLIMITED;
					}
				});
			}
		}

		public void OnGameplayRestarted()
		{
		}

		public void OnGameplayStarted()
		{
			BlocksCategories component = GetComponent<BlocksCategories>();
			foreach (BlocksCategories.Category category in component.categories)
			{
				SetUnlimitedBlocks(category.voxels);
			}
		}

		public void OnGameSavedFrequent()
		{
		}

		public void OnGameSavedInfrequent()
		{
		}
	}
}
