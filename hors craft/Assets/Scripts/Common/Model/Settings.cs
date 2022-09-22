// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Model.Settings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Model
{
	public class Settings
	{
		[Serializable]
		public class SerializableSettings
		{
			public Dictionary<string, string> strings;

			public Dictionary<string, int> ints;

			public Dictionary<string, float> floats;

			public Dictionary<string, bool> bools;

			public SerializableSettings()
			{
			}

			public SerializableSettings(Dictionary<string, string> strings, Dictionary<string, int> ints, Dictionary<string, float> floats, Dictionary<string, bool> bools)
			{
				this.strings = strings;
				this.ints = ints;
				this.floats = floats;
				this.bools = bools;
			}
		}

		private Dictionary<string, string> strings = new Dictionary<string, string>();

		private Dictionary<string, int> ints = new Dictionary<string, int>();

		private Dictionary<string, float> floats = new Dictionary<string, float>();

		private Dictionary<string, bool> bools = new Dictionary<string, bool>();

		public bool HasString(string key)
		{
			return strings.ContainsKey(key);
		}

		public bool HasInt(string key)
		{
			return ints.ContainsKey(key);
		}

		public bool HasFloat(string key)
		{
			return floats.ContainsKey(key);
		}

		public bool HasBool(string key)
		{
			return bools.ContainsKey(key);
		}

		public void SetString(string key, string value)
		{
			strings[key] = value;
		}

		public void SetInt(string key, int value)
		{
			ints[key] = value;
		}

		public void SetFloat(string key, float value)
		{
			floats[key] = value;
		}

		public void SetBool(string key, bool value)
		{
			bools[key] = value;
		}

		public string GetString(string key)
		{
			return strings[key];
		}

		public Dictionary<string, string> GetStringsMatching(string match)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> @string in strings)
			{
				if (@string.Key.Contains(match))
				{
					dictionary.Add(@string.Key, @string.Value);
				}
			}
			return dictionary;
		}

		public int GetInt(string key)
		{
			return ints[key];
		}

		public float GetFloat(string key)
		{
			return floats[key];
		}

		public bool GetBool(string key)
		{
			return bools[key];
		}

		public string GetString(string key, string defaultValue)
		{
			if (!HasString(key))
			{
				return defaultValue;
			}
			return strings[key];
		}

		public int GetInt(string key, int defaultValue)
		{
			if (!HasInt(key))
			{
				return defaultValue;
			}
			return ints[key];
		}

		public float GetFloat(string key, float defaultValue)
		{
			if (!HasFloat(key))
			{
				return defaultValue;
			}
			return floats[key];
		}

		public bool GetBool(string key, bool defaultValue)
		{
			if (!HasBool(key))
			{
				return defaultValue;
			}
			return bools[key];
		}

		public void DeleteKey(string key)
		{
			strings.Remove(key);
			ints.Remove(key);
			floats.Remove(key);
			bools.Remove(key);
		}

		public bool HasKey(string key)
		{
			return strings.ContainsKey(key) || ints.ContainsKey(key) || floats.ContainsKey(key) || bools.ContainsKey(key);
		}

		public void ClearAll()
		{
			strings.Clear();
			ints.Clear();
			floats.Clear();
			bools.Clear();
		}

		public IEnumerable<KeyValuePair<string, string>> GetStrings()
		{
			return strings;
		}

		public IEnumerable<KeyValuePair<string, int>> GetInts()
		{
			return ints;
		}

		public IEnumerable<KeyValuePair<string, float>> GetFloats()
		{
			return floats;
		}

		public IEnumerable<KeyValuePair<string, bool>> GetBools()
		{
			return bools;
		}

		public void OverrideWithSettingsFrom(Settings addition)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			Dictionary<string, float> dictionary3 = new Dictionary<string, float>();
			Dictionary<string, bool> dictionary4 = new Dictionary<string, bool>();
			foreach (string key in strings.Keys)
			{
				if (addition.HasString(key))
				{
					dictionary[key] = addition.GetString(key);
				}
				else
				{
					dictionary[key] = GetString(key);
				}
			}
			foreach (string key2 in ints.Keys)
			{
				if (addition.HasInt(key2))
				{
					dictionary2[key2] = addition.GetInt(key2);
				}
				else
				{
					dictionary2[key2] = GetInt(key2);
				}
			}
			foreach (string key3 in floats.Keys)
			{
				if (addition.HasFloat(key3))
				{
					dictionary3[key3] = addition.GetFloat(key3);
				}
				else
				{
					dictionary3[key3] = GetFloat(key3);
				}
			}
			foreach (string key4 in bools.Keys)
			{
				if (addition.HasBool(key4))
				{
					dictionary4[key4] = addition.GetBool(key4);
				}
				else
				{
					dictionary4[key4] = GetBool(key4);
				}
			}
			strings.Clear();
			strings = dictionary;
			ints.Clear();
			ints = dictionary2;
			floats.Clear();
			floats = dictionary3;
			bools.Clear();
			bools = dictionary4;
		}

		public void AppendSettingsFrom(Settings addition)
		{
			foreach (KeyValuePair<string, string> @string in addition.strings)
			{
				SetString(@string.Key, @string.Value);
			}
			foreach (KeyValuePair<string, int> @int in addition.ints)
			{
				SetInt(@int.Key, @int.Value);
			}
			foreach (KeyValuePair<string, float> @float in addition.floats)
			{
				SetFloat(@float.Key, @float.Value);
			}
			foreach (KeyValuePair<string, bool> @bool in addition.bools)
			{
				SetBool(@bool.Key, @bool.Value);
			}
		}

		public Settings ShallowCopy()
		{
			Settings settings = new Settings();
			foreach (string key in strings.Keys)
			{
				settings.SetString(key, strings[key]);
			}
			foreach (string key2 in ints.Keys)
			{
				settings.SetInt(key2, ints[key2]);
			}
			foreach (string key3 in floats.Keys)
			{
				settings.SetFloat(key3, floats[key3]);
			}
			foreach (string key4 in bools.Keys)
			{
				settings.SetBool(key4, bools[key4]);
			}
			return settings;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> @string in strings)
			{
				stringBuilder.AppendLine(@string.Key.ToString() + " : " + @string.Value.ToString() + " (string)");
			}
			foreach (KeyValuePair<string, int> @int in ints)
			{
				stringBuilder.AppendLine(@int.Key.ToString() + " : " + @int.Value.ToString() + " (int)");
			}
			foreach (KeyValuePair<string, float> @float in floats)
			{
				stringBuilder.AppendLine(@float.Key.ToString() + " : " + @float.Value.ToString() + " (float)");
			}
			foreach (KeyValuePair<string, bool> @bool in bools)
			{
				stringBuilder.AppendLine(@bool.Key.ToString() + " : " + @bool.Value.ToString() + " (bool)");
			}
			return stringBuilder.ToString();
		}

		public string ToString(string[] forbidden, string[] allowed)
		{
			string text = ToString();
			if (forbidden == null && allowed == null)
			{
				return text;
			}
			string[] source = text.Split('\n');
			source = (from x in source
				where !IsInArray(x, forbidden) && IsInArray(x, allowed, deafult: true)
				select x).ToArray();
			if (source == null)
			{
				return string.Empty;
			}
			if (source.Length < 1)
			{
				return string.Empty;
			}
			return source.Aggregate((string current, string next) => current + "\n" + next);
		}

		private bool IsInArray(string value, string[] array, bool deafult = false)
		{
			if (array == null || array.Length < 1)
			{
				return deafult;
			}
			string text = value.ToUpper();
			for (int i = 0; i < array.Length; i++)
			{
				if (text.Contains(array[i].ToUpper()))
				{
					return true;
				}
			}
			return false;
		}

		public SerializableSettings ToSerializableSettings()
		{
			return new SerializableSettings(strings, ints, floats, bools);
		}

		public static Settings FromSerializableSettings(SerializableSettings other)
		{
			Settings settings = new Settings();
			settings.strings = other.strings;
			settings.ints = other.ints;
			settings.floats = other.floats;
			settings.bools = other.bools;
			return settings;
		}
	}
}
