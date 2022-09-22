// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ResourceReward
using Common.Gameplay;
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class ResourceReward : Reward
	{
		public LootChestManager.Rarity chestCategory;

		private List<Resource> resources;

		private List<int> amounts;

		public int minResourceAmount = 5;

		public int maxResourceAmount = 20;

		public int minDifferentResourcesAmount = 1;

		public int maxDifferentResourcesAmount = 3;

		public override void ClaimReward()
		{
			if (resources == null)
			{
				resources = new List<Resource>();
			}
			else
			{
				resources.Clear();
			}
			if (amounts == null)
			{
				amounts = new List<int>();
			}
			else
			{
				amounts.Clear();
			}
			minResourceAmount = Manager.Get<ModelManager>().lootSettings.GetResourcesAmountMin(chestCategory);
			maxResourceAmount = Manager.Get<ModelManager>().lootSettings.GetResourcesAmountMax(chestCategory);
			minDifferentResourcesAmount = Manager.Get<ModelManager>().lootSettings.GetResourcesDifferentAmountMin(chestCategory);
			maxDifferentResourcesAmount = Manager.Get<ModelManager>().lootSettings.GetResourcesDifferentAmountMax(chestCategory);
			int num = Random.Range(minDifferentResourcesAmount, maxDifferentResourcesAmount + 1);
			for (int i = 0; i < num; i++)
			{
				Resource randomItem = Manager.Get<CraftingManager>().GetResourcesList(getOnlyVisibleOnList: true).GetRandomItem();
				int num2 = Random.Range(minResourceAmount, maxResourceAmount + 1);
				Singleton<PlayerData>.get.playerItems.AddResource(randomItem.id, num2);
				resources.Add(randomItem);
				amounts.Add(num2);
			}
			if (resources.IsNullOrEmpty())
			{
				amount = 0;
			}
		}

		public override List<Sprite> GetSprites()
		{
			List<Sprite> list = new List<Sprite>();
			for (int i = 0; i < resources.Count; i++)
			{
				for (int j = 0; j < amounts[i]; j++)
				{
					list.Add(Manager.Get<CraftingManager>().GetResourceDefinition(resources[i].id).GetImage());
				}
			}
			return list;
		}
	}
}
