// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Util
using System;

namespace TsgCommon
{
	public static class Util
	{
		public static int now()
		{
			DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (int)(DateTime.UtcNow - d).TotalSeconds;
		}
	}
}
