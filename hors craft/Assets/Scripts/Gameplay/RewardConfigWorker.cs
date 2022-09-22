// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RewardConfigWorker
using Common.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class RewardConfigWorker : MonoBehaviour
	{
		[Serializable]
		public class ConfigItem
		{
			public Reward reward;

			public Sprite sprite;

			public void Config(Reward reward)
			{
				reward.baseSprite = sprite;
			}
		}

		public List<ConfigItem> configs = new List<ConfigItem>();

		private void Awake()
		{
			RewardsManager componentInParent = GetComponentInParent<RewardsManager>();
			foreach (Reward reward in componentInParent.rewards.availableRewards)
			{
				configs.FirstOrDefault((ConfigItem c) => c.reward == reward)?.Config(reward);
			}
		}
	}
}
