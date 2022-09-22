// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.QuestsV2.RewardRewardRuner
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.QuestsV2
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Quests/Rewards/RewardRewardRuner")]
	public class RewardRewardRuner : MainQuestRewardRuner
	{
		public List<RewardWrapper> rewards = new List<RewardWrapper>();

		public override void Claim(Action claimed)
		{
			rewards.ForEach(delegate(RewardWrapper reawrd)
			{
				reawrd.Claim();
			});
			claimed();
		}
	}
}
