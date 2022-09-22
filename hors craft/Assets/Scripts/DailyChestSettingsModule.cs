// DecompilerFi decompiler from Assembly-CSharp.dll class: DailyChestSettingsModule
using Common.Managers;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DailyChestSettingsModule : ModelModule
{
	[CompilerGenerated]
	private static Func<string, int> _003C_003Ef__mg_0024cache0;

	private string keyNextTime()
	{
		return "daily.chest.time";
	}

	private string keyPrizes()
	{
		return "daily.chest.prizes";
	}

	private string keyEnabled()
	{
		return "daily.chest.enabled";
	}

	private string keyNotificationDelay()
	{
		return "daily.chest.notificationdelay";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyEnabled(), defaultValue: true);
		descriptions.AddDescription(keyNextTime(), 30);
		descriptions.AddDescription(keyPrizes(), "100;200;300;400;500");
		descriptions.AddDescription(keyNotificationDelay(), 10800);
	}

	public override void OnModelDownloaded()
	{
		if (Manager.Contains<DailyChestManager>())
		{
			Manager.Get<DailyChestManager>().OnModelDownloaded();
		}
	}

	public List<int> GetPrizes()
	{
		return base.settings.GetString(keyPrizes()).SplitToList(';', int.Parse);
	}

	public bool IsEnabled()
	{
		return base.settings.GetBool(keyEnabled());
	}

	public int GetTimeToNextChest()
	{
		return base.settings.GetInt(keyNextTime());
	}

	public int GetSecondNotificationDelay()
	{
		return base.settings.GetInt(keyNotificationDelay(), 10800);
	}

	public int GetPrize(int counter)
	{
		List<int> prizes = GetPrizes();
		return prizes[Mathf.Clamp(counter, 0, prizes.Count - 1)];
	}
}
