// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ResourcesReward
using Common.Gameplay;
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class ResourcesReward : Reward
	{
		public override void ClaimReward()
		{
			List<Resource> resourcesList = Manager.Get<CraftingManager>().GetResourcesList();
			int index = Random.Range(0, resourcesList.Count);
			Singleton<PlayerData>.get.playerItems.AddToResources(resourcesList[index].id, amount);
		}

		public override List<Sprite> GetSprites()
		{
			return baseSprite.AsList();
		}
	}
}
