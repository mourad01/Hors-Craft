// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ClaimRewardsStateStartParameter
using Common.Gameplay;
using Common.Managers.States;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	public class ClaimRewardsStateStartParameter : StartParameter
	{
		public List<Reward> rewards;

		public GameObject spawnedChest;
	}
}
