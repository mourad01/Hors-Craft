// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.TotalPlaytimeManager
using UnityEngine;

namespace Common.Managers
{
	public class TotalPlaytimeManager : Manager
	{
		private const string TOTAL_PLAYTIME_PREF_KEY = "timeSinceStartup";

		public float checkingFrequencyInSecs = 10f;

		private float timeSinceFirstStartup;

		private float lastCheckTime;

		public override void Init()
		{
		}

		private void Start()
		{
			timeSinceFirstStartup = GetTotalPlaytime();
			lastCheckTime = 0f;
		}

		private void Update()
		{
			if (Time.realtimeSinceStartup > lastCheckTime + checkingFrequencyInSecs)
			{
				float num = Time.realtimeSinceStartup - lastCheckTime;
				lastCheckTime = Time.realtimeSinceStartup;
				timeSinceFirstStartup += num;
				PlayerPrefs.SetInt("timeSinceStartup", (int)timeSinceFirstStartup);
			}
		}

		public float GetTotalPlaytime()
		{
			return PlayerPrefs.GetInt("timeSinceStartup", 0);
		}
	}
}
