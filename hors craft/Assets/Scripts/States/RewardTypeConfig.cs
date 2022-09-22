// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RewardTypeConfig
using Common.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	[Serializable]
	public class RewardTypeConfig
	{
		public ChestRewardType rewardType;

		public GameObject panelPrefab;

		public GameObject itemPrefab;

		public string defaultName;

		public string key;

		public List<Reward> rewards;
	}
}
