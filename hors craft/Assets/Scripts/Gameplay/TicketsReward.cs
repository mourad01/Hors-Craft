// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.TicketsReward
using Common.Gameplay;
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class TicketsReward : Reward
	{
		public override void ClaimReward()
		{
			if (Manager.Contains<TicketsManager>())
			{
				Manager.Get<TicketsManager>().ownedTickets += amount;
			}
		}

		public override List<Sprite> GetSprites()
		{
			return baseSprite.AsList();
		}
	}
}
