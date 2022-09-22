// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Connection.Util
using System;

namespace Common.Connection
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
