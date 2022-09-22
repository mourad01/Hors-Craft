// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.LocalNotificationsManager
using Common.Managers.LocalNotifications;
using Common.Managers.Validators;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	[RequireComponent(typeof(ValidateNotificationIconsPrepared))]
	public class LocalNotificationsManager : Manager
	{
		private class NotificationToBeScheduled
		{
			public enum Type
			{
				CANCEL_ALL,
				SCHEDULE
			}

			public Type type;

			public string callbackData;

			public string name;

			public string body;

			public string imagePath = string.Empty;

			public DateTime time;
		}

		private ILocalNotificationsProvider notificationsProvider;

		private bool initialized;

		private bool _authorized;

		private List<NotificationToBeScheduled> scheduledActions = new List<NotificationToBeScheduled>();

		private const string AUTHORIZED_BEFORE_KEY = "authorized.notifications.before";

		public bool authorized
		{
			get
			{
				return _authorized;
			}
			private set
			{
				_authorized = value;
			}
		}

		private bool authorizedOnceBefore
		{
			get
			{
				return PlayerPrefs.GetInt("authorized.notifications.before", 0) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("authorized.notifications.before", value ? 1 : 0);
			}
		}

		public override Type[] GetDependencies()
		{
			return new Type[1]
			{
				typeof(StatsManager)
			};
		}

		public override void Init()
		{
			notificationsProvider = new AndroidNotificationsProvider();
			if (notificationsProvider != null)
			{
				notificationsProvider.Initialize();
				initialized = true;
			}
		}

		public string GetOpenedWithNotificationCallbackData()
		{
			if (initialized)
			{
				return notificationsProvider.GetOpenedWithNotificationCallbackData();
			}
			return string.Empty;
		}

		public void CancelAllNotifications()
		{
			ScheduleWhenAuthorized(new NotificationToBeScheduled
			{
				type = NotificationToBeScheduled.Type.CANCEL_ALL
			});
		}

		public void ScheduleLocalNotification(string callbackData, string name, string body, DateTime time, string imagePath = "")
		{
			ScheduleWhenAuthorized(new NotificationToBeScheduled
			{
				type = NotificationToBeScheduled.Type.SCHEDULE,
				callbackData = callbackData,
				name = name,
				body = body,
				time = time,
				imagePath = imagePath
			});
		}

		private void ScheduleWhenAuthorized(NotificationToBeScheduled a)
		{
			scheduledActions.Add(a);
		}

		public int ScheduleNotificationQuick(string callbackData, string name, string body, DateTime time, string imagePath = "")
		{
			if (!authorized || notificationsProvider == null)
			{
				return -1;
			}
			return notificationsProvider.ScheduleNotification(callbackData, name, body, time, imagePath);
		}

		public void CancelNotificationQuick(int id)
		{
			if (authorized)
			{
				notificationsProvider.CancelNotification(id);
			}
		}

		public void CancelNotificationsQuick()
		{
			notificationsProvider.CancelAllNotifications();
		}

		private void OnGUI()
		{
			if (!authorized)
			{
				CheckAuthorization();
			}
			else if (scheduledActions.Count > 0)
			{
				FireNotificationsToBeScheduled();
			}
		}

		private void CheckAuthorization()
		{
			if (notificationsProvider != null && notificationsProvider.IsAuthorized())
			{
				authorized = true;
				if (!authorizedOnceBefore)
				{
					authorizedOnceBefore = true;
					Manager.Get<StatsManager>().NotificationsAccepted();
				}
			}
		}

		private void FireNotificationsToBeScheduled()
		{
			foreach (NotificationToBeScheduled scheduledAction in scheduledActions)
			{
				if (notificationsProvider != null)
				{
					if (scheduledAction.type == NotificationToBeScheduled.Type.CANCEL_ALL)
					{
						notificationsProvider.CancelAllNotifications();
					}
					else
					{
						notificationsProvider.ScheduleNotification(scheduledAction.callbackData, scheduledAction.name, scheduledAction.body, scheduledAction.time, string.Empty);
					}
				}
			}
			scheduledActions.Clear();
		}
	}
}
