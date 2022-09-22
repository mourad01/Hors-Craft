// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.RetentionNotificationsManager
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class RetentionNotificationsManager : Manager
	{
		private enum State
		{
			WAITING_FOR_AUTHORIZATION,
			WAITING_FOR_TEXTS,
			DONE
		}

		private State state;

		private RetentionNotificationDefinition[] definitions = new RetentionNotificationDefinition[4]
		{
			new RetentionNotificationDefinition("retention.1d", 1u),
			new RetentionNotificationDefinition("retention.3d", 3u),
			new RetentionNotificationDefinition("retention.7d", 7u),
			new RetentionNotificationDefinition("retention.30d", 30u)
		};

		private bool notificationsReadyToBeScheduled;

		private LocalNotificationsManager notificationsManager;

		private Dictionary<string, RetentionNotificationDefinition> callbackDataToDefinition = new Dictionary<string, RetentionNotificationDefinition>();

		public override Type[] GetDependencies()
		{
			return new Type[2]
			{
				typeof(StatsManager),
				typeof(LocalNotificationsManager)
			};
		}

		public override void Init()
		{
			PrepareIdDictionary();
			notificationsManager = Manager.Get<LocalNotificationsManager>();
		}

		public void SetDefinitions(RetentionNotificationDefinition[] definitionsToSet)
		{
			definitions = definitionsToSet;
		}

		public void SetTextForRetentionNotification(int days, string text)
		{
			bool flag = false;
			RetentionNotificationDefinition[] array = definitions;
			foreach (RetentionNotificationDefinition retentionNotificationDefinition in array)
			{
				if (retentionNotificationDefinition.delayInDays == days)
				{
					retentionNotificationDefinition.text = text;
					flag = true;
				}
			}
			if (!flag)
			{
				UnityEngine.Debug.LogWarning("Couldn't set texts for retention notification with days " + days + ". It is not defined in RetentionNotificationsManager!");
			}
		}

		public void InformTextsReady()
		{
			notificationsReadyToBeScheduled = true;
		}

		private void OnGUI()
		{
			if (state == State.WAITING_FOR_AUTHORIZATION)
			{
				if (notificationsManager != null && notificationsManager.authorized)
				{
					CheckIfOpenedWithNotification();
					state = State.WAITING_FOR_TEXTS;
				}
			}
			else if (state == State.WAITING_FOR_TEXTS && notificationsReadyToBeScheduled)
			{
				CancelAllNotifications();
				ScheduleRetentionNotifications();
				state = State.DONE;
			}
		}

		private void PrepareIdDictionary()
		{
			RetentionNotificationDefinition[] array = definitions;
			foreach (RetentionNotificationDefinition retentionNotificationDefinition in array)
			{
				callbackDataToDefinition.Add(retentionNotificationDefinition.callbackData, retentionNotificationDefinition);
			}
		}

		private void CheckIfOpenedWithNotification()
		{
			string openedWithNotificationCallbackData = notificationsManager.GetOpenedWithNotificationCallbackData();
			if (!string.IsNullOrEmpty(openedWithNotificationCallbackData))
			{
				foreach (KeyValuePair<string, RetentionNotificationDefinition> item in callbackDataToDefinition)
				{
					if (openedWithNotificationCallbackData == item.Key)
					{
						RetentionNotificationDefinition value = item.Value;
						Manager.Get<StatsManager>().NotificationOpened(item.Key);
						if (!value.openedOnceAlready)
						{
							Manager.Get<StatsManager>().NotificationOpenedUnique(item.Key);
							value.openedOnceAlready = true;
						}
					}
				}
			}
		}

		private void CancelAllNotifications()
		{
			notificationsManager.CancelAllNotifications();
		}

		private void ScheduleRetentionNotifications()
		{
			List<RetentionNotificationDefinition> list = new List<RetentionNotificationDefinition>(definitions);
			list.Sort((RetentionNotificationDefinition d1, RetentionNotificationDefinition d2) => (int)(d2.delayInDays - d1.delayInDays));
			foreach (RetentionNotificationDefinition item in list)
			{
				if (item.openedOnceAlready)
				{
					break;
				}
				if (item.textsPresent)
				{
					DateTime time = DateTime.Now.AddDays(item.delayInDays);
					notificationsManager.ScheduleLocalNotification(item.callbackData, Application.productName, item.text, time, string.Empty);
					UnityEngine.Debug.Log("Scheduled retention notification for " + item.delayInDays + " with text: " + item.text + " and callbackData: " + item.callbackData);
				}
			}
		}
	}
}
