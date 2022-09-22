// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Ksero.KseroFiles
using Common.MiniJSON;
using Common.Model;
using Common.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Common.Ksero
{
	public class KseroFiles
	{
		private const string JSON_FILENAME = "KseroData.json";

		private const string RUNTIME_JSON_FILENAME = "RuntimeKseroData.json";

		private static Dictionary<string, Settings> runtimeSettings;

		private static string jsonPath => Application.dataPath + "/KseroData.json";

		private static string runtimeJsonPath => Application.dataPath + "/Resources/RuntimeKseroData.json";

		private static Dictionary<string, Settings.SerializableSettings> LoadRuntimeSettings()
		{
			TextAsset textAsset = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension("RuntimeKseroData.json"));
			if (textAsset == null)
			{
				return new Dictionary<string, Settings.SerializableSettings>();
			}
			object d = Json.Deserialize(textAsset.text);
			Resources.UnloadAsset(textAsset);
			return JSONHelper.Deserialize<Dictionary<string, Settings.SerializableSettings>>(d);
		}

		public static Settings GetSettingsForGame(string game)
		{
			if (runtimeSettings == null)
			{
				runtimeSettings = new Dictionary<string, Settings>();
				Dictionary<string, Settings.SerializableSettings> dictionary = LoadRuntimeSettings();
				foreach (KeyValuePair<string, Settings.SerializableSettings> item in dictionary)
				{
					runtimeSettings.Add(item.Key, Settings.FromSerializableSettings(item.Value));
				}
			}
			if (!runtimeSettings.TryGetValue(game, out Settings value))
			{
				return new Settings();
			}
			return value;
		}
	}
}
