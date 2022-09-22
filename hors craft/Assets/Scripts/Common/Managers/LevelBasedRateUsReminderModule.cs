// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LevelBasedRateUsReminderModule
using Common.Model;
using UnityEngine;

namespace Common.Managers
{
	public class LevelBasedRateUsReminderModule : ModelModule
	{
		private const string shownRateReminderPrefID = "shownRateReminder";

		private const string ratedAlreadyPrefID = "ratedAlreadyPrefID";

		private string keyRateUsReminderFirstLevel()
		{
			return "rateusreminder.firstlevel";
		}

		private string keyRateUsReminderNextLevelDivider()
		{
			return "rateusreminder.nextleveldivider";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyRateUsReminderFirstLevel(), 4);
			descriptions.AddDescription(keyRateUsReminderNextLevelDivider(), 4);
		}

		public override void OnModelDownloaded()
		{
		}

		public bool HasToShowRateReminderForLevel(int levelNo)
		{
			if (PlayerPrefs.GetInt("ratedAlreadyPrefID", 0) == 1)
			{
				return false;
			}
			int @int = base.settings.GetInt(keyRateUsReminderFirstLevel());
			int int2 = base.settings.GetInt(keyRateUsReminderNextLevelDivider());
			if ((levelNo == @int || (levelNo - @int) % int2 == 0) && PlayerPrefs.GetInt("shownRateReminder" + levelNo.ToString(), 0) != 1)
			{
				PlayerPrefs.SetInt("shownRateReminder" + levelNo.ToString(), 1);
				return true;
			}
			return false;
		}

		public void RatedAlready()
		{
			PlayerPrefs.SetInt("ratedAlreadyPrefID", 1);
		}
	}
}
