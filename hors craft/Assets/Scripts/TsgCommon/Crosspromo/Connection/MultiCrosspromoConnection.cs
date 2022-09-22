// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.Connection.MultiCrosspromoConnection
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TsgCommon.Crosspromo.Connection
{
	internal abstract class MultiCrosspromoConnection : CrosspromoConnection
	{
		public MultiCrosspromoConnection(string gamename, string homeURL, string playerId)
			: base(gamename, homeURL, playerId)
		{
		}

		protected WWW CreateCrosspromoViewRequest(string method, string[] tags, int amount)
		{
			string platform = GetPlatform();
			ViewPostData data = new ViewPostData(method, "foo", gamename, platform, playerId, amount, preloading: true, tags, allowGif: true, withDuration: true);
			return JsonToRequest(data);
		}

		protected WWW CreateShowClickRequest(string method, long bannerId)
		{
			string platform = GetPlatform();
			ClickShowkPostData data = new ClickShowkPostData(method, "foo", gamename, platform, playerId, bannerId);
			return JsonToRequest(data);
		}

		private string GetPlatform()
		{
			return "android";
		}

		private WWW JsonToRequest(PostData data)
		{
			string text = JsonUtility.ToJson(data);
			string text2 = text.Replace("parameters", "params");
			byte[] bytes = Encoding.UTF8.GetBytes(text2);
			UnityEngine.Debug.Log("Crosspromo request: " + text2);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("content-type", "application/json");
			return new WWW(homeURL, bytes, dictionary);
		}
	}
}
