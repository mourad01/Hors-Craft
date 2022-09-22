// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.HardCurrencyManager
using Common.Managers;
using States;
using System;
using UnityEngine;

namespace Gameplay
{
	public class HardCurrencyManager : Manager, IGameCallbacksListener, ICurrency
	{
		public bool infrequentSave;

		private const string PREFS_KEY_HC = "hc.value";

		private HardForAdContext hardForAdContext;

		private LootModule lootSettings;

		private int hardCurrencyToSave;

		private int hardCurrency
		{
			get
			{
				if (infrequentSave)
				{
					return hardCurrencyToSave;
				}
				return PlayerPrefs.GetInt("hc.value", 0);
			}
			set
			{
				if (infrequentSave)
				{
					hardCurrencyToSave = value;
				}
				else
				{
					PlayerPrefs.SetInt("hc.value", value);
				}
			}
		}

		public override void Init()
		{
			lootSettings = Manager.Get<ModelManager>().lootSettings;
			if (infrequentSave)
			{
				hardCurrencyToSave = PlayerPrefs.GetInt("hc.value", 0);
			}
		}

		public void GetMoreCurrency()
		{
			GetMoreCurrencyForAd();
		}

		public void GetMoreCurrencyForAd(bool decrementStock = false)
		{
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				description = "Watch an ad to get {0} coins",
				translationKey = "menu.get.coins",
				numberOfAdsNeeded = Manager.Get<ModelManager>().currencySettings.GetHardCurrencyPerAd(),
				type = AdsCounters.None,
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.defaultText = "Get Coins";
					componentInChildren.translationKey = "menu.get.coins.confirm";
					componentInChildren.ForceRefresh();
				},
				onSuccess = delegate
				{
					int hardCurrencyPerAd = Manager.Get<ModelManager>().currencySettings.GetHardCurrencyPerAd();
					if (hardCurrencyPerAd != 0)
					{
						OnCurrencyAmountChange(hardCurrencyPerAd);
					}
					if (decrementStock)
					{
						AutoRefreshingStock.DecrementStockCount("hardForAd");
					}
				},
				reason = StatsManager.AdReason.XCRAFT_CURRENCY
			});
		}

		public void OnCurrencyAmountChange(int changeValue)
		{
			hardCurrency += changeValue;
			InformGameplay(changeValue);
		}

		public int GetCurrencyAmount()
		{
			return hardCurrency;
		}

		public void SaveHardCurrency(int value)
		{
			value = Mathf.Max(value, 0);
			hardCurrency = value;
			OnGameSavedInfrequent();
		}

		public void InformGameplay(int changeValue)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.HARD_CURRENCY_CHANGED, new CurrencyChangedContext
			{
				valueChanged = changeValue
			});
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
			{
				OnCurrencyAmountChange(10);
			}
			if (CanGetMoreCurrencyForAd())
			{
				if (hardForAdContext == null)
				{
					hardForAdContext = new HardForAdContext
					{
						onHardForAdButton = delegate
						{
							GetMoreCurrencyForAd(decrementStock: true);
						}
					};
				}
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.HARD_FOR_AD_ENABLED, hardForAdContext);
			}
			else
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.HARD_FOR_AD_ENABLED, hardForAdContext);
			}
		}

		public void TryToBuySomething(int price, Action onSuccess, Action onError = null, Action onCancel = null)
		{
			if (CanIBuyThis(price))
			{
				OnCurrencyAmountChange(-price);
				onSuccess?.Invoke();
			}
			else
			{
				onError?.Invoke();
			}
		}

		public bool TryToBuySomething(string key, int defaultPrice = 0)
		{
			int hardPriceFor = Manager.Get<ModelManager>().currencySettings.GetHardPriceFor(key, defaultPrice);
			if (CanIBuyThis(hardPriceFor))
			{
				OnCurrencyAmountChange(-hardPriceFor);
				return true;
			}
			return false;
		}

		public bool CanIBuyThis(int cost)
		{
			return GetCurrencyAmount() >= cost;
		}

		public bool CanIBuyThis(string key, int defaultPrice = 0)
		{
			return GetCurrencyAmount() >= GetPrice(key, defaultPrice);
		}

		public int GetPrice(string key, int defaultPrice = 0)
		{
			return Manager.Get<ModelManager>().currencySettings.GetHardPriceFor(key, defaultPrice);
		}

		public bool CanGetMoreCurrency()
		{
			return true;
		}

		public bool CanGetMoreCurrencyForAd()
		{
			return false;
		}

		public void OnGameSavedInfrequent()
		{
			if (infrequentSave)
			{
				PlayerPrefs.SetInt("hc.value", hardCurrencyToSave);
			}
		}

		public virtual void OnGameSavedFrequent()
		{
		}

		public virtual void OnGameplayStarted()
		{
		}

		public virtual void OnGameplayRestarted()
		{
		}
	}
}
