// DecompilerFi decompiler from Assembly-CSharp.dll class: OfferSettingsModule
using Common.Managers;
using Common.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class OfferSettingsModule : ModelModule
{
	public struct RewardConfig
	{
		public int id;

		public int count;
	}

	private List<OfferPackDefinition> offerpacks;

	private List<RewardConfig[]> newOfferpacks;

	private string keyOfferPackValidTime()
	{
		return "offerpack.timevalid";
	}

	private string keyStarterPackGender()
	{
		return "starterpack.gender";
	}

	private string keyStarterPackValidTime()
	{
		return "starterpack.timevalid";
	}

	private string keyOfferPackShowDelay()
	{
		return "offerpack.delayafterstart";
	}

	private string keyStarterPackShowDelay()
	{
		return "starterpack.delayafterstart";
	}

	private string keyOfferPackShowDelayBetween()
	{
		return "offerpack.delaybetween";
	}

	private string keyStarterPackShowDelayBetween()
	{
		return "starterpack.delaybetween";
	}

	private string keyStarterPackEnabled()
	{
		return "starterpack.enabled";
	}

	private string keyOfferPackEnabled()
	{
		return "offerpack.enabled";
	}

	private string keyOfferPackActiveStarter()
	{
		return "offerpack.active.starter";
	}

	private string keyOfferPack(int count)
	{
		return $"offerpack.starter.{count}";
	}

	private string keyDynamicNrOfStarterPacks()
	{
		return "starterpack.nrofpacks";
	}

	private string keyDynamicStarterPackDelay(int packIndex)
	{
		return $"starterpack.{packIndex}.delay";
	}

	private string keyDynamicStarterPackMaxShows(int packIndex)
	{
		return $"starterpack.{packIndex}.shows";
	}

	private string keyDynamicStarterPackAvaliabilityTime(int packIndex)
	{
		return $"starterpack.{packIndex}.avaliability";
	}

	private string keyDynamicStarterPackAward(int index)
	{
		return $"starterpack.{index}.awards";
	}

	private string keyDynamicStarterPackRewards(int index)
	{
		return $"starterpack.{index}.rewards";
	}

	private string keyDynamicStarterThisPackEnabled(int index)
	{
		return $"starterpack.{index}.enabled";
	}

	private string keyDynamicStarterPackDisableWhenAdsFree(int index)
	{
		return $"starterpack.{index}.disableWhenAdsFree";
	}

	private string keyDynamicStarterPackEnabled()
	{
		return "dynamicstarterpack.enabled";
	}

	private string keyDynamicStarterButtonImage()
	{
		return "dynamicstarterpack.buttonImage";
	}

	private string keyDynamicStarterBackgroundImage(int index)
	{
		return $"starterpack.{index}.backgroundImage";
	}

	private string keyDynamicStarterImagesUpdatingInterval()
	{
		return "dynamicstarterpack.images.updating.interval.in.days";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyStarterPackGender(), 0);
		descriptions.AddDescription(keyOfferPackValidTime(), 86400);
		descriptions.AddDescription(keyStarterPackValidTime(), 86400);
		descriptions.AddDescription(keyStarterPackShowDelay(), 60);
		descriptions.AddDescription(keyOfferPackShowDelay(), 60);
		descriptions.AddDescription(keyStarterPackShowDelayBetween(), 60);
		descriptions.AddDescription(keyOfferPackShowDelayBetween(), 60);
		descriptions.AddDescription(keyStarterPackEnabled(), defaultValue: true);
		descriptions.AddDescription(keyOfferPackEnabled(), defaultValue: true);
		descriptions.AddDescription(keyOfferPackActiveStarter(), 0);
		descriptions.AddDescription(keyDynamicStarterPackEnabled(), defaultValue: false);
		descriptions.AddDescription(keyDynamicNrOfStarterPacks(), 0);
		descriptions.AddDescription(keyDynamicStarterButtonImage(), string.Empty);
		descriptions.AddDescription(keyDynamicStarterImagesUpdatingInterval(), 2f);
		for (int i = 0; i < 15; i++)
		{
			descriptions.AddDescription(keyDynamicStarterPackDelay(i), 86400);
			descriptions.AddDescription(keyDynamicStarterPackAvaliabilityTime(i), 28800);
			descriptions.AddDescription(keyDynamicStarterPackAward(i), string.Empty);
			descriptions.AddDescription(keyDynamicStarterPackRewards(i), string.Empty);
			descriptions.AddDescription(keyDynamicStarterBackgroundImage(i), string.Empty);
			descriptions.AddDescription(keyDynamicStarterPackMaxShows(i), 1);
			descriptions.AddDescription(keyDynamicStarterThisPackEnabled(i), 1);
			descriptions.AddDescription(keyDynamicStarterPackDisableWhenAdsFree(i), 0);
		}
	}

	public override void OnModelDownloaded()
	{
		offerpacks = new List<OfferPackDefinition>();
		int num = 0;
		while (true)
		{
			string @string = base.settings.GetString(keyOfferPack(num), null);
			if (string.IsNullOrEmpty(@string))
			{
				break;
			}
			offerpacks.Add(new OfferPackDefinition(@string));
			num++;
		}
		if (GetNrOfDynamicStarterPacks() > 0)
		{
			offerpacks = new List<OfferPackDefinition>();
			for (int i = 0; i < GetNrOfDynamicStarterPacks(); i++)
			{
				string string2 = base.settings.GetString(keyDynamicStarterPackAward(i), null);
				if (!string.IsNullOrEmpty(string2))
				{
					offerpacks.Add(new OfferPackDefinition(string2));
				}
			}
		}
		ParseOfferpacksNewRewards();
		DownloadOfferpackImages();
	}

	public List<OfferPackDefinition> GetOfferPacks()
	{
		return offerpacks;
	}

	public OfferPackDefinition GetActiveStarterPack(int index = -1)
	{
		List<OfferPackDefinition> offerPacks = GetOfferPacks();
		int num = base.settings.GetInt(keyOfferPackActiveStarter());
		if (index != -1)
		{
			num = index;
		}
		if (offerPacks == null)
		{
			return null;
		}
		if (num >= offerPacks.Count)
		{
			return null;
		}
		return offerpacks[num];
	}

	public bool AreOfferPacksEnabled()
	{
		return base.settings.GetBool(keyOfferPackEnabled(), defaultValue: true);
	}

	public bool AreDynamicStarterPacksEnabled()
	{
		return base.settings.GetBool(keyDynamicStarterPackEnabled(), defaultValue: false);
	}

	public bool AreStarterPacksEnabled()
	{
		return base.settings.GetBool(keyStarterPackEnabled(), defaultValue: true);
	}

	public int GetOfferPackValidTime()
	{
		return base.settings.GetInt(keyOfferPackValidTime(), 86400);
	}

	public int GetStarterPackValidTime()
	{
		return base.settings.GetInt(keyStarterPackValidTime(), 86400);
	}

	public int GetOfferPackShowDelay()
	{
		return base.settings.GetInt(keyOfferPackShowDelay(), 2);
	}

	public int GetStarterPackShowDelay()
	{
		return base.settings.GetInt(keyStarterPackShowDelay(), 2);
	}

	public int GetOfferPackShowDelayBetween()
	{
		return base.settings.GetInt(keyOfferPackShowDelayBetween(), 5);
	}

	public int GetStarterPackShowDelayBetween()
	{
		return base.settings.GetInt(keyStarterPackShowDelayBetween(), 5);
	}

	public Skin.Gender GetStarterGender()
	{
		return (Skin.Gender)base.settings.GetInt(keyStarterPackGender(), 0);
	}

	public int GetNrOfDynamicStarterPacks()
	{
		return base.settings.GetInt(keyDynamicNrOfStarterPacks(), 0);
	}

	public int GetDynamicStarterPackDelay(int index)
	{
		return base.settings.GetInt(keyDynamicStarterPackDelay(index), 60);
	}

	public int GetDynamicStarterPackMaxShowsRaw(int index)
	{
		return base.settings.GetInt(keyDynamicStarterPackMaxShows(index), 1);
	}

	public int GetDynamicStarterPackMaxShows(int index)
	{
		int @int = base.settings.GetInt(keyDynamicStarterPackMaxShows(index), 1);
		return (@int < 0) ? int.MaxValue : @int;
	}

	public int GetDynamicStarterPackAvaliabilityTime(int index)
	{
		return base.settings.GetInt(keyDynamicStarterPackAvaliabilityTime(index), 60);
	}

	public string GetDynamicStarterBackgroundImage(int index)
	{
		return base.settings.GetString(keyDynamicStarterBackgroundImage(index), string.Empty);
	}

	public string GetDynamicStarterButtonImage()
	{
		return base.settings.GetString(keyDynamicStarterButtonImage(), string.Empty);
	}

	private void DownloadOfferpackImages()
	{
		if (!AreDynamicStarterPacksEnabled() || !Manager.Contains<DynamicOfferPackManager>())
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add(GetDynamicStarterButtonImage());
		if (GetNrOfDynamicStarterPacks() > 0)
		{
			for (int i = 0; i < GetNrOfDynamicStarterPacks(); i++)
			{
				list.Add(GetDynamicStarterBackgroundImage(i));
			}
		}
		Manager.Get<DynamicOfferPackManager>().DownloadOfferpacksImages(list.ToArray());
	}

	public bool GetDynamicStarterPackEnabled(int index)
	{
		return (base.settings.GetInt(keyDynamicStarterThisPackEnabled(index), 1) != 0) ? true : false;
	}

	public bool GetDynamicStarterDisableWhenAdsFree(int index)
	{
		return (base.settings.GetInt(keyDynamicStarterPackDisableWhenAdsFree(index), 0) != 0) ? true : false;
	}

	public double GetImagesUpdatingIntervalInDays()
	{
		return base.settings.GetFloat(keyDynamicStarterImagesUpdatingInterval());
	}

	public RewardConfig[] GetActiveStarterPackWithNewRewards(int index = -1)
	{
		int num = base.settings.GetInt(keyOfferPackActiveStarter());
		if (index != -1)
		{
			num = index;
		}
		if (newOfferpacks == null)
		{
			return null;
		}
		if (num >= newOfferpacks.Count)
		{
			return null;
		}
		return newOfferpacks[num];
	}

	private string GetOfferpackReward(int index)
	{
		return base.settings.GetString(keyDynamicStarterPackRewards(index), null);
	}

	private void ParseOfferpacksNewRewards()
	{
		if (GetNrOfDynamicStarterPacks() <= 0)
		{
			return;
		}
		newOfferpacks = new List<RewardConfig[]>();
		for (int i = 0; i < GetNrOfDynamicStarterPacks(); i++)
		{
			string offerpackReward = GetOfferpackReward(i);
			if (!string.IsNullOrEmpty(offerpackReward))
			{
				TryParseAndAddNewRewards(offerpackReward);
			}
		}
	}

	private void TryParseAndAddNewRewards(string rewardString)
	{
		rewardString = RemoveWhitespace(rewardString);
		List<RewardConfig> list = new List<RewardConfig>();
		string[] array = rewardString.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(':');
			if (array3.Length == 2 && int.TryParse(array3[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int result) && int.TryParse(array3[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int result2))
			{
				list.Add(new RewardConfig
				{
					id = result,
					count = result2
				});
			}
		}
		newOfferpacks.Add(list.ToArray());
	}

	private string RemoveWhitespace(string input)
	{
		return new string((from c in input.ToCharArray()
			where !char.IsWhiteSpace(c)
			select c).ToArray());
	}
}
