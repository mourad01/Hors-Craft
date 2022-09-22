// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LocalNotifications.AndroidNotificationsProvider
using System;
using UnityEngine;

namespace Common.Managers.LocalNotifications
{
	public class AndroidNotificationsProvider : ILocalNotificationsProvider
	{
		private const string NOTIFICATION_PREFIX_KEY = "notification-id-";

		private const int MAX_NOTIFICATIONS = 1000;

		public void Initialize()
		{
		}

		public bool IsAuthorized()
		{
			return true;
		}

		public void CancelAllNotifications()
		{
			for (int i = 0; i < 1000; i++)
			{
				string key = "notification-id-" + i;
				if (PlayerPrefs.HasKey(key))
				{
					PlayerPrefs.DeleteKey(key);
					AndroidLocalNotificationsHelper.CancelNotification(i);
					continue;
				}
				break;
			}
		}

		public void CancelNotification(int id)
		{
			AndroidLocalNotificationsHelper.CancelNotification(id);
		}

		public int ScheduleNotification(string callbackData, string name, string body, DateTime time, string imagePath = "")
		{
			int firstEmptyId = GetFirstEmptyId();
			UnityEngine.Debug.Log("notyfiing with id: " + firstEmptyId);
			PlayerPrefs.SetString("notification-id-" + firstEmptyId, name);
			TimeSpan delay = time.Subtract(DateTime.Now);
			AndroidLocalNotificationsHelper.SendNotification(firstEmptyId, callbackData, delay, name, body, imagePath);
			return firstEmptyId;
		}

		private int GetFirstEmptyId()
		{
			int result = -1;
			for (int i = 0; i < 1000; i++)
			{
				string key = "notification-id-" + i;
				if (!PlayerPrefs.HasKey(key))
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public string GetOpenedWithNotificationCallbackData()
		{
			return AndroidLocalNotificationsHelper.GetNotificationCallbackData();
		}
	}
}
