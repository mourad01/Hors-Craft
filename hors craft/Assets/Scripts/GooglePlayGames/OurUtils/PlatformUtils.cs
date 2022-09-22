// DecompilerFi decompiler from Assembly-CSharp.dll class: GooglePlayGames.OurUtils.PlatformUtils
using System;
using UnityEngine;

namespace GooglePlayGames.OurUtils
{
	public static class PlatformUtils
	{
		public static bool Supported
		{
			get
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getPackageManager", Array.Empty<object>());
				AndroidJavaObject androidJavaObject2 = null;
				try
				{
					androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[1]
					{
						"com.google.android.play.games"
					});
				}
				catch (Exception)
				{
					return false;
				}
				return androidJavaObject2 != null;
			}
		}
	}
}
