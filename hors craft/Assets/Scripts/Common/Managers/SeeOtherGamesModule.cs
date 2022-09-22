// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SeeOtherGamesModule
using Common.Model;
using System;
using UnityEngine;

namespace Common.Managers
{
	public class SeeOtherGamesModule : ModelModule
	{
		protected string keyAndroidSOGURL()
		{
			return "android.seeothergames.url";
		}

		protected string keyiOSSOGURL()
		{
			return "ios.seeothergames.url";
		}

		protected string keyWebGLSOGURL()
		{
			return "webgl.seeothergames.url";
		}

		protected string keySOGEnabled()
		{
			return "seeothergames.enabled";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyAndroidSOGURL(), string.Empty);
			descriptions.AddDescription(keyiOSSOGURL(), string.Empty);
			descriptions.AddDescription(keyWebGLSOGURL(), string.Empty);
			descriptions.AddDescription(keySOGEnabled(), defaultValue: false);
		}

		public override void OnModelDownloaded()
		{
		}

		public string GetSOGURL()
		{
			string empty = string.Empty;
			empty = GetAndroidSOGURL();
			return ParseMultiURL(empty);
		}

		private string GetAndroidSOGURL()
		{
			return base.settings.GetString(keyAndroidSOGURL());
		}

		private string GetiOSSOGURL()
		{
			return base.settings.GetString(keyiOSSOGURL());
		}

		private string GetWebGLSOGURL()
		{
			return base.settings.GetString(keyWebGLSOGURL());
		}

		public bool IsSOGEnabled()
		{
			return base.settings.GetBool(keySOGEnabled());
		}

		private string ParseMultiURL(string url)
		{
			string[] array = url.Split(new char[3]
			{
				' ',
				'\t',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			return array[UnityEngine.Random.Range(0, array.Length)];
		}
	}
}
