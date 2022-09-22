// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GiftsReward
using Common.Gameplay;
using Common.Managers;
using Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class GiftsReward : Reward
	{
		private Craftable randomGift;

		public override void ClaimReward()
		{
			if (randomGift == null)
			{
				List<Craftable> craftableList = Manager.Get<CraftingManager>().GetCraftableListInstance().craftableList;
				List<Craftable> array = (from c in craftableList
					where c.customCraftableObject != null && c.customCraftableObject.GetComponent<GiftCraftable>() != null
					select c).ToList();
				randomGift = array.Random();
			}
			int id = randomGift.id;
			Singleton<PlayerData>.get.playerItems.AddCraftable(id, 1);
			randomGift = null;
		}

		public override List<Sprite> GetSprites()
		{
			if (randomGift == null)
			{
				List<Craftable> craftableList = Manager.Get<CraftingManager>().GetCraftableListInstance().craftableList;
				List<Craftable> array = (from c in craftableList
					where c.customCraftableObject != null && c.customCraftableObject.GetComponent<GiftCraftable>() != null
					select c).ToList();
				randomGift = array.Random();
			}
			return randomGift.sprite.AsList();
		}
	}
}
