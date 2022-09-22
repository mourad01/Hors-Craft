// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.TimeScaleHelper
using UnityEngine;

namespace Common.Utils
{
	public static class TimeScaleHelper
	{
		private static float currentScale = 1f;

		public static float value
		{
			get
			{
				return currentScale;
			}
			set
			{
				currentScale = value;
				Time.timeScale = currentScale;
				Time.fixedDeltaTime = currentScale * 0.02f;
			}
		}
	}
}
