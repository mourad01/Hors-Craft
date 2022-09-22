// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CraftableReward
using Common.Gameplay;
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class CraftableReward : Reward
	{
		private Craftable craftable;

		public override void ClaimReward()
		{
			if (craftable == null)
			{
				craftable = Manager.Get<CraftingManager>().GetCraftableListInstance().GetRandom();
			}
			Singleton<PlayerData>.get.playerItems.OnCraftableUnlockByAd(craftable.id);
			Singleton<PlayerData>.get.playerItems.AddCraftable(craftable.id, 1);
			craftable = null;
		}

		public override List<Sprite> GetSprites()
		{
			if (craftable == null)
			{
				craftable = Manager.Get<CraftingManager>().GetCraftableListInstance().GetRandom();
			}
			Sprite tobject = (!(craftable.sprite == null)) ? craftable.sprite : VoxelSprite.GetVoxelSprite((ushort)craftable.blockId);
			return tobject.AsList();
		}
	}
}
