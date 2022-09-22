// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.AllCraftableItemsReward
using Common.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class AllCraftableItemsReward : Reward
	{
		public override void ClaimReward()
		{
			PlayerPrefs.SetInt("overrideschematicsadsnumber", 1);
			PlayerPrefs.Save();
		}

		public override List<Sprite> GetSprites()
		{
			return baseSprite.AsList();
		}
	}
}
