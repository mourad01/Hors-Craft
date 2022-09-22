// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.PlayerSession
using UnityEngine;

namespace Common.Utils
{
	public static class PlayerSession
	{
		private const string SESSION_NO_ID = "playerSessionNo";

		public static void IncrementSessionNo()
		{
			PlayerPrefs.SetInt("playerSessionNo", GetSessionNo() + 1);
		}

		public static int GetSessionNo()
		{
			return PlayerPrefs.GetInt("playerSessionNo", 0);
		}
	}
}
