// DecompilerFi decompiler from Assembly-CSharp.dll class: DailyEventManager
using Common.Managers;
using Common.Utils;
using System;
using System.Globalization;
using UnityEngine;

public class DailyEventManager : Manager
{
	public bool alwaysZeroDay;

	public bool overrideFirstDay;

	public int firstWorldIndexOverride;

	protected string OVERRIDE_PREFS_KEY = "Dressup.firstWorldOverrideTime";

	private bool shownToday;

	public DayOfWeek currentDay
	{
		get
		{
			if (alwaysZeroDay)
			{
				return DayOfWeek.Sunday;
			}
			if (overrideFirstDay)
			{
				if (PlayerSession.GetSessionNo() <= 1 && !PlayerPrefs.HasKey(OVERRIDE_PREFS_KEY))
				{
					PlayerPrefs.SetString(OVERRIDE_PREFS_KEY, DateTime.Now.ToString("g", CultureInfo.CurrentCulture));
					PlayerPrefs.Save();
				}
				else
				{
					string @string = PlayerPrefs.GetString(OVERRIDE_PREFS_KEY, DateTime.MinValue.ToString("g", CultureInfo.CurrentCulture));
					DateTime dateTime = DateTime.ParseExact(@string, "g", CultureInfo.CurrentCulture);
					TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);
					int timeToMidnight = GetTimeToMidnight(dateTime);
					if ((double)timeToMidnight < timeSpan.TotalSeconds)
					{
						return DateTime.Now.DayOfWeek;
					}
				}
				return (DayOfWeek)firstWorldIndexOverride;
			}
			return DateTime.Now.DayOfWeek;
		}
	}

	private int lastDayPlayed
	{
		get
		{
			return PlayerPrefs.GetInt("Dressup.lastDayPlayed", -1);
		}
		set
		{
			PlayerPrefs.SetInt("Dressup.lastDayPlayed", value);
		}
	}

	[ContextMenu("What world index is loaded?")]
	private void DebugPrintCurrentWorldIndex()
	{
		int num = -1;
		if (Manager.Contains<SavedWorldManager>())
		{
			num = Manager.Get<SavedWorldManager>().currentWorldDataIndex;
		}
		UnityEngine.Debug.Log($"Current day : {DateTime.Now.DayOfWeek}({(int)DateTime.Now.DayOfWeek}), current loaded world: {num}");
	}

	private int GetTimeToMidnight(DateTime customDate)
	{
		int num = 24 - customDate.Hour - 1;
		int num2 = 60 - customDate.Minute - 1;
		int num3 = 60 - customDate.Second - 1;
		return num3 + num2 * 60 + num * 3600;
	}

	public bool ShouldShowEventOver()
	{
		bool flag = false;
		if (lastDayPlayed == -1)
		{
			flag = false;
			shownToday = true;
		}
		if (!shownToday)
		{
			shownToday = true;
			UnityEngine.Debug.LogError((currentDay != (DayOfWeek)lastDayPlayed) + " : should show?");
			flag = (currentDay != (DayOfWeek)lastDayPlayed);
		}
		else
		{
			flag = false;
		}
		lastDayPlayed = (int)currentDay;
		return flag;
	}

	private bool ShouldShowWardrobe()
	{
		return currentDay != (DayOfWeek)lastDayPlayed;
	}

	public override void Init()
	{
		Manager.Get<SavedWorldManager>().currentWorldDataIndex = (int)currentDay;
	}
}
