// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Model.PlayerPrefsToSettings
using System.Collections.Generic;
using UnityEngine;

namespace Common.Model
{
	public static class PlayerPrefsToSettings
	{
		private const string HAS_SAVED_MODEL_VER_ID = "has.saved.model.for.";

		private const string STRINGS_PREFIX = "string.prefix.";

		private const string INTS_PREFIX = "int.prefix.";

		private const string FLOATS_PREFIX = "float.prefix.";

		private const string BOOLS_PREFIX = "bool.prefix.";

		public static bool HasSavedModelDataFor(ModelDescription modelDescription)
		{
			return PlayerPrefs.GetInt("has.saved.model.for." + modelDescription.GetIdentifier().ToString(), 0) != 0;
		}

		private static void SavedModelDataFor(ModelDescription modelDescription)
		{
			PlayerPrefs.SetInt("has.saved.model.for." + modelDescription.GetIdentifier().ToString(), 1);
		}

		public static Settings Load(ModelDescription modelDescription)
		{
			Settings settings = new Settings();
			foreach (KeyValuePair<string, ModelDescription.ValueType> keysAndType in modelDescription.GetKeysAndTypes())
			{
				string key = keysAndType.Key;
				switch (keysAndType.Value)
				{
				case ModelDescription.ValueType.TEXT:
					settings.SetString(key, GetText(key));
					break;
				case ModelDescription.ValueType.INT:
					settings.SetInt(key, GetInt(key));
					break;
				case ModelDescription.ValueType.FLOAT:
					settings.SetFloat(key, GetFloat(key));
					break;
				case ModelDescription.ValueType.BOOL:
					settings.SetBool(key, GetBool(key));
					break;
				}
			}
			return settings;
		}

		public static void Save(ModelDescription modelDescription, Settings settings)
		{
			foreach (KeyValuePair<string, string> @string in settings.GetStrings())
			{
				SetText(@string.Key, @string.Value);
			}
			foreach (KeyValuePair<string, int> @int in settings.GetInts())
			{
				SetInt(@int.Key, @int.Value);
			}
			foreach (KeyValuePair<string, float> @float in settings.GetFloats())
			{
				SetFloat(@float.Key, @float.Value);
			}
			foreach (KeyValuePair<string, bool> @bool in settings.GetBools())
			{
				SetBool(@bool.Key, @bool.Value);
			}
			SavedModelDataFor(modelDescription);
		}

		private static string GetText(string key)
		{
			return PlayerPrefs.GetString("string.prefix." + key);
		}

		private static int GetInt(string key)
		{
			return PlayerPrefs.GetInt("int.prefix." + key);
		}

		private static float GetFloat(string key)
		{
			return PlayerPrefs.GetFloat("float.prefix." + key);
		}

		private static bool GetBool(string key)
		{
			return PlayerPrefs.GetInt("bool.prefix." + key) != 0;
		}

		private static void SetText(string key, string value)
		{
			PlayerPrefs.SetString("string.prefix." + key, value);
		}

		private static void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt("int.prefix." + key, value);
		}

		private static void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat("float.prefix." + key, value);
		}

		private static void SetBool(string key, bool value)
		{
			PlayerPrefs.SetInt("bool.prefix." + key, value ? 1 : 0);
		}
	}
}
