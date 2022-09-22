// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.Connection.CrosspromoConnection
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TsgCommon.Crosspromo.Connection
{
	internal abstract class CrosspromoConnection
	{
		protected string gamename;

		protected string homeURL;

		protected string playerId;

		public bool running
		{
			get;
			protected set;
		}

		public CrosspromoConnection(string gamename, string homeURL, string playerId)
		{
			this.gamename = gamename;
			this.homeURL = homeURL;
			this.playerId = playerId;
			running = false;
		}

		protected WWW CreateCrosspromoRequest(string method)
		{
			string arg = "android";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{ ");
			stringBuilder.AppendFormat("\"method\": \"{0}\", ", method);
			stringBuilder.Append("\"id\": \"foo\", ");
			stringBuilder.Append("\"params\": { ");
			stringBuilder.AppendFormat("\"app\" : \"{0}\"", gamename);
			stringBuilder.AppendFormat(", \"platform\" : \"{0}\"", arg);
			stringBuilder.AppendFormat(", \"playerId\" : \"{0}\"", playerId);
			stringBuilder.Append(", \"allowGif\" :  true ");
			stringBuilder.Append(", \"withDuration\" :  true ");
			stringBuilder.Append(" } }");
			string text = stringBuilder.ToString();
			UnityEngine.Debug.Log("Crosspromo request: " + text);
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("content-type", "application/json");
			return new WWW(homeURL, bytes, dictionary);
		}
	}
}
