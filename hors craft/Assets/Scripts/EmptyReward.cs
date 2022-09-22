// DecompilerFi decompiler from Assembly-CSharp.dll class: EmptyReward
using Common.Gameplay;
using System.Collections.Generic;
using UnityEngine;

public class EmptyReward : Reward
{
	public override void ClaimReward()
	{
	}

	public override List<Sprite> GetSprites()
	{
		return baseSprite.AsList();
	}
}
