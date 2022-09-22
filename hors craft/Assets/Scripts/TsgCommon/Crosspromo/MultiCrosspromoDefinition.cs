// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.MultiCrosspromoDefinition
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace TsgCommon.Crosspromo
{
	internal class MultiCrosspromoDefinition : CrosspromoDefinition
	{
		public static bool TryParseFromJSON(string json, out MultiCrosspromoDefinition definition, int index, bool batch = false)
		{
			object obj = Json.Deserialize(json);
			try
			{
				Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
				if (dictionary.ContainsKey("error"))
				{
					CrosspromoDefinition.ParseErrorAndThrow(dictionary["error"]);
				}
				Dictionary<string, object> dictionary2 = (!batch) ? (dictionary["result"] as Dictionary<string, object>) : ((dictionary["result"] as List<object>)[index] as Dictionary<string, object>);
				if (!(dictionary2["enabled"] as string!= "false"))
				{
					definition = null;
					return false;
				}
				definition = ParseMultiDefinitionParameters(dictionary2);
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogWarning("Crosspromo parse exception: " + arg);
				definition = null;
				return false;
			}
			return true;
		}

		protected static MultiCrosspromoDefinition ParseMultiDefinitionParameters(Dictionary<string, object> parameters)
		{
			MultiCrosspromoDefinition multiCrosspromoDefinition = new MultiCrosspromoDefinition();
			multiCrosspromoDefinition.textureUrl = parameters["imageUrl"].ToString();
			multiCrosspromoDefinition.clickUrl = parameters["hrefUrl"].ToString();
			multiCrosspromoDefinition.campaignId = parameters["campaignId"].ToString();
			multiCrosspromoDefinition.bannerId = parameters["bannerId"].ToString();
			string s = parameters["duration"].ToString();
			if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
			{
				multiCrosspromoDefinition.duration = result;
			}
			else
			{
				multiCrosspromoDefinition.duration = float.Parse(s);
			}
			return multiCrosspromoDefinition;
		}
	}
}
