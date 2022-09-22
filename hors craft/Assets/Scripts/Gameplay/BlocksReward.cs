// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.BlocksReward
using Common.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class BlocksReward : Reward
	{
		public override void ClaimReward()
		{
			string key = "numberOfWatchedRewardedAdsBlocks";
			int @int = PlayerPrefs.GetInt(key, 0);
			@int++;
			PlayerPrefs.SetInt(key, @int);
		}

		public override List<Sprite> GetSprites()
		{
			return new List<Sprite>(Singleton<BlocksController>.get.VoxelsSpritesToNextUnlock);
		}
	}
}
