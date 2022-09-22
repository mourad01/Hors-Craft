// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldDownloader
using Common.Managers;
using Ionic.Zlib;
using MiniJSON;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public class WorldDownloader : MonoBehaviour
{
	public bool downloading;

	public string nameToDownload;

	private string indetifier;

	public List<string> files;

	private string cdnPathWorld;

	private Dictionary<string, string> shopImageCdnPath;

	private Dictionary<string, string> texturesImagePaths;

	private Dictionary<string, object> mainJson;

	private Thread thread;

	private string persistentDataPath;

	private string text;

	private bool threadStarted;

	private WWW mainDownloader;

	private Texture[] downloadedTextures;

	public bool isWorldReadyToPlay;

	private SavedWorldManager.DownloadWorld downloadedWorld;

	public string tilesetName;

	public bool wasDownloadSuccesfull;

	private Action<SavedWorldManager.DownloadWorld> lastOnWorldDownloaded;

	private bool commpresed = true;

	private const float DOWNLOAD_TIMEOUT = 30f;

	private float lastProgressValue;

	private float timeWithoutChange;

	private bool errorShown;

	public void Init(SavedWorldManager.DownloadWorld initial, Action<string> onError)
	{
		downloading = true;
		downloadedWorld = initial;
		persistentDataPath = Application.persistentDataPath;
		nameToDownload = initial.id;
		shopImageCdnPath = new Dictionary<string, string>();
		texturesImagePaths = new Dictionary<string, string>();
		downloadedTextures = new Texture[initial.textures.Count];
		if (initial.textures.Count > 0)
		{
			tilesetName = initial.textures[0];
		}
		StartCoroutine(GetCDNPath("WorldPath", nameToDownload, OnPathDownloaded, onError));
		StartCoroutine(GetCDNPath("ShopWorldImagePath", initial.shop_image, delegate(string result)
		{
			shopImageCdnPath[initial.shop_image] = result;
		}, onError));
		int i = 0;
		initial.textures.ForEach(delegate(string name)
		{
			StartCoroutine(GetCDNPath("WorldTexturePath", name, delegate(string result)
			{
				texturesImagePaths[name] = result;
				i++;
			}, onError));
		});
	}

	public List<SavedWorldManager.Animal> GetWorldsAnimals()
	{
		return downloadedWorld.animals;
	}

	public string GetPathToTileset()
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, string> texturesImagePath in texturesImagePaths)
		{
			if (texturesImagePath.Key.ToUpper().Contains("TILESET"))
			{
				text = texturesImagePath.Value;
				break;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		return GetPathToImage(text);
	}

	public string GetTilesetName()
	{
		return tilesetName;
	}

	public void DownloadWorldForPlaying(Action<SavedWorldManager.DownloadWorld> onWorldDownloaded)
	{
		TryToUpdateLoading(GetDownloadText(), 0f);
		lastProgressValue = 0f;
		timeWithoutChange = 0f;
		lastOnWorldDownloaded = onWorldDownloaded;
		StartCoroutine(DownloadWorldCooroutine(cdnPathWorld, delegate
		{
			DownloadWorldTextures(delegate
			{
				isWorldReadyToPlay = true;
				onWorldDownloaded(downloadedWorld);
				lastOnWorldDownloaded = null;
				mainDownloader = null;
				wasDownloadSuccesfull = true;
			});
		}));
	}

	private void DownloadWorldTextures(Action onTexturesDownloaded)
	{
		int textureDownloadedCounter = 0;
		foreach (KeyValuePair<string, string> texturesImagePath in texturesImagePaths)
		{
			StartCoroutine(GetImage(texturesImagePath.Key, texturesImagePath.Value, delegate(Texture texture)
			{
				downloadedTextures[textureDownloadedCounter] = texture;
				textureDownloadedCounter++;
				if (textureDownloadedCounter == downloadedTextures.Length)
				{
					onTexturesDownloaded();
				}
			}, saveToFile: true));
		}
	}

	public void DownloadShopImage(Action<Texture> onTextureDownload)
	{
		if (shopImageCdnPath != null && shopImageCdnPath.Count != 0)
		{
			Dictionary<string, string>.Enumerator enumerator = shopImageCdnPath.GetEnumerator();
			enumerator.MoveNext();
			string key = enumerator.Current.Key;
			if (shopImageCdnPath.ContainsKey(key) || !string.IsNullOrEmpty(shopImageCdnPath[key]))
			{
				StartCoroutine(GetImage(key, shopImageCdnPath[key], onTextureDownload));
			}
		}
	}

	public static WorldDownloader InitializeDownloader(SavedWorldManager.DownloadWorld initial, Action<string> onError)
	{
		GameObject gameObject = GameObject.Find("downloaderRoot");
		if (gameObject == null)
		{
			gameObject = new GameObject("downloaderRoot");
		}
		GameObject gameObject2 = new GameObject(initial.display_name + "_downloader");
		WorldDownloader worldDownloader = gameObject2.AddComponent<WorldDownloader>();
		worldDownloader.Init(initial, onError);
		gameObject2.transform.SetParent(gameObject.transform);
		return worldDownloader;
	}

	private void Unzip()
	{
		if (commpresed)
		{
			text = DecompressGzipedWorlds(Convert.FromBase64String(text));
		}
		else
		{
			text = Encoding.UTF8.GetString(Convert.FromBase64String(text));
		}
		mainJson = (Json.Deserialize(text) as Dictionary<string, object>);
		SaveFiles();
		downloading = false;
		threadStarted = false;
	}

	private string GetPathToImage(string filename)
	{
		return $"{persistentDataPath}/Worlds/{nameToDownload}/{filename}";
	}

	public void SaveImage(Texture texture, string fileName)
	{
		SaveImageToFile(texture, fileName);
	}

	private void SaveImageToFile(Texture texture, string fileName)
	{
		byte[] bytes = (texture as Texture2D).EncodeToPNG();
		string pathToImage = GetPathToImage(fileName);
		CreateIfDontExist();
		File.WriteAllBytes(pathToImage, bytes);
	}

	public string CreateIfDontExist()
	{
		string text = $"{persistentDataPath}/Worlds/{nameToDownload}";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public void SaveFiles()
	{
		string arg = CreateIfDontExist();
		foreach (string key in mainJson.Keys)
		{
			if (!key.Equals("worldName"))
			{
				byte[] bytes = Convert.FromBase64String(mainJson[key].ToString());
				File.WriteAllBytes($"{arg}/{key}", bytes);
			}
		}
	}

	private string DecompressGzipedWorlds(byte[] bytes)
	{
		using (GZipStream stream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
		{
			using (StreamReader streamReader = new StreamReader(stream))
			{
				return streamReader.ReadToEnd();
			}
		}
	}

	public void OnPathDownloaded(string cdnPath)
	{
		cdnPathWorld = cdnPath;
	}

	private float howManyTexturesIsNotNull()
	{
		float num = 0f;
		for (int i = 0; i < downloadedTextures.Length; i++)
		{
			if (downloadedTextures[i] != null)
			{
				num += (float)(1 / downloadedTextures.Length);
			}
		}
		return num;
	}

	private IEnumerator GetImage(string imageName, string path, Action<Texture> onDownloaded, bool saveToFile = false)
	{
		path = path.Replace("\\", string.Empty).Trim('"');
		WWW www = new WWW(path);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (saveToFile)
			{
				SaveImage(www.texture, imageName);
			}
			onDownloaded(www.texture);
		}
		else
		{
			UnityEngine.Debug.LogError(www.error + " " + imageName);
		}
	}

	private IEnumerator GetCDNPath(string what, string fileName, Action<string> onDownloaded, Action<string> onError = null)
	{
		string path2 = Manager.Get<ConnectionInfoManager>().homeURL;
		path2 = $"{path2}CraftWorlds/{what}?file={fileName}";
		WWW www = new WWW(path2);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			onDownloaded(www.text);
		}
		else if (onError != null)
		{
			UnityEngine.Debug.LogError("Failed to download:" + path2);
			onError(nameToDownload);
		}
	}

	private void Update()
	{
		if (mainDownloader != null && !wasDownloadSuccesfull)
		{
			float progress = GetProgress();
			CheckErrorAndTryToUpdate(progress);
		}
	}

	public float GetProgress()
	{
		if (mainDownloader != null)
		{
			return mainDownloader.progress * 0.6f + 0.25f * howManyTexturesIsNotNull() + ((thread != null && !thread.IsAlive) ? 0.15f : 0f);
		}
		return 0f;
	}

	private void CheckErrorAndTryToUpdate(float value)
	{
		if (!mainDownloader.error.IsNullOrEmpty())
		{
			value = 0f;
			if (!errorShown)
			{
				OnWorldDownloadError();
			}
		}
		if (value > lastProgressValue)
		{
			lastProgressValue = value;
			timeWithoutChange = 0f;
			TryToUpdateLoading(GetDownloadText(), value);
			return;
		}
		timeWithoutChange += Time.deltaTime;
		if (timeWithoutChange > 30f)
		{
			OnWorldDownloadError();
		}
	}

	private void TryToUpdateLoading(string text, float value)
	{
		EmptyLoadingState emptyLoadingState = Manager.Get<StateMachineManager>().currentState as EmptyLoadingState;
		if (emptyLoadingState != null)
		{
			emptyLoadingState.UpdateProgressState(text, value);
		}
	}

	private string GetDownloadText()
	{
		return Manager.Get<TranslationsManager>().GetText("loading.tag.downloading", "Downloading:");
	}

	private IEnumerator DownloadWorldCooroutine(string url, Action onWorldDownloaded)
	{
		url = url.Replace("\\", string.Empty).Trim('"');
		mainDownloader = new WWW(url);
		yield return mainDownloader;
		if (mainDownloader.error.IsNullOrEmpty())
		{
			text = mainDownloader.text;
			thread = new Thread(Unzip);
			thread.Start();
			threadStarted = true;
			while (threadStarted)
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}
			mainDownloader = null;
			onWorldDownloaded();
		}
		else
		{
			OnWorldDownloadError();
		}
	}

	private void OnWorldDownloadError()
	{
		errorShown = true;
		Manager.Get<StateMachineManager>().PushState<GenericPopupTitleState>(GenericPopupStateStartParameter.OnInternetError(delegate
		{
			if (WorldsFragment.areWorldsFromPause)
			{
				Manager.Get<StateMachineManager>().PopStatesUntil<PauseState>();
			}
			else
			{
				Manager.Get<StateMachineManager>().PopStatesUntil<WorldShopState>();
			}
		}, delegate
		{
			Manager.Get<StateMachineManager>().PopState();
			DownloadWorldForPlaying(lastOnWorldDownloaded);
			Manager.Get<StateMachineManager>().PushState<EmptyLoadingState>(new LoadingStartParameter("loading.tag.downloading", "Downloading:"));
		}));
	}

	private void Fake()
	{
		threadStarted = false;
	}
}
