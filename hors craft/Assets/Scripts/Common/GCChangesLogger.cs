// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.GCChangesLogger
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public static class GCChangesLogger
	{
		private static Stack<float> loggingTimesStarts = new Stack<float>();

		private static Stack<long> loggingGCsStarts = new Stack<long>();

		public static void Begin()
		{
			loggingGCsStarts.Push(GC.GetTotalMemory(forceFullCollection: true));
			loggingTimesStarts.Push(Time.realtimeSinceStartup);
		}

		public static void End(string whatWasMeasured)
		{
			float num = loggingTimesStarts.Pop();
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num2 = realtimeSinceStartup - num;
			TimeSpan timeSpan = new TimeSpan((long)(1E+07f * num2));
			if (timeSpan.Milliseconds != 0)
			{
				long num3 = loggingGCsStarts.Pop();
				long totalMemory = GC.GetTotalMemory(forceFullCollection: true);
				UnityEngine.Debug.LogWarning("GCLogger: " + whatWasMeasured + " took " + timeSpan.Minutes + "m " + timeSpan.Seconds + "s " + timeSpan.Milliseconds + "ms");
				UnityEngine.Debug.LogWarning("GCLogger: " + whatWasMeasured + " memory diff: " + (totalMemory - num3) / 1024 + "B");
			}
		}
	}
}
