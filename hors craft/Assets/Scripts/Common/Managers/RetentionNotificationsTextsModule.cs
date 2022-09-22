// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.RetentionNotificationsTextsModule
using Common.Model;

namespace Common.Managers
{
	public class RetentionNotificationsTextsModule : ModelModule
	{
		private string keyLocalNotificationRetention(int days)
		{
			return "localnotification.retention." + days + "day";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyLocalNotificationRetention(1), string.Empty);
			descriptions.AddDescription(keyLocalNotificationRetention(3), string.Empty);
			descriptions.AddDescription(keyLocalNotificationRetention(7), string.Empty);
			descriptions.AddDescription(keyLocalNotificationRetention(30), string.Empty);
		}

		public override void OnModelDownloaded()
		{
			RetentionNotificationsManager retentionNotificationsManager = Manager.Get<RetentionNotificationsManager>();
			retentionNotificationsManager.SetTextForRetentionNotification(1, base.settings.GetString(keyLocalNotificationRetention(1)));
			retentionNotificationsManager.SetTextForRetentionNotification(3, base.settings.GetString(keyLocalNotificationRetention(3)));
			retentionNotificationsManager.SetTextForRetentionNotification(7, base.settings.GetString(keyLocalNotificationRetention(7)));
			retentionNotificationsManager.SetTextForRetentionNotification(30, base.settings.GetString(keyLocalNotificationRetention(30)));
			retentionNotificationsManager.InformTextsReady();
		}
	}
}
