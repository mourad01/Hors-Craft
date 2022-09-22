// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftSoftCurrencyManager
using Common.Managers;
using Gameplay;
using States;
using System;
using UnityEngine;

public class CraftSoftCurrencyManager : AbstractSoftCurrencyManager
{
	protected struct MoreCurrencyInfo
	{
		public string textDescription;

		public bool hasAd;

		public string textCancel;
	}

	public bool infrequentSave;

	private const string PREFS_KEY_SC = "sc.value";

	private int softCurrencyToSave;

	private int softCurrency
	{
		get
		{
			if (infrequentSave)
			{
				return softCurrencyToSave;
			}
			return PlayerPrefs.GetInt("sc.value", 0);
		}
		set
		{
			if (infrequentSave)
			{
				softCurrencyToSave = value;
			}
			else
			{
				PlayerPrefs.SetInt("sc.value", value);
			}
		}
	}

	public override void Init()
	{
		base.Init();
		if (infrequentSave)
		{
			softCurrencyToSave = PlayerPrefs.GetInt("sc.value", 0);
		}
	}

	public override void OnCurrencyAmountChange(int changeValue)
	{
		softCurrency += changeValue;
		InformGameplay(changeValue);
	}

	public override int GetProbableCurrency()
	{
		return softCurrency;
	}

	public override int GetCurrencyAmount()
	{
		return softCurrency;
	}

	public override void SaveSoftCurrency(int value)
	{
		value = Mathf.Max(value, 0);
		softCurrency = value;
	}

	public override void InformGameplay(int changeValue)
	{
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.SOFT_CURRENCY_CHANGED, new CurrencyChangedContext
		{
			valueChanged = changeValue
		});
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
		{
			OnCurrencyAmountChange(10);
		}
	}

	public override void TryToBuySomething(int price, Action onSuccess, Action onError = null, Action onCancel = null)
	{
		if (CanIBuyThis(price))
		{
			OnCurrencyAmountChange(-price);
			onSuccess?.Invoke();
		}
		else if (onError != null)
		{
			onError();
		}
		else
		{
			GetMoreCurrency();
		}
	}

	public override bool TryToBuySomething(string key, int defaultPrice = 0)
	{
		int softPriceFor = Manager.Get<ModelManager>().currencySettings.GetSoftPriceFor(key, defaultPrice);
		if (CanIBuyThis(softPriceFor))
		{
			OnCurrencyAmountChange(-softPriceFor);
			return true;
		}
		return false;
	}

	public override void GetMoreCurrency()
	{
		int level = 1;
		if (Manager.Contains<ProgressManager>())
		{
			level = Manager.Get<ProgressManager>().level;
		}
		CurrencyModule currencySetting = Manager.Get<ModelManager>().currencySettings;
		int coins = currencySetting.SoftCurrencyPerAd(level);
		MoreCurrencyInfo currencyInfo = GetCurrencyInfo(currencySetting, coins);
		Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
		{
			numberOfAdsNeeded = 1,
			translationKey = string.Empty,
			description = currencyInfo.textDescription,
			reason = StatsManager.AdReason.XCRAFT_CURRENCY,
			immediatelyAd = false,
			type = AdsCounters.None,
			onSuccess = delegate(bool b)
			{
				if (b)
				{
					AutoRefreshingStock.DecrementStockCount("softForAd");
					Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(coins);
				}
			},
			configWatchButton = delegate(GameObject go)
			{
				ConfigureWatchButton(go, currencyInfo);
			},
			configCancelButton = delegate(GameObject go)
			{
				ConfigureCancelButton(go, currencyInfo);
			},
			updateAction = delegate(WatchXAdsPopUpStateConnector connector)
			{
				currencyInfo = GetCurrencyInfo(currencySetting, coins);
				connector.popupDescription.defaultText = currencyInfo.textDescription;
				connector.popupDescription.ForceRefresh();
				ConfigureWatchButton(connector.unlockButton.gameObject, currencyInfo);
				ConfigureCancelButton(connector.returnButton.gameObject, currencyInfo);
			}
		});
	}

	public override int GetPrice(string key, int defaultPrice = 0)
	{
		return Manager.Get<ModelManager>().currencySettings.GetSoftPriceFor(key, defaultPrice);
	}

	public override int BlockCost(ushort block)
	{
		return base.BlockCost(block);
	}

	public override int ClothesCost(string id)
	{
		int index = int.Parse(id.Split('.')[0]);
		return Manager.Get<ModelManager>().dressupSettings.GetClothesItemBasePriceValue(index);
	}

	public override void OnGameSavedInfrequent()
	{
		if (infrequentSave)
		{
			PlayerPrefs.SetInt("sc.value", softCurrencyToSave);
		}
	}

	private MoreCurrencyInfo GetCurrencyInfo(CurrencyModule currencySetting, int coins)
	{
		bool flag = AutoRefreshingStock.GetStockCount("softForAd", currencySetting.GetFreeSoftCooldown(), currencySetting.GetFreeSoftMaxCount(), 3) > 0;
		string text;
		if (flag)
		{
			text = Manager.Get<TranslationsManager>().GetText("currency.soft.watch.ad", "Watch ad to get {0} coins");
			text = text.Replace("{0}", coins.ToString());
		}
		else
		{
			text = Manager.Get<TranslationsManager>().GetText("currency.soft.no.ad", "You need wait {0:00}:{1:00}:{2:00}");
			float nextItemCooldown = AutoRefreshingStock.GetNextItemCooldown("softForAd");
			TimeSpan timeSpan = TimeSpan.FromSeconds(nextItemCooldown);
			text = string.Format(text, timeSpan.Hours.ToString(), timeSpan.Minutes.ToString(), timeSpan.Seconds.ToString());
		}
		MoreCurrencyInfo result = default(MoreCurrencyInfo);
		result.hasAd = flag;
		result.textCancel = string.Empty;
		result.textDescription = text;
		return result;
	}

	private void ConfigureWatchButton(GameObject go, MoreCurrencyInfo currencySetting)
	{
		if (!currencySetting.hasAd)
		{
			go.SetActive(value: false);
			return;
		}
		go.SetActive(value: true);
		TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
		componentInChildren.translationKey = "menu.watch";
		componentInChildren.defaultText = "Watch";
		componentInChildren.ForceRefresh();
	}

	private void ConfigureCancelButton(GameObject go, MoreCurrencyInfo currencySetting)
	{
		TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
		if (currencySetting.hasAd)
		{
			componentInChildren.translationKey = "menu.cancel";
			componentInChildren.defaultText = "cancel";
			componentInChildren.ForceRefresh();
		}
		else
		{
			componentInChildren.translationKey = "soft.no.ads.cancel";
			componentInChildren.defaultText = "OK";
			componentInChildren.ForceRefresh();
		}
	}
}
