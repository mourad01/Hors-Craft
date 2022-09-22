// DecompilerFi decompiler from Assembly-CSharp.dll class: DidYouKnowManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using States;
using System.Collections;
using System.IO;
using UnityEngine;

public class DidYouKnowManager : Manager
{
	private const string currentDidYouKnowIndexID = "currentDidYouKnowIndexID";

	private Pair<string, string>[] videoImagePairNames;

	private int videoCount;

	public bool isModelDownloaded
	{
		get;
		private set;
	}

	public override void Init()
	{
		isModelDownloaded = false;
		Manager.Get<ModelManager>().timeBasedDidYouKnow.OnModelModuleDownloaded = OnModelDownloaded;
	}

	public void OnModelDownloaded()
	{
		videoImagePairNames = Manager.Get<ModelManager>().timeBasedDidYouKnow.GetVideoImagePairNames();
		videoCount = GetVideoCount();
		isModelDownloaded = true;
	}

	public int GetDidYouKnowIndexToDisplay()
	{
		int num = PlayerPrefs.GetInt("currentDidYouKnowIndexID", 0);
		if (num >= videoCount)
		{
			num = 0;
			PlayerPrefs.SetInt("currentDidYouKnowIndexID", num);
		}
		return num;
	}

	public bool NextHintEnabled()
	{
		return videoCount > 1;
	}

	public bool GoToFeatureEnabled()
	{
		int @int = PlayerPrefs.GetInt("currentDidYouKnowIndexID", 0);
		return Manager.Get<ModelManager>().timeBasedDidYouKnow.IsGoToFeatureEnabled(@int);
	}

	public void SetNextDidYouKnowIndex()
	{
		int @int = PlayerPrefs.GetInt("currentDidYouKnowIndexID", 0);
		@int = ((@int + 1 < videoCount) ? (@int + 1) : 0);
		PlayerPrefs.SetInt("currentDidYouKnowIndexID", @int);
	}

	public void OnGoToState()
	{
		int @int = PlayerPrefs.GetInt("currentDidYouKnowIndexID", 0);
		GoToState goToState = Manager.Get<ModelManager>().timeBasedDidYouKnow.GetGoToState(@int);
		if (goToState == GoToState.Crafting || goToState == GoToState.Blueprints)
		{
			Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
			{
				canSave = true,
				allowTimeChange = true,
				categoryToOpen = "Crafting"
			});
		}
	}

	public void OnPopupClose()
	{
		SetNextDidYouKnowIndex();
		if (!AllVideosDownloaded())
		{
			StartCoroutine(DownloadVideo(videoCount));
			StartCoroutine(DownloadPlayImage(videoCount));
		}
	}

	public bool HasToShowDidYouKnow()
	{
		return isModelDownloaded && Manager.Get<ModelManager>().timeBasedDidYouKnow.HasToShowDidYouKnow(Time.timeSinceLevelLoad);
	}

	public string GetVideoFilePath(int videoIndex)
	{
		if (videoIndex == 0)
		{
			return videoImagePairNames[videoIndex].First;
		}
		return Application.persistentDataPath + "/" + videoImagePairNames[videoIndex].First;
	}

	public string GetPlayImageFilePath(int videoIndex)
	{
		if (videoIndex >= videoImagePairNames.Length)
		{
			return string.Empty;
		}
		if (videoIndex == 0)
		{
			return Application.streamingAssetsPath + "/" + videoImagePairNames[videoIndex].Second;
		}
		return Application.persistentDataPath + "/" + videoImagePairNames[videoIndex].Second;
	}

	private IEnumerator DownloadVideo(int currentVideoCount)
	{
		string url = Manager.Get<ModelManager>().timeBasedDidYouKnow.GetVideoUrl(videoImagePairNames[videoCount].First);
		WWW wwwUrl = new WWW(url, FormFactory.CreateBasicWWWForm());
		yield return wwwUrl;
		string videoUrl = JSONHelper.Deserialize<string>(wwwUrl.text);
		UnityEngine.Debug.Log("Video download started from : " + videoUrl);
		WWW wwwVideo = new WWW(videoUrl, FormFactory.CreateBasicWWWForm());
		yield return wwwVideo;
		if (wwwVideo != null && wwwVideo.isDone && wwwVideo.error == null)
		{
			File.WriteAllBytes(GetVideoFilePath(videoCount), wwwVideo.bytes);
			videoCount++;
			PlayerPrefs.SetInt("currentDidYouKnowIndexID", videoCount - 1);
			UnityEngine.Debug.Log("'DidYouKnow' video downloaded");
		}
		else
		{
			UnityEngine.Debug.LogWarning("'DidYouKnow' video download error!\nDownload source: " + videoUrl);
			UnityEngine.Debug.LogWarning($"wwwVideo.isDone: {wwwVideo.isDone}\nwwwVideo.error: {wwwVideo.error}");
		}
	}

	private IEnumerator DownloadPlayImage(int currentVideoCount)
	{
		string url = Manager.Get<ModelManager>().timeBasedDidYouKnow.GetVideoUrl(videoImagePairNames[currentVideoCount].Second);
		WWW wwwUrl = new WWW(url, FormFactory.CreateBasicWWWForm());
		yield return wwwUrl;
		string imageUrl = JSONHelper.Deserialize<string>(wwwUrl.text);
		UnityEngine.Debug.Log("Image download started from : " + imageUrl);
		WWW wwwImage = new WWW(imageUrl, FormFactory.CreateBasicWWWForm());
		yield return wwwImage;
		if (wwwImage != null && wwwImage.isDone && wwwImage.error == null)
		{
			File.WriteAllBytes(GetPlayImageFilePath(currentVideoCount), wwwImage.bytes);
			UnityEngine.Debug.Log("'DidYouKnow' playImage downloaded");
		}
		else
		{
			UnityEngine.Debug.LogWarning("'DidYouKnow' playImage download error!\nDownload source: " + imageUrl);
			UnityEngine.Debug.LogWarning($"wwwImage.isDone: {wwwImage.isDone}\nwwwImage.error: {wwwImage.error}");
		}
	}

	private bool AllVideosDownloaded()
	{
		return GetVideoCount() >= videoImagePairNames.Length;
	}

	private int GetVideoCount()
	{
		int i;
		for (i = 1; i < videoImagePairNames.Length && File.Exists(GetVideoFilePath(i)); i++)
		{
		}
		return i;
	}
}
