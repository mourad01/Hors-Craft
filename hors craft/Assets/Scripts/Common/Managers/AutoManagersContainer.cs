// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AutoManagersContainer
using Common.Crosspromo;
using System.Collections.Generic;

namespace Common.Managers
{
	public class AutoManagersContainer : ManagersContainer
	{
		public enum AutoManagerAdConfig
		{
			HEYZAP_TAPJOY_ADMOB
		}

		public string gamename = "put game ID here";

		public AutoManagerAdConfig adConfig;

		protected override List<Manager> AppendDefaultManagers()
		{
			List<Manager> list = new List<Manager>();
			int num = 0;
			ConnectionInfoManager connectionInfoManager = AppendIfNotPresent<ConnectionInfoManager>(list, num++);
			connectionInfoManager.gameName = gamename;
			AppendIfNotPresent<StatsManager>(list, num++);
			AppendIfNotPresent<TranslationsManager>(list, num++);
			AppendIfNotPresent<LocalNotificationsManager>(list, num++);
			AppendIfNotPresent<RetentionNotificationsManager>(list, num++);
			AppendIfNotPresent<FeedbackManager>(list, num++);
			AppendIfNotPresent<CrosspromoManager>(list, num++);
			AppendIfNotPresent<TotalPlaytimeManager>(list, num++);
			AppendIfNotPresent<CanvasManager>(list, num++);
			if (adConfig == AutoManagerAdConfig.HEYZAP_TAPJOY_ADMOB)
			{
				AppendIfNotPresent<TapjoyManager>(list, num++);
			}
			AppendIfNotPresent<StateMachineManager>(list, num++);
			return list;
		}
	}
}
