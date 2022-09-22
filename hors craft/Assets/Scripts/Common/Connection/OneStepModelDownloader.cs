// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Connection.OneStepModelDownloader
using Common.MiniJSON;
using Common.Utils;
using System.Collections;
using UnityEngine;

namespace Common.Connection
{
	public class OneStepModelDownloader : AbstractModelDownloader
	{
		public enum Mode
		{
			SETTINGS,
			TRANSLATIONS
		}

		public static SystemLanguage? languageToDownload;

		private Mode mode;

		public OneStepModelDownloader(string gamename, string homeURL, Mode mode)
			: base(gamename, homeURL)
		{
			this.mode = mode;
		}

		public override IEnumerator DownloadModelCoroutine(OnModelDownloaded onModelDownloaded)
		{
			string modelURL;
			if (mode == Mode.TRANSLATIONS)
			{
				SystemLanguage systemLanguage = Application.systemLanguage;
				if (languageToDownload.HasValue)
				{
					systemLanguage = languageToDownload.Value;
				}
				modelURL = GetTranslationsURL(systemLanguage.ToString().ToTitleCase());
			}
			else
			{
				modelURL = GetSettingsURL();
			}
			UnityEngine.Debug.Log("Trying to download model from " + modelURL + " for game: " + gamename + " ...");
			WWW modelWWW = new WWW(modelURL, FormFactory.CreateBasicWWWForm());
			yield return modelWWW;
			if (!string.IsNullOrEmpty(modelWWW.error))
			{
				UnityEngine.Debug.LogWarning("Server sent error: " + modelWWW.error);
				yield break;
			}
			if (string.IsNullOrEmpty(modelWWW.text))
			{
				UnityEngine.Debug.LogWarning("Server sent invalid (empty) model!");
				yield break;
			}
			object result = Json.Deserialize(modelWWW.text);
			if (result == null)
			{
				UnityEngine.Debug.LogWarning("Couldn't deserialize model! It's " + modelWWW.text);
				yield break;
			}
			UnityEngine.Debug.Log("Successfully downloaded and deserialized model!");
			onModelDownloaded(result);
		}

		private string GetSettingsURL()
		{
			return homeURL + "game/getSettings/platform/android/gamename/" + gamename + "/playerId/" + PlayerId.GetId();
		}

		private string GetTranslationsURL(string language)
		{
			return homeURL + "game/getTranslations/platform/android/language/" + language + "/gamename/" + gamename;
		}
	}
}
