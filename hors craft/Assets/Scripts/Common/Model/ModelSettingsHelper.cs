// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Model.ModelSettingsHelper
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Common.Model
{
	public static class ModelSettingsHelper
	{
		public static int GetIntFromStringSettings(Settings settings, string key, int def = 0)
		{
			if (settings.HasInt(key))
			{
				return settings.GetInt(key);
			}
			if (settings.HasString(key))
			{
				string @string = settings.GetString(key);
				if (int.TryParse(@string, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
				{
					return result;
				}
				UnityEngine.Debug.LogError("Couldn't parse " + @string + " to int. Key: " + key);
			}
			return def;
		}

		public static float GetFloatFromStringSettings(Settings settings, string key, float def = 0f)
		{
			if (settings.HasFloat(key))
			{
				return settings.GetFloat(key);
			}
			if (settings.HasString(key))
			{
				string @string = settings.GetString(key);
				if (float.TryParse(@string, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
				{
					return result;
				}
				UnityEngine.Debug.LogError("Couldn't parse " + @string + " to float. Key: " + key);
			}
			return def;
		}

		public static bool GetBoolFromStringSettings(Settings settings, string key, bool def = false)
		{
			if (settings.HasBool(key))
			{
				return settings.GetBool(key);
			}
			if (settings.HasString(key))
			{
				string @string = settings.GetString(key);
				if (bool.TryParse(@string, out bool result))
				{
					return result;
				}
				if (int.TryParse(@string, NumberStyles.Any, CultureInfo.InvariantCulture, out int result2))
				{
					switch (result2)
					{
					case 1:
						return true;
					case 0:
						return false;
					}
				}
				UnityEngine.Debug.LogError("Couldn't parse " + @string + " to bool. Key: " + key);
			}
			return def;
		}

		public static List<string> GetStringListFromSettings(Settings settings, string key)
		{
			if (settings.HasString(key))
			{
				string @string = settings.GetString(key, string.Empty);
				if (string.IsNullOrEmpty(@string))
				{
					return new List<string>();
				}
				string[] source = @string.Split(',', '.', ';', ' ');
				return source.ToList();
			}
			return new List<string>();
		}

		public static bool HasField(Settings settings, string key)
		{
			return settings.HasKey(key);
		}
	}
}
