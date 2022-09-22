// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.ServerURL
namespace Common.Utils
{
	public static class ServerURL
	{
		public static string GetProductionServerURL()
		{
			return "https://projectx-mobile.apps.tensquaregames.com/";
		}

		public static string GetDevServerURL(string suffix = "dev")
		{
			return "https://projectx-" + suffix + ".devs.tensquaregames.com/";
		}

		public static string GetGinProductionServerURL()
		{
			return "https://gin-all.apps.tensquaregames.com/jsonrpc";
		}

		public static string GetGinDevServerURL()
		{
			return "http://devs.tensquaregames.com:8997/jsonrpc";
		}
	}
}
