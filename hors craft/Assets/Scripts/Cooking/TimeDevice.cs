// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.TimeDevice
using UnityEngine;

namespace Cooking
{
	public class TimeDevice : StorageDevice
	{
		public AudioClip cookingClip;

		public AudioClip burningClip;

		public AudioClip burnedClip;

		public AudioClip timerEndClip;

		private float burnTime;

		private float timeRequired;

		public float GetTimeRequired()
		{
			if (base.workController.cookingGameplay.isTutorialOrMinigame)
			{
				return 3f;
			}
			return timeRequired;
		}

		public float GetBurnTime()
		{
			if (base.workController.cookingGameplay.isTutorialOrMinigame)
			{
				return 9999f;
			}
			return burnTime;
		}

		protected override void SetUpgradeValues(UpgradeEffect effect, float value)
		{
			switch (effect)
			{
			case UpgradeEffect.SPEED:
				timeRequired = value;
				break;
			case UpgradeEffect.CAPACITY:
				slotsUnlocked = (int)value;
				UpdateSlotsAvaible();
				break;
			case UpgradeEffect.BURN_TIME:
				burnTime = value;
				break;
			}
		}
	}
}
