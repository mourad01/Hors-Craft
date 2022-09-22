// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Model.JSONToSettings
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Common.Model
{
	public static class JSONToSettings
	{
		public static Settings ParseGetContentModel(object jsonSettings, ModelDescription jsonDescription)
		{
			Settings settings = new Settings();
			if (jsonSettings == null)
			{
				UnityEngine.Debug.LogWarning("Tried to parse invalid, null JSON result!");
				return null;
			}
			Dictionary<string, object> dictionary = jsonSettings as Dictionary<string, object>;
			if (dictionary == null)
			{
				UnityEngine.Debug.LogWarning("Tried to parse invalid JSON: main element isn't Dictionary<string,object>!");
				return null;
			}
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				string key = item.Key;
				string value = item.Value.ToString();
				bool nonDefined;
				switch (jsonDescription.GetTypeFor(key, out nonDefined))
				{
				case ModelDescription.ValueType.TEXT:
					if (nonDefined)
					{
						UnityEngine.Debug.LogWarning("Key " + key + " undefined in model! Setting its type to: TEXT");
					}
					settings.SetString(key, value);
					break;
				case ModelDescription.ValueType.INT:
					settings.SetInt(key, ParseInt(value));
					break;
				case ModelDescription.ValueType.FLOAT:
					settings.SetFloat(key, ParseFloat(value));
					break;
				case ModelDescription.ValueType.BOOL:
					settings.SetBool(key, ParseBool(value));
					break;
				}
			}
			return settings;
		}

		public static Settings ParseTwoStepsSettings(object jsonSettings, ModelDescription jsonDescription)
		{
			return ParseTwoStepsTranslations(jsonSettings, jsonDescription);
		}

		public static Settings ParseTwoStepsTranslations(object jsonSettings, ModelDescription jsonDescription)
		{
			Settings settings = new Settings();
			if (jsonSettings == null)
			{
				UnityEngine.Debug.LogWarning("Tried to parse invalid, null JSON result!");
				return null;
			}
			Dictionary<string, object> dictionary = jsonSettings as Dictionary<string, object>;
			if (dictionary == null)
			{
				UnityEngine.Debug.LogWarning("Tried to parse invalid JSON: main element isn't Dictionary<string,object>! It's " + jsonSettings.ToString() + " / type: " + jsonSettings.GetType());
				TroubleshootJsonResultData(jsonSettings, string.Empty);
				return null;
			}
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				string key = item.Key;
				string value = item.Value.ToString();
				bool nonDefined;
				switch (jsonDescription.GetTypeFor(key, out nonDefined))
				{
				case ModelDescription.ValueType.TEXT:
					settings.SetString(key, value);
					break;
				case ModelDescription.ValueType.INT:
					settings.SetInt(key, ParseInt(value));
					break;
				case ModelDescription.ValueType.FLOAT:
					settings.SetFloat(key, ParseFloat(value));
					break;
				case ModelDescription.ValueType.BOOL:
					settings.SetBool(key, ParseBool(value));
					break;
				}
			}
			return settings;
		}

		private static int ParseInt(string value)
		{
			if (!int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
			{
				throw new ArgumentException("Couldn't convert " + value + " to int!");
			}
			return result;
		}

		private static float ParseFloat(string value)
		{
			if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
			{
				throw new ArgumentException("Couldn't convert " + value + " to float!");
			}
			return result;
		}

		private static bool ParseBool(string value)
		{
			if (!bool.TryParse(value, out bool result))
			{
				if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int result2))
				{
					return (result2 != 0) ? true : false;
				}
				throw new ArgumentException("Couldn't convert " + value + " to bool!");
			}
			return result;
		}

		private static void TroubleshootJsonResultData(object data, string logPrefix = "")
		{
			UnityEngine.Debug.Log(logPrefix + " data type: " + data.GetType() + " value: " + data.ToString());
			if (data.GetType() == typeof(List<object>))
			{
				UnityEngine.Debug.Log(logPrefix + " it's a list with length: " + (data as List<object>).Count);
				foreach (object item in data as List<object>)
				{
					TroubleshootJsonResultData(item, logPrefix + " -");
				}
			}
		}
	}
}
