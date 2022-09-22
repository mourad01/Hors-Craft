// DecompilerFi decompiler from Assembly-CSharp.dll class: Com.Google.Android.Gms.Games.Stats.Stats_LoadPlayerStatsResultObject
using Com.Google.Android.Gms.Common.Api;
using Google.Developers;
using System;

namespace Com.Google.Android.Gms.Games.Stats
{
	public class Stats_LoadPlayerStatsResultObject : JavaObjWrapper, Stats_LoadPlayerStatsResult, Result
	{
		private const string CLASS_NAME = "com/google/android/gms/games/stats/Stats$LoadPlayerStatsResult";

		public Stats_LoadPlayerStatsResultObject(IntPtr ptr)
			: base(ptr)
		{
		}

		public PlayerStats getPlayerStats()
		{
			IntPtr ptr = InvokeCall<IntPtr>("getPlayerStats", "()Lcom/google/android/gms/games/stats/PlayerStats;", Array.Empty<object>());
			return new PlayerStatsObject(ptr);
		}

		public Status getStatus()
		{
			IntPtr ptr = InvokeCall<IntPtr>("getStatus", "()Lcom/google/android/gms/common/api/Status;", Array.Empty<object>());
			return new Status(ptr);
		}
	}
}
