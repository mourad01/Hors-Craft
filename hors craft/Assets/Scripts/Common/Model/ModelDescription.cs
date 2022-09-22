// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Model.ModelDescription
using System.Collections.Generic;

namespace Common.Model
{
	public class ModelDescription
	{
		public enum ValueType
		{
			TEXT,
			INT,
			FLOAT,
			BOOL
		}

		private string identifier;

		private Dictionary<string, ValueType> keysToTypes = new Dictionary<string, ValueType>();

		private Settings defaultSettings = new Settings();

		public ModelDescription(int version)
		{
			identifier = version.ToString();
		}

		public ModelDescription(string uniqueIdentifier)
		{
			identifier = uniqueIdentifier;
		}

		public string GetIdentifier()
		{
			return identifier;
		}

		public IEnumerable<KeyValuePair<string, ValueType>> GetKeysAndTypes()
		{
			return keysToTypes;
		}

		public Settings GetDefaultSettings()
		{
			return defaultSettings;
		}

		public void AddDescription(string key, string defaultValue)
		{
			keysToTypes[key] = ValueType.TEXT;
			defaultSettings.SetString(key, defaultValue);
		}

		public void AddDescription(string key, int defaultValue)
		{
			keysToTypes[key] = ValueType.INT;
			defaultSettings.SetInt(key, defaultValue);
		}

		public void AddDescription(string key, float defaultValue)
		{
			keysToTypes[key] = ValueType.FLOAT;
			defaultSettings.SetFloat(key, defaultValue);
		}

		public void AddDescription(string key, bool defaultValue)
		{
			keysToTypes[key] = ValueType.BOOL;
			defaultSettings.SetBool(key, defaultValue);
		}

		public ValueType GetTypeFor(string key, out bool nonDefined)
		{
			if (!keysToTypes.ContainsKey(key))
			{
				nonDefined = true;
				return ValueType.TEXT;
			}
			nonDefined = false;
			return keysToTypes[key];
		}
	}
}
