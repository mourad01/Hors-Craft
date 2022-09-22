// DecompilerFi decompiler from Assembly-CSharp.dll class: OfferPackManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using UnityEngine;

public class OfferPackManager : Manager
{
	private const string TIME_STARTER_LAST_TIME_SHOWN = "starterpack.lasttimeshown";

	private const string TIME_OFFER_LAST_TIME_SHOWN = "offerpack.lasttimeshown";

	private const string TIME_STARTER_ENDED_TIME = "starterpack.endedtime";

	private const string TIME_STARTER_ENDED = "starterpack.ended";

	private const string TIME_STARTER_PACK_FIRST_SHOW = "starterpack.firstshow";

	private const string TIME_OFFER_PACK = "offerpack.timecreated";

	private const string CURRENT_OFFER_PACK = "offerpack.current";

	private bool showInGamePlay = true;

	private bool showInPauseState;

	private OfferPackDefinition currentOfferPack;

	private OfferPackDefinition[] starterPacks;

	private bool _IsOfferValid;

	public bool CanShowInGamePlayState => showInGamePlay;

	public bool CanShowInPauseState => showInPauseState;

	private float offerLastShownTime
	{
		get
		{
			return PlayerPrefs.GetFloat("offerpack.lasttimeshown", 0f);
		}
		set
		{
			PlayerPrefs.SetFloat("offerpack.lasttimeshown", value);
		}
	}

	private float starterLastShownTime
	{
		get
		{
			return PlayerPrefs.GetFloat("starterpack.lasttimeshown", 0f);
		}
		set
		{
			PlayerPrefs.SetFloat("starterpack.lasttimeshown", value);
		}
	}

	private float offerCreationTime
	{
		get
		{
			return PlayerPrefs.GetFloat("offerpack.timecreated", 0f);
		}
		set
		{
			PlayerPrefs.SetFloat("offerpack.timecreated", value);
		}
	}

	private OfferPackDefinition currentOfferPackSaved
	{
		get
		{
			string @string = PlayerPrefs.GetString("offerpack.current", string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				return null;
			}
			return new OfferPackDefinition(@string);
		}
		set
		{
			PlayerPrefs.SetString("offerpack.current", value.ToParsable());
		}
	}

	private float starterFirstShow
	{
		get
		{
			return PlayerPrefs.GetFloat("starterpack.firstshow", 0f);
		}
		set
		{
			PlayerPrefs.SetFloat("starterpack.firstshow", value);
		}
	}

	private bool starterEnded
	{
		get
		{
			return PlayerPrefs.GetInt("starterpack.ended", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("starterpack.ended", value ? 1 : 0);
		}
	}

	private float starterEndedTime
	{
		get
		{
			return PlayerPrefs.GetFloat("starterpack.endedtime");
		}
		set
		{
			PlayerPrefs.SetFloat("starterpack.endedtime", value);
		}
	}

	public bool IsOfferValid
	{
		get
		{
			return _IsOfferValid;
		}
		private set
		{
			_IsOfferValid = value;
		}
	}

	public bool starterEnabled => settings.AreStarterPacksEnabled();

	public bool offerEnabled => settings.AreOfferPacksEnabled();

	protected double timestamp => Misc.GetTimeStampDouble();

	protected OfferSettingsModule settings => Manager.Get<ModelManager>().offerPackSettings;

	public override void Init()
	{
		IsOfferValid = true;
		AddProductsToManager();
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.OFFERPACK_ENABLED);
	}

	private void AddProductsToManager()
	{
	}

	public bool IsOfferToShow()
	{
		return true;
	}

	public void OnOfferPackShown()
	{
		offerLastShownTime = (float)timestamp;
	}

	public virtual void OnStarterShow()
	{
		if (starterFirstShow == 0f)
		{
			starterFirstShow = (float)timestamp;
		}
		starterLastShownTime = (float)timestamp;
	}

	private bool CheckStarterEnd()
	{
		if (starterLastShownTime == 0f)
		{
			return false;
		}
		if (!starterEnded)
		{
			if (!(CalculateTimeToEndOfStarterPack() <= 0.0))
			{
				return false;
			}
			starterEnded = true;
		}
		return true;
	}

	private bool DoesPlayerHaveThisWrold(OfferPackDefinition offer)
	{
		if (offer == null)
		{
			return false;
		}
		WorldItemData worldItemData = offer.TryToGet<WorldItemData>();
		if (worldItemData != null)
		{
			return !Singleton<PlayerData>.get.playerWorlds.IsWorldBought(worldItemData.uniqueItemId);
		}
		return true;
	}

	public virtual void OnWorldBought(string worldId)
	{
		if (!starterEnded)
		{
			OfferPackDefinition starterPack = GetStarterPack();
			if (starterPack != null)
			{
				WorldItemData worldItemData = starterPack.TryToGet<WorldItemData>();
				if (worldItemData.uniqueItemId == worldId)
				{
					starterEnded = true;
					starterEndedTime = (float)timestamp;
				}
			}
			return;
		}
		OfferPackDefinition currentOffer = GetCurrentOffer();
		if (currentOffer != null)
		{
			WorldItemData worldItemData2 = currentOffer.TryToGet<WorldItemData>();
			if (worldItemData2.uniqueItemId == worldId)
			{
				currentOfferPack = null;
				GetCurrentOffer();
			}
		}
	}

	public virtual bool ShouldShowStarterPack(int timeInGameplay)
	{
		if (!starterEnabled || starterEnded)
		{
			return false;
		}
		if (starterFirstShow == 0f)
		{
			bool flag = timeInGameplay > settings.GetStarterPackShowDelay();
			if (flag)
			{
				OnStarterShow();
			}
			return flag;
		}
		float num = settings.GetStarterPackShowDelayBetween();
		bool flag2 = (double)(starterLastShownTime + num) < timestamp;
		if (flag2)
		{
			OnStarterShow();
		}
		return flag2;
	}

	public bool IsStarterOfferActive()
	{
		return !starterEnded;
	}

	public virtual bool ShouldShowOfferPack(int timeInGameplay)
	{
		if (!offerEnabled || !offerEnabled || !IsOfferValid)
		{
			return false;
		}
		if (!CheckStarterEnd())
		{
			return false;
		}
		if (offerLastShownTime == 0f)
		{
			bool flag = (double)timeInGameplay > (double)((float)settings.GetOfferPackShowDelay() + starterEndedTime) - timestamp;
			if (flag)
			{
				OnOfferPackShown();
			}
			return flag;
		}
		bool flag2 = (double)(offerLastShownTime + (float)settings.GetOfferPackShowDelayBetween()) < timestamp;
		if (flag2)
		{
			OnOfferPackShown();
		}
		return flag2;
	}

	private bool ShouldGenerateNewOffer(OfferPackDefinition currentOffer)
	{
		int offerPackValidTime = Manager.Get<ModelManager>().offerPackSettings.GetOfferPackValidTime();
		return Misc.GetTimeStampDouble() > (double)(offerCreationTime + (float)offerPackValidTime);
	}

	public virtual OfferPackDefinition GetStarterPack()
	{
		return settings.GetActiveStarterPack();
	}

	public Skin.Gender GetStarterPackGender()
	{
		return settings.GetStarterGender();
	}

	public OfferPackDefinition GetCurrentOffer()
	{
		if (!IsOfferValid)
		{
			return null;
		}
		if (currentOfferPack == null)
		{
			currentOfferPack = LoadOfferPack();
		}
		if (currentOfferPack == null || ShouldGenerateNewOffer(currentOfferPack))
		{
			currentOfferPack = GenerateOfferPack();
			offerCreationTime = (float)timestamp;
			IsOfferValid = (currentOfferPack != null);
			currentOfferPackSaved = currentOfferPack;
		}
		return currentOfferPack;
	}

	private OfferPackDefinition LoadOfferPack()
	{
		return currentOfferPackSaved;
	}

	public OfferPackDefinition GenerateOfferPack()
	{
		OfferPackDefinition offerPackDefinition = new OfferPackDefinition();
		offerPackDefinition.packItems.Add(PackFactory.GetData(PackItemType.WorldItem));
		offerPackDefinition.packItems.Add(PackFactory.GetData(PackItemType.Clothes));
		offerPackDefinition.packItems.Add(PackFactory.GetData(PackItemType.Blocks));
		offerPackDefinition.packItems.Add(PackFactory.GetData(PackItemType.SoftCurrency));
		offerPackDefinition.FillWithPseudoRandom();
		if (!offerPackDefinition.IsValid())
		{
		}
		return offerPackDefinition;
	}

	public virtual void GrantPack(OfferPackDefinition offerPack)
	{
		offerPack.packItems.ForEach(delegate(PackItemData pack)
		{
			pack.GrantItem();
		});
	}

	public virtual void GrantCurrentPack()
	{
		if (currentOfferPack != null)
		{
			currentOfferPack.packItems.ForEach(delegate(PackItemData pack)
			{
				pack.GrantItem();
			});
			currentOfferPack = null;
			GetCurrentOffer();
		}
	}

	public void OnStarterPackBuy()
	{
		starterEnded = true;
		starterEndedTime = (float)timestamp;
	}

	public double CalculateTimeToEndOfStarterPack()
	{
		double num = (double)(starterFirstShow + (float)settings.GetStarterPackValidTime()) - timestamp;
		return (!(num < 0.0)) ? num : 0.0;
	}

	public double CalculateTimeToEndOfOfferPack()
	{
		if (!IsOfferValid)
		{
			return 0.0;
		}
		double num = (double)(offerCreationTime + (float)settings.GetOfferPackValidTime()) - timestamp;
		if (num <= 0.0)
		{
			GetCurrentOffer();
		}
		return num;
	}

	private string FormatTime(double seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (seconds <= 0.0)
		{
			return string.Empty;
		}
		return $"{timeSpan:hh\\:mm}";
	}

	public string GetTimeToEndOfferPack()
	{
		if (offerEnabled)
		{
			return string.Empty;
		}
		return FormatTime(CalculateTimeToEndOfOfferPack());
	}

	public string GetTimeToEndStarterPack()
	{
		if (starterEnabled)
		{
			return string.Empty;
		}
		return FormatTime(CalculateTimeToEndOfStarterPack());
	}

	public virtual void OnStarterPackWillShow()
	{
	}

	public virtual void OnStarterPackWasShown()
	{
	}

	public virtual bool ShouldShowPackButton()
	{
		return false;
	}
}
