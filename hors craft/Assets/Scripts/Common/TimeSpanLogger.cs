// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.TimeSpanLogger
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public static class TimeSpanLogger
	{
		private static Stack<float> loggingStarts = new Stack<float>();

		public static void Begin()
		{
			loggingStarts.Push(Time.realtimeSinceStartup);
		}

		public static void End(string whatWasMeasured)
		{
			float num = loggingStarts.Pop();
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num2 = realtimeSinceStartup - num;
			TimeSpan timeSpan = new TimeSpan((long)(1E+07f * num2));
			if (timeSpan.Milliseconds == 0)
			{
				UnityEngine.Debug.Log("TimeSpanLogger: " + 0 + "ms");
			}
			else
			{
				UnityEngine.Debug.Log("TimeSpanLogger: " + whatWasMeasured + " took " + timeSpan.Minutes + "m " + timeSpan.Seconds + "s " + timeSpan.Milliseconds + "ms");
			}
		}
	}
}
