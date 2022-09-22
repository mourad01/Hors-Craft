// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ClothesReward
using Common.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class ClothesReward : Reward
	{
		public override void ClaimReward()
		{
			string key = "numberOfWatchedRewardedAdsClothes";
			int @int = PlayerPrefs.GetInt(key, 0);
			@int++;
			PlayerPrefs.SetInt(key, @int);
		}

		public override List<Sprite> GetSprites()
		{
			SkinList skinList = (!(SkinList.customPlayerSkinList != null)) ? SkinList.instance : SkinList.customPlayerSkinList;
			return new List<Sprite>(skinList.GetNextToUnlockSprites());
		}
	}
}
