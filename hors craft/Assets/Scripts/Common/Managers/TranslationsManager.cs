// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.TranslationsManager
using Common.Behaviours;
using Common.Connection;
using Common.Model;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Common.Managers
{
	[RequireComponent(typeof(TranslationsSwapperController))]
	public class TranslationsManager : Manager
	{
		public SystemLanguage simulatedLanguage = SystemLanguage.English;

		public SystemLanguage currentLanguage;

		private SafeModel safeModel;

		private int translationsBeingDownloaded;

		private const string DOWNLOADED_TRANSLATIONS_KEYS_PREF_KEY = "downloaded.translations.keys";

		private List<SystemLanguage> unsupportedLanguages = new List<SystemLanguage>
		{
			SystemLanguage.Arabic,
			SystemLanguage.Hebrew
		};

		private const float NEW_LANGUAGE_DISPLAY_DURATION = 2f;

		private float hideNewLanguageDisplayTime;

		public bool translationsDownloaded => translationsBeingDownloaded <= 0;

		public override void Init()
		{
			ModelDescription description = ConstructTranslationsModelDescription();
			int num = (int)(currentLanguage = (SystemLanguage)PlayerPrefs.GetInt("game.language", (int)Application.systemLanguage));
			StartDownloadModel(description);
		}

		private ModelDescription ConstructTranslationsModelDescription()
		{
			ModelDescription modelDescription = new ModelDescription("translations");
			List<string> list = LoadPreviouslyDownloadedTranslationsKeys();
			foreach (string item in list)
			{
				modelDescription.AddDescription(item, string.Empty);
			}
			return modelDescription;
		}

		public void StopCoroutinesAndDownload()
		{
			StopAllCoroutines();
			StartDownloadModel(ConstructTranslationsModelDescription());
		}

		private void StartDownloadModel(ModelDescription description)
		{
			translationsBeingDownloaded++;
			TwoStepsModelDownloader.languageToDownload = currentLanguage;
			string gameName = Manager.Get<ConnectionInfoManager>().gameName;
			string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
			safeModel = new SafeModel(gameName, homeURL, description, SafeModel.Mode.TWO_STEPS_TRANSLATIONS);
			LoadDefaultTranslations(safeModel);
			UnityEngine.Debug.Log("Previously saved translations: " + safeModel.settings.ToString());
			safeModel.StartDownloadFromServer(this, OnModelDownloaded);
		}

		private void LoadDefaultTranslations(SafeModel safeModel)
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Game/translations");
			if (!(textAsset == null))
			{
				Settings.SerializableSettings serializableSettings = JSONHelper.Deserialize<Settings.SerializableSettings>(textAsset.text);
				if (serializableSettings == null)
				{
					UnityEngine.Debug.LogError("Couldnt deserialize translations.txt");
					return;
				}
				Settings settings = Settings.FromSerializableSettings(serializableSettings);
				safeModel.SaveSettings(settings);
			}
		}

		private void OnModelDownloaded()
		{
			translationsBeingDownloaded--;
			SaveDownloadedTranslationsKeys();
		}

		public string GetText(string key, string fallback = "")
		{
			if (safeModel != null && safeModel.settings.HasString(key))
			{
				return safeModel.settings.GetString(key);
			}
			return fallback;
		}

		private void SaveDownloadedTranslationsKeys()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> @string in safeModel.settings.GetStrings())
			{
				stringBuilder.Append(@string.Key);
				stringBuilder.Append(' ');
			}
			PlayerPrefs.SetString("downloaded.translations.keys", stringBuilder.ToString());
		}

		private List<string> LoadPreviouslyDownloadedTranslationsKeys()
		{
			string @string = PlayerPrefs.GetString("downloaded.translations.keys");
			return new List<string>(@string.Split(new char[1]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries));
		}

		public void DownloadPreviousLanguage()
		{
			int length = Enum.GetValues(typeof(SystemLanguage)).Length;
			int num = (int)(currentLanguage - 1) % length;
			if (num < 0)
			{
				num += length;
			}
			currentLanguage = (SystemLanguage)num;
			if (unsupportedLanguages.Contains(currentLanguage))
			{
				DownloadPreviousLanguage();
				return;
			}
			hideNewLanguageDisplayTime = Time.realtimeSinceStartup + 2f;
			StartDownloadModel(ConstructTranslationsModelDescription());
		}

		public void DownloadNextLanguage()
		{
			int num = (int)(currentLanguage = (SystemLanguage)((int)(currentLanguage + 1) % Enum.GetValues(typeof(SystemLanguage)).Length));
			if (unsupportedLanguages.Contains(currentLanguage))
			{
				DownloadNextLanguage();
				return;
			}
			hideNewLanguageDisplayTime = Time.realtimeSinceStartup + 2f;
			StartDownloadModel(ConstructTranslationsModelDescription());
		}

		public void UpdateCurrentLanguage(SystemLanguage language)
		{
			currentLanguage = language;
			StartDownloadModel(ConstructTranslationsModelDescription());
		}

		public string GetConnectionErrorText()
		{
			return GetText("error.connection", "Please turn on the Internet connection or move to an area with good reception");
		}

		private void OnGUI()
		{
			if (Time.realtimeSinceStartup < hideNewLanguageDisplayTime)
			{
				Vector2 size = new Vector2((float)Screen.width / 1.5f, Screen.height / 5);
				Vector2 position = new Vector2((float)(Screen.width / 2) - size.x / 2f, 0f);
				Rect position2 = new Rect(position, size);
				GUIStyle gUIStyle = new GUIStyle(GUI.skin.box);
				gUIStyle.fontSize += 20;
				gUIStyle.alignment = TextAnchor.MiddleCenter;
				GUI.Box(position2, currentLanguage.ToString().ToUpperInvariant(), gUIStyle);
			}
		}
	}
}
