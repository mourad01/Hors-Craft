// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.CrosspromoDefinition
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace TsgCommon.Crosspromo
{
	internal class CrosspromoDefinition
	{
		public Texture2D texture;

		public string textureUrl = string.Empty;

		public string clickUrl = string.Empty;

		public float duration = float.MaxValue;

		public string campaignId = string.Empty;

		public string bannerId = string.Empty;

		public Sprite containerSprite;

		public Sprite backgroundSprite;

		public bool isGif;

		public bool IsValid()
		{
			return (isGif || texture != null) && !string.IsNullOrEmpty(clickUrl);
		}

		public override string ToString()
		{
			return ((!(texture != null)) ? string.Empty : ("texture size: " + texture.width + " / " + texture.height)) + " url: " + clickUrl + " duration: " + duration + " crosspromoId: " + campaignId + " bannerId: " + bannerId;
		}

		public static bool TryParseFromJSON(string json, out CrosspromoDefinition definition)
		{
			object obj = Json.Deserialize(json);
			try
			{
				Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
				if (dictionary.ContainsKey("error"))
				{
					ParseErrorAndThrow(dictionary["error"]);
				}
				Dictionary<string, object> dictionary2 = dictionary["result"] as Dictionary<string, object>;
				if (!(dictionary2["enabled"] as string!= "false"))
				{
					definition = null;
					return false;
				}
				definition = ParseDefinitionParameters(dictionary2);
			}
			catch (Exception)
			{
				definition = null;
				return false;
			}
			return true;
		}

		protected static CrosspromoDefinition ParseDefinitionParameters(Dictionary<string, object> parameters)
		{
			CrosspromoDefinition crosspromoDefinition = new CrosspromoDefinition();
			crosspromoDefinition.textureUrl = parameters["imageUrl"].ToString();
			crosspromoDefinition.clickUrl = parameters["hrefUrl"].ToString();
			crosspromoDefinition.campaignId = parameters["campaignId"].ToString();
			crosspromoDefinition.bannerId = parameters["bannerId"].ToString();
			string s = parameters["duration"].ToString();
			if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
			{
				crosspromoDefinition.duration = result;
			}
			else
			{
				crosspromoDefinition.duration = float.Parse(s);
			}
			return crosspromoDefinition;
		}

		protected static void ParseErrorAndThrow(object errorObject)
		{
			StringBuilder stringBuilder = new StringBuilder("Crosspromo response error:\n");
			Dictionary<string, object> dictionary = errorObject as Dictionary<string, object>;
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				stringBuilder.AppendLine(item.Key + " --> " + item.Value);
			}
			throw new Exception(stringBuilder.ToString());
		}
	}
}
