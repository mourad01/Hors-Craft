// DecompilerFi decompiler from Assembly-CSharp.dll class: UltimateCraftModelDownloader
using Common.Managers;
using Common.Utils;
using Gameplay;
using MiniJSON;
using System.Collections.Generic;
using UnityEngine;

public class UltimateCraftModelDownloader : Singleton<UltimateCraftModelDownloader>
{
	private const string KEY_BACKUP_MODEL = "ultimate_model_json";

	private List<PackageData> downloadedPackages;

	private Dictionary<string, WorldFilter> downloadedFilters;

	private Dictionary<string, WorldTag> downloadedTags;

	private Dictionary<string, int> costOfClothes;

	private Dictionary<ushort, int> costOfBlocks;

	public string GetPriceString(string iapId)
	{
		PackageData packageData = GetIapPackages().Find((PackageData data) => data.iapIdentifier == iapId);
		return (packageData == null) ? string.Empty : packageData.fakeCost;
	}

	public List<PackageData> GetIapPackages()
	{
		if (downloadedPackages == null)
		{
			return GetIapsData();
		}
		return downloadedPackages;
	}

	public Dictionary<string, WorldFilter> GetFilters()
	{
		if (downloadedFilters == null)
		{
			return GetFilterData();
		}
		return downloadedFilters;
	}

	public Dictionary<string, WorldTag> GetTags()
	{
		if (downloadedTags == null)
		{
			return GetTagData();
		}
		return downloadedTags;
	}

	public int GetPriceOfBlock(ushort blockId)
	{
		if (costOfBlocks != null && costOfBlocks.ContainsKey(blockId))
		{
			return costOfBlocks[blockId];
		}
		return Manager.Get<ModelManager>().worldsSettings.GetCostOfBlockDefault();
	}

	public int GetPriceOfClothes(string id)
	{
		if (costOfClothes == null)
		{
			return Manager.Get<ModelManager>().worldsSettings.GetCostOfClothesDefault();
		}
		if (costOfClothes.ContainsKey(id))
		{
			return costOfClothes[id];
		}
		return 0;
	}

	private List<PackageData> GetIapsData()
	{
		List<PackageData> list = new List<PackageData>();
		list.Add(new PackageData("free pack", "currencyshop.package.video", 100, string.Empty, string.Empty, isVideo: true));
		list.Add(new PackageData("small pack", "currencyshop.package.small", 1000, "iap.iap", string.Empty, isVideo: false));
		list.Add(new PackageData("medium pack", "currencyshop.package.medium", 10000, "iap.iap", "currencyshop.label.popular", isVideo: false));
		list.Add(new PackageData("big pack", "currencyshop.package.big", 100000, "iap.iap", "currencyshop.label.bestvalue", isVideo: false));
		return list;
	}

	private Dictionary<string, WorldFilter> GetFilterData()
	{
		Dictionary<string, WorldFilter> dictionary = new Dictionary<string, WorldFilter>();
		dictionary.Add("ALL", new WorldFilter("ALL", "shop.filters.all", enabled: true));
		dictionary.Add("NEWEST", new WorldFilter("NEWEST", "shop.filters.newest", enabled: true));
		dictionary.Add("POPULAR", new WorldFilter("POPULAR", "shop.filters.popular", enabled: true));
		dictionary.Add("MY_WORLDS", new WorldFilter("MY_WORLDS", "shop.filters.my_worlds", enabled: true));
		return dictionary;
	}

	private Dictionary<string, WorldTag> GetTagData()
	{
		Dictionary<string, WorldTag> dictionary = new Dictionary<string, WorldTag>();
		dictionary.Add("new", new WorldTag("new", "shop.tag.new", visible: true, new Color(1f, 0.65f, 0f)));
		dictionary.Add("popular", new WorldTag("new", "shop.tag.popular", visible: true, new Color(1f, 1f, 0f)));
		return dictionary;
	}

	public void DownloadModel()
	{
		string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		string value = "ios";
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
		{
			value = "android";
		}
		dictionary.Add("platform", value);
		dictionary.Add("gamename", Manager.Get<ConnectionInfoManager>().gameName);
		dictionary.Add("playerId", PlayerId.GetId());
		SimpleRequestMaker.MakeRequest(homeURL, "CraftWorlds/GetModel", dictionary, ParseMain, OnError);
	}

	private void OnError(string error)
	{
		UnityEngine.Debug.LogError("Model cannot be downloaded!");
		string @string = PlayerPrefs.GetString("ultimate_model_json", string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			UnityEngine.Debug.LogError("No first backup model, trying reading from file!");
			TryingToReadFromFile();
		}
		else
		{
			ParseMain(@string);
		}
	}

	private void TryingToReadFromFile()
	{
		string text = Resources.Load<TextAsset>("Backup/ultimateConfigurationBackup").text;
		if (string.IsNullOrEmpty(text))
		{
			throw new NoModelException("No ultimate model!");
		}
		ParseMain(text);
	}

	public int FindCurrencySizeOfPackage(string identifier)
	{
		return downloadedPackages.Find((PackageData package) => package.iapIdentifier.Equals(identifier))?.currencyCount ?? 0;
	}

	public WorldFilter GetFilter(string tag)
	{
		if (downloadedFilters.ContainsKey(tag))
		{
			return downloadedFilters[tag];
		}
		return null;
	}

	public string GetTagText(string tag)
	{
		if (downloadedTags.ContainsKey(tag))
		{
			return Manager.Get<TranslationsManager>().GetText(downloadedTags[tag].value, tag);
		}
		return string.Empty;
	}

	public bool GetTagVisiblity(string tag)
	{
		if (downloadedTags.ContainsKey(tag))
		{
			return downloadedTags[tag].isVisible;
		}
		return false;
	}

	public Color GetTagColor(string tag)
	{
		if (downloadedTags.ContainsKey(tag))
		{
			return downloadedTags[tag].colorValue;
		}
		return Color.white;
	}

	private void ParseMain(WWW data)
	{
		ParseMain(data.text);
	}

	private void ParseMain(string text)
	{
		UnityEngine.Debug.Log("UltimateModel: " + text);
		PlayerPrefs.SetString("ultimate_model_json", text);
		Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
		List<object> iaps = dictionary["iaps"] as List<object>;
		List<object> worlds = dictionary["worlds"] as List<object>;
		List<object> filters = dictionary["filters"] as List<object>;
		List<object> tags = dictionary["tags"] as List<object>;
		Dictionary<string, object> cost = dictionary["cost"] as Dictionary<string, object>;
		Dictionary<string, object> quests = dictionary["quests"] as Dictionary<string, object>;
		ParseIaps(iaps);
		ParseWorlds(worlds);
		ParseFilters(filters);
		ParseTags(tags);
		ParseCost(cost);
		ParseQuests(quests);
		GetIapPackages();
		GetFilters();
		GetTags();
	}

	private void ParseCost(Dictionary<string, object> cost)
	{
		List<object> list = cost["blocks"] as List<object>;
		List<object> list2 = cost["clothes"] as List<object>;
		costOfBlocks = new Dictionary<ushort, int>();
		costOfClothes = new Dictionary<string, int>();
		for (int i = 0; i < list.Count; i++)
		{
			CostData costData = JSONHelper.Deserialize<CostData>(list[i]);
			costOfBlocks[costData.TryToGetNumericId()] = costData.cost;
		}
		for (int j = 0; j < list2.Count; j++)
		{
			CostData costData2 = JSONHelper.Deserialize<CostData>(list2[j]);
			costOfClothes[costData2.id] = costData2.cost;
		}
	}

	private void ParseIaps(List<object> iaps)
	{
	}

	private void ParseWorlds(List<object> worlds)
	{
		Manager.Get<SavedWorldManager>().ParseAndDownloadable(worlds);
	}

	private void ParseQuests(Dictionary<string, object> quests)
	{
		Manager.Get<QuestManager>().worldsQuests.OnDownloadModel(quests);
	}

	private void ParseFilters(List<object> filters)
	{
		downloadedFilters = new Dictionary<string, WorldFilter>();
		filters.ForEach(delegate(object filter)
		{
			WorldFilter worldFilter = JSONHelper.Deserialize<WorldFilter>(filter);
			if (worldFilter.isEnabled)
			{
				downloadedFilters.Add(worldFilter.id, worldFilter);
			}
		});
	}

	private void ParseTags(List<object> tags)
	{
		downloadedTags = new Dictionary<string, WorldTag>();
		tags.ForEach(delegate(object tag)
		{
			WorldTag worldTag = JSONHelper.Deserialize<WorldTag>(tag);
			downloadedTags.Add(worldTag.value, worldTag);
		});
	}
}
