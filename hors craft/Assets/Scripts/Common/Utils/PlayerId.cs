// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.PlayerId
using UnityEngine;

namespace Common.Utils
{
	public class PlayerId
	{
		public static string GetId()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
	}
}
