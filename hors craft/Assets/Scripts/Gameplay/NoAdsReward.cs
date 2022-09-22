// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.NoAdsReward
using Common.Gameplay;
using System.Collections.Generic;
using TsgCommon;
using UnityEngine;

namespace Gameplay
{
	public class NoAdsReward : Reward
	{
		public Sprite noAdsClaimSprite;

		public override void ClaimReward()
		{
			TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.SetNoAds(newState: true);
		}

		public override List<Sprite> GetSprites()
		{
			return noAdsClaimSprite.AsList();
		}
	}
}
