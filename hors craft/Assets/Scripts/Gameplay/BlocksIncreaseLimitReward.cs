// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.BlocksIncreaseLimitReward
using Common.Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class BlocksIncreaseLimitReward : Reward
	{
		public Voxel.RarityCategory voxelCategory;

		public override void ClaimReward()
		{
			PlayerPrefs.SetInt("Max.blocks.increase" + voxelCategory.ToString(), PlayerPrefs.GetInt("Max.blocks.increase" + voxelCategory.ToString(), 0) + amount);
		}

		public override List<Sprite> GetSprites()
		{
			return baseSprite.AsList();
		}
	}
}
