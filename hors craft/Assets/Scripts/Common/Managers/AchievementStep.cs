// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AchievementStep
using Common.Gameplay;
using System;
using UnityEngine;

namespace Common.Managers
{
	public class AchievementStep
	{
		public double countToUnlock;

		public Reward reward;

		public int rewardAmount;

		public Action claimAction;

		[HideInInspector]
		public Achievement achievement;

		[HideInInspector]
		public bool isUnlocked;

		[HideInInspector]
		public bool isClaimed;
	}
}
