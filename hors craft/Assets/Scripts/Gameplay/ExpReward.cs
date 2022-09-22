// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ExpReward
using Common.Gameplay;
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Rewards/ExpReward")]
	public class ExpReward : Reward
	{
		public override void ClaimReward()
		{
			if (Manager.Contains<ProgressManager>())
			{
				Manager.Get<ProgressManager>().IncreaseExperience(amount);
			}
		}

		public override List<Sprite> GetSprites()
		{
			return baseSprite.AsList();
		}
	}
}
