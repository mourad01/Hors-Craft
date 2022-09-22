// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ChatBotData
using AIMLbot.Utils;
using Common.Managers;
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class ChatBotData
	{
		private static SettingsVariation loadedVariationsData;

		private static SettingsVariation variations
		{
			get
			{
				if (loadedVariationsData != null)
				{
					return loadedVariationsData;
				}
				TextAsset textAsset = Resources.Load<TextAsset>("AIMLConfig/SettingsVariations_" + Manager.Get<ConnectionInfoManager>().gameName);
				if (textAsset == null)
				{
					textAsset = Resources.Load<TextAsset>("AIMLConfig/SettingsVariations");
				}
				string text = textAsset.text;
				loadedVariationsData = JSONHelper.Deserialize<SettingsVariation>(text);
				return loadedVariationsData;
			}
		}

		public static void Create(bool male, int seed, SettingsDictionary dict)
		{
			seed = Mathf.Abs(seed);
			ApplySettingsFrom(variations.both, seed, dict);
			if (male)
			{
				ApplySettingsFrom(variations.male, seed, dict);
			}
			else
			{
				ApplySettingsFrom(variations.female, seed, dict);
			}
		}

		private static void ApplySettingsFrom(Dictionary<string, List<string>> settings, int seed, SettingsDictionary dict)
		{
			foreach (KeyValuePair<string, List<string>> setting in settings)
			{
				dict.updateSetting(setting.Key, setting.Value[seed / 100 % setting.Value.Count]);
				seed++;
			}
		}
	}
}
