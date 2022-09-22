// DecompilerFi decompiler from Assembly-CSharp.dll class: DynamicOfferPackManager
using Common.Gameplay;
using Common.Managers;
using Common.Utils;
using Gameplay;
using States;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class DynamicOfferPackManager : OfferPackManager
{
	private const string KEY_CURRENTPACK_NR = "currentdynamicstarterpack";

	private const string KEY_CURRENTPACK_TIMESTAMP = "currentdynamicstarterpacktimestamp";

	private const string KEY_CURRENTPACK_SHOWNTIMES = "currentdynamicstarterpacktimesshown";

	public const string KEY_OVERRIDE_CLOTHES_ADS = "overrideclothesadsnumber";

	public const string KEY_OVERRIDE_PETS_ADS = "overridepetsadsnumber";

	public const string KEY_OVERRIDE_BLOCKS_ADS = "overrideblocksadsnumber";

	public const string KEY_OVERRIDE_SCHEMATICS_ADS = "overrideschematicsadsnumber";

	public const string KEY_SHOULD_SHOW_BUTTON = "dynamicstarterpack.shouldShowButton";

	public const string KEY_SKIPPED_ON_FIRST_RUN = "dynamicstarterpack.skippedOnFirstRun";

	public const string KEY_LAST_IMAGES_DOWNLOAD_DATE = "DynamicOfferpackLastDownloadDateTime";

	private const string timerFormatLong = "{0:D2}H {1:D2}M {2:D2}S";

	private const string timerFormatShort = "{0:D2}:{1:D2}:{2:D2}";

	public bool forceOpen;

	public bool skipShowingOnFirstRun;

	private bool skipShowing;

	public bool forceDynamicStarterPacksEnabled;

	public const string PACK_BOUGHT_KEY = "pack{0}bought";

	public readonly string[] OFFERPACK_IDS = new string[3]
	{
		"pack.decorators",
		"pack.engineer",
		"pack.material"
	};

	private bool shouldShowButton;

	public static string PATH_PACKS_BACKGROUND_FOLDER => Application.persistentDataPath + "/dynamicOfferPack/";

	private OfferSettingsModule model => Manager.Get<ModelManager>().offerPackSettings;

	protected bool ShouldShowButton
	{
		get
		{
			return shouldShowButton;
		}
		set
		{
			shouldShowButton = value;
			PlayerPrefs.SetInt("dynamicstarterpack.shouldShowButton", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public override void Init()
	{
		if (PlayerPrefs.HasKey("dynamicstarterpack.shouldShowButton"))
		{
			shouldShowButton = ((PlayerPrefs.GetInt("dynamicstarterpack.shouldShowButton", 0) != 0) ? true : false);
		}
		if (skipShowingOnFirstRun && !PlayerPrefs.HasKey("dynamicstarterpack.skippedOnFirstRun"))
		{
			skipShowing = true;
			PlayerPrefs.SetInt("dynamicstarterpack.skippedOnFirstRun", 0);
		}
	}

	public override bool ShouldShowOfferPack(int timeInGameplay)
	{
		return false;
	}

	public override void OnStarterShow()
	{
	}

	public override void OnWorldBought(string worldId)
	{
	}

	public override bool ShouldShowStarterPack(int timeInGameplay)
	{
		return false;
	}

	public override bool ShouldShowPackButton()
	{
		if (!IsButtonSpriteExist())
		{
			return false;
		}
		double num = LoadFirstShownTimestamp();
		int index = LoadCurrentDynamicStarterPack();
		int dynamicStarterPackAvaliabilityTime = base.settings.GetDynamicStarterPackAvaliabilityTime(index);
		double num2 = base.timestamp - num;
		int num3 = LoadDynamicStarterPackTimesShown();
		int dynamicStarterPackMaxShowsRaw = base.settings.GetDynamicStarterPackMaxShowsRaw(index);
		if (!ShouldShowButton)
		{
			return false;
		}
		if (num2 < (double)dynamicStarterPackAvaliabilityTime && num3 > 0 && dynamicStarterPackMaxShowsRaw != -1)
		{
			return true;
		}
		return false;
	}

	public string GetItemShopId()
	{
		int num = LoadCurrentDynamicStarterPack();
		return OFFERPACK_IDS[num];
	}

	public string GetFormattedTimeToEnd(bool longFormat = false)
	{
		int index = LoadCurrentDynamicStarterPack();
		int dynamicStarterPackAvaliabilityTime = base.settings.GetDynamicStarterPackAvaliabilityTime(index);
		double num = LoadFirstShownTimestamp();
		double value = Mathf.Clamp((float)(num + (double)dynamicStarterPackAvaliabilityTime - base.timestamp), 0f, float.MaxValue);
		TimeSpan timeSpan = TimeSpan.FromSeconds(value);
		return string.Format((!longFormat) ? "{0:D2}:{1:D2}:{2:D2}" : "{0:D2}H {1:D2}M {2:D2}S", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public override void OnStarterPackWillShow()
	{
	}

	public override void OnStarterPackWasShown()
	{
		UpdateTimesShown();
		PlayerPrefs.SetFloat("lastTimeShown", (float)base.timestamp);
		PlayerPrefs.Save();
	}

	public override OfferPackDefinition GetStarterPack()
	{
		return new OfferPackDefinition();
	}

	public Sprite GetSpriteForButton()
	{
		string path = $"{PATH_PACKS_BACKGROUND_FOLDER}{model.GetDynamicStarterButtonImage()}";
		return LoadSprite(path);
	}

	public bool IsButtonSpriteExist()
	{
		string path = $"{PATH_PACKS_BACKGROUND_FOLDER}{model.GetDynamicStarterButtonImage()}";
		return File.Exists(path);
	}

	public bool IsOfferpackSpriteExist(int id)
	{
		string path = $"{PATH_PACKS_BACKGROUND_FOLDER}{model.GetDynamicStarterBackgroundImage(id)}";
		return File.Exists(path);
	}

	public override void GrantCurrentPack()
	{
		ShouldShowButton = false;
		int num = LoadCurrentDynamicStarterPack();
		OfferSettingsModule.RewardConfig[] activeStarterPackWithNewRewards = base.settings.GetActiveStarterPackWithNewRewards(num);
		if (activeStarterPackWithNewRewards.IsNullOrEmpty())
		{
			return;
		}
		OfferSettingsModule.RewardConfig[] array = activeStarterPackWithNewRewards;
		for (int i = 0; i < array.Length; i++)
		{
			OfferSettingsModule.RewardConfig rewardConfig = array[i];
			DailyRewards id = (DailyRewards)rewardConfig.id;
			for (int j = 0; j < rewardConfig.count; j++)
			{
				Reward rewardObject = Manager.Get<RewardsManager>().GetRewardObject(id.ToString().ToLower());
				rewardObject.ClaimReward();
			}
		}
		PlayerPrefs.SetInt($"pack{num}bought", 1);
		PlayerPrefs.Save();
		UpdateCurrentPack();
	}

	public override void GrantPack(OfferPackDefinition offerPack)
	{
	}

	public int LoadCurrentDynamicStarterPack()
	{
		return PlayerPrefs.GetInt("currentdynamicstarterpack", 0);
	}

	private void SaveCurrentDynamicStarterPack(int current)
	{
		PlayerPrefs.SetInt("currentdynamicstarterpack", current);
		PlayerPrefs.Save();
	}

	private void UpdateCurrentPack()
	{
		int num = LoadCurrentDynamicStarterPack();
		SaveCurrentDynamicStarterPack(Mathf.Clamp(num + 1, 0, base.settings.GetNrOfDynamicStarterPacks() - 1));
		SaveTimesShown(0);
	}

	private double LoadFirstShownTimestamp()
	{
		return PlayerPrefs.GetFloat("currentdynamicstarterpacktimestamp", 0f);
	}

	private void SaveFirstShownTimestamp(double timestamp)
	{
		ShouldShowButton = true;
		PlayerPrefs.SetFloat("currentdynamicstarterpacktimestamp", (float)timestamp);
		PlayerPrefs.Save();
	}

	private void UpdateTimesShown()
	{
		SaveTimesShown(LoadDynamicStarterPackTimesShown() + 1);
	}

	private void SaveTimesShown(int timesShown)
	{
		PlayerPrefs.SetInt("currentdynamicstarterpacktimesshown", timesShown);
		PlayerPrefs.Save();
	}

	private int LoadDynamicStarterPackTimesShown()
	{
		return PlayerPrefs.GetInt("currentdynamicstarterpacktimesshown", 0);
	}

	[ContextMenu("Show DynamicStarterPackState")]
	public void DebugShowDynamicStarterPackState()
	{
		StateMachineManager stateMachineManager = Manager.Get<StateMachineManager>();
		if (!stateMachineManager.IsCurrentStateA<PauseState>())
		{
			stateMachineManager.PushState<PauseState>();
		}
		stateMachineManager.PushState<StarterPackState>();
	}

	private bool AreDynamicStarterPacksEnabled()
	{
		return base.settings.AreDynamicStarterPacksEnabled() || forceDynamicStarterPacksEnabled;
	}

	public void DownloadOfferpacksImages(string[] names)
	{
		if (PlayerPrefs.HasKey("DynamicOfferpackLastDownloadDateTime"))
		{
			double imagesUpdatingIntervalInDays = base.settings.GetImagesUpdatingIntervalInDays();
			DateTime now = DateTime.Now;
			long dateData = Convert.ToInt64(PlayerPrefs.GetString("DynamicOfferpackLastDownloadDateTime"));
			DateTime value = DateTime.FromBinary(dateData);
			double totalDays = now.Subtract(value).TotalDays;
			if (totalDays < imagesUpdatingIntervalInDays)
			{
				UnityEngine.Debug.Log("Skipping downloading offerpack images");
				return;
			}
		}
		foreach (string text in names)
		{
			if (!text.IsNullOrEmpty())
			{
				StartCoroutine(DownloadSprite(text));
			}
		}
		PlayerPrefs.SetString("DynamicOfferpackLastDownloadDateTime", DateTime.Now.ToBinary().ToString());
		PlayerPrefs.Save();
	}

	private IEnumerator DownloadSprite(string spriteName)
	{
		string url = GetSpriteUrl(spriteName);
		WWW wwwUrl = new WWW(url, FormFactory.CreateBasicWWWForm());
		yield return wwwUrl;
		string imageUrl = JSONHelper.Deserialize<string>(wwwUrl.text);
		UnityEngine.Debug.Log("Image download started from : " + imageUrl);
		WWW wwwImage = new WWW(imageUrl, FormFactory.CreateBasicWWWForm());
		yield return wwwImage;
		if (wwwImage != null && wwwImage.isDone && wwwImage.error == null)
		{
			CreatePathIfDontExist();
			File.WriteAllBytes(GetSpritePath(spriteName), wwwImage.bytes);
			UnityEngine.Debug.Log("'DynamicStarterPack' image downloaded");
			yield break;
		}
		UnityEngine.Debug.LogWarning("'DynamicStarterPack' image download error!\nDownload source: " + imageUrl);
		UnityEngine.Debug.LogWarning($"wwwImage.isDone: {wwwImage.isDone}\nwwwImage.error: {wwwImage.error}");
		if (PlayerPrefs.HasKey("DynamicOfferpackLastDownloadDateTime"))
		{
			PlayerPrefs.DeleteKey("DynamicOfferpackLastDownloadDateTime");
			PlayerPrefs.Save();
		}
	}

	public string GetSpritePath(string spriteName)
	{
		return PATH_PACKS_BACKGROUND_FOLDER + spriteName;
	}

	public string GetSpriteUrl(string spriteName)
	{
		string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
		return $"{homeURL}DownloadFile/Path?filePath=dynamicOfferPack/{spriteName}";
	}

	public void CreatePathIfDontExist()
	{
		if (!Directory.Exists(PATH_PACKS_BACKGROUND_FOLDER))
		{
			Directory.CreateDirectory(PATH_PACKS_BACKGROUND_FOLDER);
		}
	}

	private Sprite LoadSprite(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		if (File.Exists(path))
		{
			byte[] data = File.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(4, 4, TextureFormat.RGBA32, mipChain: false);
			texture2D.LoadImage(data);
			return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
		}
		return null;
	}
}
