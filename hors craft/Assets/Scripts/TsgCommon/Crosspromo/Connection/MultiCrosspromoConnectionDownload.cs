// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.Connection.MultiCrosspromoConnectionDownload
using System.Collections;
using System.IO;
using UnityEngine;

namespace TsgCommon.Crosspromo.Connection
{
	internal class MultiCrosspromoConnectionDownload : MultiCrosspromoConnection
	{
		public delegate void OnDefinitionDownloaded(MultiCrosspromoDefinition definition);

		private string containerUrl;

		private string containerFilePath;

		private string backgroundUrl;

		private string backgroundFilePath;

		public MultiCrosspromoConnectionDownload(string gamename, string homeURL, string playerId, string containerFilePath = null, string backgroundFilePath = null)
			: base(gamename, homeURL, playerId)
		{
			this.containerFilePath = containerFilePath;
			this.backgroundFilePath = backgroundFilePath;
		}

		public void Download(MonoBehaviour coroutinesProvider, OnDefinitionDownloaded onDefinitionDownloaded)
		{
			base.running = true;
			coroutinesProvider.StartCoroutine(DownloadDefinitionAndDisableRunning(coroutinesProvider, onDefinitionDownloaded));
		}

		private IEnumerator DownloadDefinitionAndDisableRunning(MonoBehaviour coroutinesProvider, OnDefinitionDownloaded onDefinitionDownloaded)
		{
			yield return coroutinesProvider.StartCoroutine(DownloadDefinition(coroutinesProvider, onDefinitionDownloaded));
			base.running = false;
		}

		private IEnumerator DownloadDefinition(MonoBehaviour coroutinesProvider, OnDefinitionDownloaded onDefinitionDownloaded)
		{
			string methodName = "viewBatch";
			WWW www = CreateCrosspromoViewRequest("viewBatch", MonoBehaviourSingleton<MultiCrosspromoController>.get.tags, MonoBehaviourSingleton<MultiCrosspromoController>.get.crosspromoAmount);
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				UnityEngine.Debug.LogWarning("Crosspromo: downloading definition WWW Error: " + www.error);
				yield break;
			}
			if (string.IsNullOrEmpty(www.text))
			{
				UnityEngine.Debug.LogWarning("Crosspromo: server sent empty response!");
				yield break;
			}
			UnityEngine.Debug.Log("Crosspromo response: " + www.text);
			bool batch = methodName == "viewBatch";
			int i = 0;
			while (true)
			{
				if (i >= MonoBehaviourSingleton<MultiCrosspromoController>.get.crosspromoAmount || !MultiCrosspromoDefinition.TryParseFromJSON(www.text, out MultiCrosspromoDefinition definition, i, batch))
				{
					yield break;
				}
				if (definition.textureUrl.Contains(".gif"))
				{
					definition.isGif = true;
				}
				else
				{
					definition.isGif = false;
					yield return coroutinesProvider.StartCoroutine(DownloadTexture(coroutinesProvider, definition));
				}
				if (!definition.IsValid())
				{
					break;
				}
				while (MonoBehaviourSingleton<MultiCrosspromoController>.get.containerUrl.IsNullOrEmpty())
				{
					yield return null;
				}
				containerUrl = MonoBehaviourSingleton<MultiCrosspromoController>.get.containerUrl;
				if (MonoBehaviourSingleton<MultiCrosspromoController>.get.containerSprite == null)
				{
					WWW image2 = new WWW(containerUrl);
					yield return image2;
					if (image2.bytes != null)
					{
						Directory.CreateDirectory(Path.GetDirectoryName(Application.persistentDataPath + containerFilePath));
						File.WriteAllBytes(Application.persistentDataPath + containerFilePath, image2.bytes);
						yield return new WaitForSecondsRealtime(0.2f);
					}
					definition.containerSprite = LoadNewSprite(Application.persistentDataPath + containerFilePath);
				}
				backgroundUrl = MonoBehaviourSingleton<MultiCrosspromoController>.get.backgroundUrl;
				if (backgroundUrl.IsNOTNullOrEmpty())
				{
					WWW image = new WWW(backgroundUrl);
					yield return image;
					if (image.bytes != null)
					{
						Directory.CreateDirectory(Path.GetDirectoryName(Application.persistentDataPath + backgroundFilePath));
						File.WriteAllBytes(Application.persistentDataPath + backgroundFilePath, image.bytes);
						yield return new WaitForSecondsRealtime(0.2f);
					}
					definition.backgroundSprite = LoadNewSprite(Application.persistentDataPath + backgroundFilePath);
				}
				onDefinitionDownloaded(definition);
				i++;
			}
			UnityEngine.Debug.LogWarning("Crosspromo: non-valid texture downloaded!");
		}

		private IEnumerator DownloadTexture(MonoBehaviour coroutinesProvider, CrosspromoDefinition definition)
		{
			WWW www = new WWW(definition.textureUrl);
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				UnityEngine.Debug.LogWarning("Crosspromo: downloading texture WWW Error: " + www.error);
				yield break;
			}
			definition.texture = new Texture2D(www.texture.width, www.texture.height);
			www.LoadImageIntoTexture(definition.texture);
		}

		private Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100f, SpriteMeshType spriteType = SpriteMeshType.Tight)
		{
			Texture2D texture2D = LoadTexture(filePath);
			if (texture2D == null)
			{
				return null;
			}
			return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f), pixelsPerUnit, 0u, spriteType);
		}

		private Texture2D LoadTexture(string filePath)
		{
			if (File.Exists(filePath))
			{
				byte[] data = File.ReadAllBytes(filePath);
				Texture2D texture2D = new Texture2D(2, 2);
				if (texture2D.LoadImage(data))
				{
					return texture2D;
				}
			}
			return null;
		}
	}
}
