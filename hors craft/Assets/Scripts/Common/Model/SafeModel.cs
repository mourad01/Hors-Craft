// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Model.SafeModel
using Common.Connection;
using System;
using UnityEngine;

namespace Common.Model
{
	public class SafeModel
	{
		public enum Mode
		{
			GET_CONTENT,
			TWO_STEPS_SETTINGS,
			TWO_STEPS_TRANSLATIONS,
			ONE_STEP_SETTINGS
		}

		private string[] forbiddenToLog;

		private string app;

		private string homeURL;

		private ModelDescription modelDescription;

		private Settings currentSettings = new Settings();

		private Mode mode;

		private Action callbackOnDownloaded;

		public Settings settings => currentSettings;

		public SafeModel(string app, string homeURL, ModelDescription modelDescription = null, Mode mode = Mode.GET_CONTENT, string[] forbiddenToLog = null)
		{
			this.app = app;
			this.homeURL = homeURL;
			this.modelDescription = modelDescription;
			this.mode = mode;
			if (PlayerPrefsToSettings.HasSavedModelDataFor(modelDescription))
			{
				currentSettings = PlayerPrefsToSettings.Load(modelDescription);
			}
			else
			{
				currentSettings = modelDescription.GetDefaultSettings().ShallowCopy();
			}
			this.forbiddenToLog = forbiddenToLog;
		}

		public void StartDownloadFromServer(MonoBehaviour coroutinesProvider, Action callbackOnDownloaded = null)
		{
			AbstractModelDownloader abstractModelDownloader = CreateModelDownloader();
			coroutinesProvider.StartCoroutine(abstractModelDownloader.DownloadModelCoroutine(OnModelDownloaded));
			this.callbackOnDownloaded = callbackOnDownloaded;
		}

		private AbstractModelDownloader CreateModelDownloader()
		{
			switch (mode)
			{
			default:
				return new ModelDownloader(app, homeURL);
			case Mode.TWO_STEPS_SETTINGS:
				return new TwoStepsModelDownloader(app, homeURL, TwoStepsModelDownloader.Mode.SETTINGS);
			case Mode.TWO_STEPS_TRANSLATIONS:
				return new TwoStepsModelDownloader(app, homeURL, TwoStepsModelDownloader.Mode.TRANSLATIONS);
			case Mode.ONE_STEP_SETTINGS:
				return new OneStepModelDownloader(app, homeURL, OneStepModelDownloader.Mode.SETTINGS);
			}
		}

		public void SaveSettings(Settings settings)
		{
			currentSettings.AppendSettingsFrom(settings);
			PlayerPrefsToSettings.Save(modelDescription, currentSettings);
		}

		private void OnModelDownloaded(object jsonResult)
		{
			ParseSettings(jsonResult, modelDescription);
			UnityEngine.Debug.LogFormat("Downloaded model, time {0}, with ID: {1}  values: \n {2}", Time.realtimeSinceStartup, modelDescription.GetIdentifier(), settings.ToString(forbiddenToLog, null));
			PlayerPrefsToSettings.Save(modelDescription, currentSettings);
			if (callbackOnDownloaded != null)
			{
				callbackOnDownloaded();
				callbackOnDownloaded = null;
			}
		}

		private void ParseSettings(object jsonResult, ModelDescription modelDescription)
		{
			switch (mode)
			{
			default:
			{
				Settings settings = JSONToSettings.ParseGetContentModel(jsonResult, modelDescription);
				if (settings != null)
				{
					currentSettings.OverrideWithSettingsFrom(settings);
				}
				break;
			}
			case Mode.TWO_STEPS_SETTINGS:
			case Mode.ONE_STEP_SETTINGS:
			{
				Settings settings = JSONToSettings.ParseTwoStepsSettings(jsonResult, modelDescription);
				if (settings != null)
				{
					currentSettings.AppendSettingsFrom(settings);
				}
				break;
			}
			case Mode.TWO_STEPS_TRANSLATIONS:
			{
				Settings settings = JSONToSettings.ParseTwoStepsTranslations(jsonResult, modelDescription);
				if (settings != null)
				{
					currentSettings.AppendSettingsFrom(settings);
				}
				break;
			}
			}
		}
	}
}
