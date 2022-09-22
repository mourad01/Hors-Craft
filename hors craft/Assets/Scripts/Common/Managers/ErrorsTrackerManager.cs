// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.ErrorsTrackerManager
using UnityEngine;

namespace Common.Managers
{
	public class ErrorsTrackerManager : Manager
	{
		private const float TRACKS_INTERVAL = 5f;

		private int errorsPendingToBeTracked;

		private float lastTrackTime;

		public override void Init()
		{
			errorsPendingToBeTracked = 0;
			Application.logMessageReceivedThreaded += ThreadSafeLogCallback;
		}

		private void ThreadSafeLogCallback(string message, string stacktrace, LogType logType)
		{
			if (logType == LogType.Error || logType == LogType.Exception)
			{
				errorsPendingToBeTracked++;
			}
		}

		private void Update()
		{
			if (Time.realtimeSinceStartup > lastTrackTime + 5f)
			{
				lastTrackTime = Time.realtimeSinceStartup;
				if (errorsPendingToBeTracked > 0)
				{
					Track();
				}
			}
		}

		private void Track()
		{
			Manager.Get<StatsManager>().ErrorsOccurred(errorsPendingToBeTracked);
			errorsPendingToBeTracked = 0;
			UnityEngine.Debug.Log("successfully tracked");
		}
	}
}
