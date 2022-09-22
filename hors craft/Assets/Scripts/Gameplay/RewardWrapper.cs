// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RewardWrapper
using Common.Gameplay;
using System;
using UnityEngine;

namespace Gameplay
{
	[Serializable]
	public class RewardWrapper
	{
		public Reward reward;

		public int amount;

		public void Claim()
		{
			Reward reward = UnityEngine.Object.Instantiate(this.reward);
			reward.amount = amount;
			reward.ClaimReward();
		}
	}
}
