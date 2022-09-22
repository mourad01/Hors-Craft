// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ChatBotLoader
using AIMLbot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Gameplay
{
	public class ChatBotLoader : Singleton<ChatBotLoader>
	{
		private const string CONFIG_DIR = "AIMLConfig";

		private const string AIML_DIR = "AIMLConfig/AIMLs";

		private TextAsset[] aimlFiles;

		private Dictionary<string, string> loadedSettingsContents;

		private List<Action> loadingActions;

		private ChatbotMobileWeb bot;

		private int botLoadingStepIndex;

		private TextAsset loadedTextAsset;

		private int botLoadingStepsCount => loadingActions.Count;

		public float loadingPercentage => (float)botLoadingStepIndex / (float)botLoadingStepsCount;

		public ChatBotLoader()
		{
			loadedSettingsContents = new Dictionary<string, string>();
			loadingActions = new List<Action>
			{
				delegate
				{
					bot.LoadGlobalSettings(GetSettingContent("Settings"));
				},
				delegate
				{
					bot.LoadGenderSubstitutions(GetSettingContent("GenderSubstitutions"));
				},
				delegate
				{
					bot.LoadPerson2Substitutions(GetSettingContent("Person2Substitutions"));
				},
				delegate
				{
					bot.LoadPersonSubstitutions(GetSettingContent("PersonSubstitutions"));
				},
				delegate
				{
					bot.LoadSubstitutions(GetSettingContent("Substitutions"));
				},
				delegate
				{
					bot.LoadDefaultPredicates(GetSettingContent("DefaultPredicates"));
				},
				delegate
				{
					bot.LoadSplitters(GetSettingContent("Splitters"));
				},
				delegate
				{
					bot.LoadUserDefaultPredicates();
				},
				delegate
				{
					bot.AimlBot.Graphmaster = new TemplatesDictionary();
					loadedTextAsset = Resources.Load<TextAsset>("AIMLGraph");
					MemoryStream memoryStream = new MemoryStream(loadedTextAsset.bytes);
					StreamReader streamReader = new StreamReader(memoryStream);
					bot.AimlBot.Graphmaster.LoadFromStream(streamReader);
					streamReader.Close();
					memoryStream.Close();
				},
				delegate
				{
					Resources.UnloadAsset(loadedTextAsset);
					loadedTextAsset = null;
				}
			};
			loadingActions.Add(delegate
			{
				bot.LoadBrain();
			});
		}

		public ChatbotMobileWeb CreateAndGetBot(bool male, int seed = 0)
		{
			if (bot == null)
			{
				bot = new ChatbotMobileWeb();
				botLoadingStepIndex = 0;
			}
			return bot;
		}

		public void ClearBot()
		{
		}

		private string GetSettingContent(string name)
		{
			if (!loadedSettingsContents.TryGetValue(name, out string value))
			{
				value = Resources.Load<TextAsset>("AIMLConfig/" + name).text;
				loadedSettingsContents.Add(name, value);
			}
			return value;
		}

		public void LoadNextStep()
		{
			if (!(loadingPercentage >= 1f))
			{
				loadingActions[botLoadingStepIndex]();
				botLoadingStepIndex++;
				if (!(loadingPercentage >= 1f))
				{
				}
			}
		}
	}
}
