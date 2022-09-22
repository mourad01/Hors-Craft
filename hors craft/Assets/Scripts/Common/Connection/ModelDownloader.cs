// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Connection.ModelDownloader
using Common.MiniJSON;
using System.Collections;
using UnityEngine;

namespace Common.Connection
{
	public class ModelDownloader : AbstractModelDownloader
	{
		public ModelDownloader(string gamename, string homeURL)
			: base(gamename, homeURL)
		{
		}

		public override IEnumerator DownloadModelCoroutine(OnModelDownloaded onModelDownloaded)
		{
			string url2 = homeURL + "game/content";
			string modelId = gamename;
			url2 = url2 + "?modelId=" + modelId.ToString();
			WWW www = new WWW(url2, FormFactory.CreateBasicWWWForm());
			UnityEngine.Debug.Log("Trying to download model from " + url2 + " with modelId: " + modelId);
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				UnityEngine.Debug.LogWarning("No connection to server! WWW Error: " + www.error);
				yield break;
			}
			object jsonResult = null;
			if (!string.IsNullOrEmpty(www.text))
			{
				jsonResult = Json.Deserialize(www.text);
				UnityEngine.Debug.Log("Deserialized JSON text: " + www.text);
			}
			else
			{
				UnityEngine.Debug.Log("Cannot deserialize JSON text: " + www.text);
			}
			UnityEngine.Debug.Log("Successfully downloaded model from " + url2 + " with modelId: " + modelId);
			onModelDownloaded(jsonResult);
		}
	}
}
