// DecompilerFi decompiler from Assembly-CSharp.dll class: DailyChestManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using UnityEngine;

public class DailyChestManager : Manager
{
	public GameObject chestToSpawn;

	private GameObject runningInstance;

	private const string OPENED_CHEST_COUNT_KEY = "daily.chest.opened.count";

	private const string MULTIPLIER_KEY = "daily.chest.multiplier";

	public override void Init()
	{
	}

	public int GetOpenedChestCount()
	{
		return PlayerPrefs.GetInt("daily.chest.opened.count", 0);
	}

	public float GetDailyChestMultiplier()
	{
		return PlayerPrefs.GetFloat("daily.chest.multiplier", 1f);
	}

	public void SetDailyChestMultiplier(float value)
	{
		PlayerPrefs.SetFloat("daily.chest.multiplier", value);
	}

	public void IncOpenChestCount()
	{
		SetOpenChestCount(GetOpenedChestCount() + 1);
	}

	public void SetOpenChestCount(int value)
	{
		PlayerPrefs.SetInt("daily.chest.opened.count", value);
	}

	public bool TryOpenChest()
	{
		if (!IsChestReadyToOpen())
		{
			return false;
		}
		Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(GetCurrentReward());
		IncOpenChestCount();
		AutoRefreshingStock.DecrementStockCount("daily.chest");
		ScheduleNotifications();
		Manager.Get<StatsManager>().DailyChestOpened();
		return true;
	}

	public bool IsChestReadyToOpen()
	{
		return AutoRefreshingStock.GetStockCount("daily.chest", Manager.Get<ModelManager>().dailyChestSettings.GetTimeToNextChest(), 1, 0) >= 1;
	}

	public int GetCurrentReward()
	{
		int prize = Manager.Get<ModelManager>().dailyChestSettings.GetPrize(GetOpenedChestCount());
		return (int)(GetDailyChestMultiplier() * (float)prize);
	}

	public int GetLastReward()
	{
		int prize = Manager.Get<ModelManager>().dailyChestSettings.GetPrize(GetOpenedChestCount() - 1);
		return (int)(GetDailyChestMultiplier() * (float)prize);
	}

	public void SpawnIfNeeded(Vector2 position, bool hideAfterOpen)
	{
		SpawnAt(position, hideAfterOpen);
	}

	public string GetTimeForNextChest()
	{
		double num = AutoRefreshingStock.GetNextItemCooldown("daily.chest");
		if (num < 0.0)
		{
			num = 0.0;
		}
		if (num == 0.0 || GetOpenedChestCount() == 0)
		{
			return string.Empty;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(num);
		return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	}

	private void SpawnAt(Vector2 position, bool hideAfterOpen)
	{
		if (Manager.Get<ModelManager>().dailyChestSettings.IsEnabled())
		{
			if (runningInstance == null)
			{
				runningInstance = UnityEngine.Object.Instantiate(chestToSpawn);
			}
			runningInstance.GetComponent<DailyChestUI>().Init(position, hideAfterOpen);
		}
	}

	public void OnModelDownloaded()
	{
		AutoRefreshingStock.InitStockItem("daily.chest", Manager.Get<ModelManager>().dailyChestSettings.GetTimeToNextChest(), 1, 1);
		ScheduleNotifications();
	}

	public void DestroyCurrentInstanceIfShould()
	{
		runningInstance.GetComponent<DailyChestUI>().DestroyAfterAnimation();
	}

	public void ScheduleNotifications()
	{
		double num = Misc.GetTimeStampDouble() + (double)AutoRefreshingStock.GetNextItemCooldown("daily.chest");
		DateTime time = Misc.UnixTimeStampToDateTimeDouble(num);
		int secondNotificationDelay = Manager.Get<ModelManager>().dailyChestSettings.GetSecondNotificationDelay();
		DateTime time2 = Misc.UnixTimeStampToDateTimeDouble(num + (double)secondNotificationDelay);
		Manager.Get<LocalNotificationsManager>().CancelAllNotifications();
		string text = Manager.Get<TranslationsManager>().GetText("daily.chest.notfication.1", string.Empty);
		string text2 = Manager.Get<TranslationsManager>().GetText("daily.chest.notfication.2", string.Empty);
		string text3 = Manager.Get<TranslationsManager>().GetText("daily.chest.notfication.3", string.Empty);
		Manager.Get<LocalNotificationsManager>().ScheduleLocalNotification("daily.chest.notfication.1", Application.productName, text, time, string.Empty);
		Manager.Get<LocalNotificationsManager>().ScheduleLocalNotification("daily.chest.notfication.2", Application.productName, text2, time2, string.Empty);
		for (int i = 0; i < 30; i++)
		{
			DateTime time3 = new DateTime(time2.Year, time2.Month, time2.Day, 18, 0, 0).Add(new TimeSpan((i + 1) * 24, 0, 0));
			Manager.Get<LocalNotificationsManager>().ScheduleLocalNotification("daily.chest.notfication." + (3 + i), Application.productName, text3, time3, string.Empty);
		}
	}
}
