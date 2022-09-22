// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LocalNotifications.iOSNotificationsProvider
using System;

namespace Common.Managers.LocalNotifications
{
	public class iOSNotificationsProvider : ILocalNotificationsProvider
	{
		public void Initialize()
		{
		}

		public bool IsAuthorized()
		{
			return false;
		}

		public void CancelAllNotifications()
		{
		}

		public void CancelNotification(int id)
		{
		}

		public int ScheduleNotification(string callbackData, string name, string body, DateTime time, string imagePath)
		{
			return -1;
		}

		public string GetOpenedWithNotificationCallbackData()
		{
			return string.Empty;
		}
	}
}
