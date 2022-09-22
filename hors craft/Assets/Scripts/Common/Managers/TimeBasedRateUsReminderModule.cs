// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.TimeBasedRateUsReminderModule
using Common.Model;
using UnityEngine;

namespace Common.Managers
{
	public class TimeBasedRateUsReminderModule : ModelModule
	{
		private const string shownRateReminderPrefID = "shownRateReminder";

		private const string ratedAlreadyPrefID = "ratedAlreadyPrefID";

		private string keyRateUsReminderFirstTime()
		{
			return "rateusreminder.firsttime";
		}

		private string keyRateUsReminderTimeDivider()
		{
			return "rateusreminder.nexttimedivider";
		}

		private string keyRateUsReminderEnabled()
		{
			return "rateusreminder.enabled";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyRateUsReminderFirstTime(), 40);
			descriptions.AddDescription(keyRateUsReminderTimeDivider(), 60);
			descriptions.AddDescription(keyRateUsReminderEnabled(), defaultValue: true);
		}

		public override void OnModelDownloaded()
		{
		}

		public bool HasToShowRateReminder(int passedTime)
		{
			bool flag = PlayerPrefs.GetInt("ratedAlreadyPrefID", 0) == 1;
			if (!IsRateUsReminderEnabled() || flag)
			{
				return false;
			}
			int @int = base.settings.GetInt(keyRateUsReminderFirstTime());
			int int2 = base.settings.GetInt(keyRateUsReminderTimeDivider());
			int int3 = PlayerPrefs.GetInt("shownRateReminder", 0);
			bool flag2 = false;
			if (int3 == 0)
			{
				if (passedTime > @int)
				{
					flag2 = true;
				}
			}
			else
			{
				passedTime -= @int;
				passedTime -= (int3 - 1) * Mathf.Max(int2, 1);
				if (passedTime > int2)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				PlayerPrefs.SetInt("shownRateReminder", PlayerPrefs.GetInt("shownRateReminder", 0) + 1);
			}
			return flag2;
		}

		public void RatedAlready()
		{
			PlayerPrefs.SetInt("ratedAlreadyPrefID", 1);
		}

		private bool IsRateUsReminderEnabled()
		{
			return base.settings.GetBool(keyRateUsReminderEnabled());
		}
	}
}
