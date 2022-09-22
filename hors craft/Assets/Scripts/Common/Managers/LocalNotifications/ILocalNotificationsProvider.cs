// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LocalNotifications.ILocalNotificationsProvider
using System;

namespace Common.Managers.LocalNotifications
{
	public interface ILocalNotificationsProvider
	{
		void Initialize();

		bool IsAuthorized();

		void CancelAllNotifications();

		void CancelNotification(int id);

		int ScheduleNotification(string notificationId, string name, string body, DateTime time, string imagePath = "");

		string GetOpenedWithNotificationCallbackData();
	}
}
