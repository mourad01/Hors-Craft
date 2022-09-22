// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LocalNotifications.AndroidLocalNotificationsHelper
using System;
using UnityEngine;

namespace Common.Managers.LocalNotifications
{
	public class AndroidLocalNotificationsHelper
	{
		public enum NotificationExecuteMode
		{
			Inexact,
			Exact,
			ExactAndAllowWhileIdle
		}

		private static string fullClassName = "com.playcoolzombiesportgames.android.utils.UnityNotificationManager";

		private static string mainActivityClassName = "com.playcoolzombiesportgames.android.utils.XUnityActivity";

		public static void SendNotification(int id, string callbackData, TimeSpan delay, string title, string message, string bigPicture = "")
		{
			long delay2 = (int)delay.TotalSeconds;
			Color32 bgColor = Color.white;
			SendNotification(id, callbackData, delay2, title, message, bgColor, sound: true, vibrate: false, lights: true, string.Empty, NotificationExecuteMode.Inexact, bigPicture);
		}

		public static void SendNotification(int id, string callbackData, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = false, bool lights = true, string bigIcon = "", NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact, string bigPicture = "")
		{
			new AndroidJavaClass(fullClassName)?.CallStatic("SetNotification", id, callbackData, delay * 1000, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, mainActivityClassName, bigPicture);
		}

		public static void SendRepeatingNotification(int id, string callbackData, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = false, bool lights = true, string bigIcon = "", string bigPicture = "")
		{
			new AndroidJavaClass(fullClassName)?.CallStatic("SetRepeatingNotification", id, callbackData, delay * 1000, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, mainActivityClassName, bigPicture);
		}

		public static void CancelNotification(int id)
		{
			new AndroidJavaClass(fullClassName)?.CallStatic("CancelNotification", id);
		}

		public static string GetNotificationCallbackData()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.playcoolzombiesportgames.android.utils.XUnityActivity");
			if (androidJavaClass == null)
			{
				return string.Empty;
			}
			return androidJavaClass.CallStatic<string>("GetIntentExtraXData", Array.Empty<object>());
		}
	}
}
