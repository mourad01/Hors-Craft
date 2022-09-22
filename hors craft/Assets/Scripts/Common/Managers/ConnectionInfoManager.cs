// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.ConnectionInfoManager
using Common.Utils;
using System;
using UnityEngine;

namespace Common.Managers
{
	public class ConnectionInfoManager : Manager
	{
		public bool simulateProductionURLInEditor;

		public string gameName = string.Empty;

		public string devServerSuffix = "dev";

		public string homeURL
		{
			get
			{
				string url = (!Debug.isDebugBuild) ? ServerURL.GetProductionServerURL() : ServerURL.GetDevServerURL(devServerSuffix);
				return HttpToHttps(url);
			}
		}

		public string crosspromoUrl
		{
			get
			{
				if (Debug.isDebugBuild)
				{
					return ServerURL.GetGinDevServerURL();
				}
				return HttpToHttps(ServerURL.GetGinProductionServerURL());
			}
		}

		public override void Init()
		{
		}

		private string HttpToHttps(string url)
		{
			if (url.Contains("https"))
			{
				return url;
			}
			return url.Replace("http", "https");
		}

		public void Ping(Action<bool> onPing)
		{
			SimpleRequestMaker.MakeRequest(homeURL, string.Empty, null, delegate
			{
				onPing(obj: true);
			}, delegate
			{
				onPing(obj: false);
			});
		}
	}
}
