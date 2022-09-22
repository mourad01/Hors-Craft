// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.BlocksUnlimitedReward
using Common.Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class BlocksUnlimitedReward : Reward
	{
		public Voxel.RarityCategory voxelCategory;

		public override void ClaimReward()
		{
			PlayerPrefs.SetInt("Max.blocks.Unlimited." + voxelCategory.ToString(), 1);
		}

		public override List<Sprite> GetSprites()
		{
			return baseSprite.AsList();
		}
	}
}
