// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Connection.TwoStepsModelDownloader
using Common.MiniJSON;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Common.Connection
{
	public class TwoStepsModelDownloader : AbstractModelDownloader
	{
		public enum Mode
		{
			SETTINGS,
			TRANSLATIONS
		}

		public static SystemLanguage? languageToDownload;

		private Mode mode;

		public TwoStepsModelDownloader(string gamename, string homeURL, Mode mode)
			: base(gamename, homeURL)
		{
			this.mode = mode;
		}

		public override IEnumerator DownloadModelCoroutine(OnModelDownloaded onModelDownloaded)
		{
			string askWWWURL = GetAskServerForModelURL();
			WWWForm form = FormFactory.CreateBasicWWWForm();
			if (mode == Mode.TRANSLATIONS)
			{
				SystemLanguage systemLanguage = Application.systemLanguage;
				if (languageToDownload.HasValue)
				{
					systemLanguage = languageToDownload.Value;
				}
				form.AddField("language", systemLanguage.ToString().ToTitleCase());
			}
			UnityWebRequest askWWW = UnityWebRequest.Post(askWWWURL, form);
			UnityEngine.Debug.Log("Asking server for model for game: " + gamename + " ...");
			yield return askWWW.Send();
			if (!string.IsNullOrEmpty(askWWW.error))
			{
				UnityEngine.Debug.LogWarning("Server sent WWW error: " + askWWW.error);
				yield break;
			}
			if (string.IsNullOrEmpty(askWWW.downloadHandler.text))
			{
				UnityEngine.Debug.LogWarning("Server sent invalid (empty) modelURL!");
				yield break;
			}
			string modelURL = askWWW.downloadHandler.text;
			UnityEngine.Debug.Log("Trying to download model for game: " + gamename + " ... " + modelURL);
			UnityWebRequest modelWWW = UnityWebRequest.Get(modelURL);
			yield return modelWWW.Send();
			if (!string.IsNullOrEmpty(modelWWW.error))
			{
				UnityEngine.Debug.LogWarning("Server sent error: " + modelWWW.error);
				yield break;
			}
			string text = modelWWW.downloadHandler.text;
			if (string.IsNullOrEmpty(text))
			{
				text = Convert.ToBase64String(modelWWW.downloadHandler.data);
			}
			if (string.IsNullOrEmpty(text))
			{
				UnityEngine.Debug.LogWarning("Server sent invalid (empty) model! Bytes: " + modelWWW.downloadHandler.data.Length + " sets URL: " + modelURL);
				yield break;
			}
			object result = Json.Deserialize(modelWWW.downloadHandler.text);
			if (result == null)
			{
				UnityEngine.Debug.LogWarning("Couldn't deserialize model! It's " + modelWWW.downloadHandler.text);
				yield break;
			}
			UnityEngine.Debug.Log("Successfully downloaded and deserialized model!");
			onModelDownloaded(result);
		}

		private string GetAskServerForModelURL()
		{
			if (mode == Mode.SETTINGS)
			{
				return homeURL + "game/getSettingsURL";
			}
			return homeURL + "game/getTranslationsURL";
		}
	}
}
