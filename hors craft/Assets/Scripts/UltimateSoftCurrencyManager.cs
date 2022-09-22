// DecompilerFi decompiler from Assembly-CSharp.dll class: UltimateSoftCurrencyManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using System.Globalization;
using TsgCommon;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;

public class UltimateSoftCurrencyManager : AbstractSoftCurrencyManager
{
	[Serializable]
	public class CurrencyDiff
	{
		public int id;

		public int change;

		public double timestamp;

		public CurrencyDiff()
		{
		}

		public CurrencyDiff(int change)
		{
			this.change = change;
			timestamp = Misc.GetTimeStampDouble();
			id = PlayerPrefs.GetInt("diff_id_generator", 0);
			PlayerPrefs.SetInt("diff_id_generator", id + 1);
		}
	}

	public const string QUEUE_CURRENCY_KEY = "queue_currency_diffs";

	public const string DIFF_ID_GENERATOR = "diff_id_generator";

	public const string WAS_FIRST_DOWNLOAD = "first_download";

	private List<CurrencyDiff> _queueOfPurchases;

	private const string PREFS_KEY_SC = "sc.value";

	private int propableCurrency;

	public List<CurrencyDiff> queueOfPurchases
	{
		get
		{
			if (_queueOfPurchases == null)
			{
				string @string = PlayerPrefs.GetString("queue_currency_diffs", string.Empty);
				if (string.IsNullOrEmpty(@string))
				{
					_queueOfPurchases = new List<CurrencyDiff>();
				}
				else
				{
					_queueOfPurchases = JSONHelper.Deserialize<List<CurrencyDiff>>(@string);
				}
			}
			return _queueOfPurchases;
		}
	}

	public override void Init()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	public override void InitializeAtModelDownloaded()
	{
		InitializePlayerCurrency();
	}

	private void InitializePlayerCurrency()
	{
		ProcessInitialQueue(PlayerId.GetId(), delegate(int value)
		{
			_queueOfPurchases = new List<CurrencyDiff>();
			SaveQueueToPrefs();
			UnityEngine.Debug.Log($"Player has {value} sc");
			SaveSoftCurrency(value);
		}, Manager.Get<ModelManager>().worldsSettings.GetWorldsStartCurrency());
	}

	/*public override void OnIAPBought(Product product)
	{
		int changeValue = Singleton<UltimateCraftModelDownloader>.get.FindCurrencySizeOfPackage(product.definition.id);
		if (Manager.Get<ModelManager>().adsFree.GetRemoveAdsAfterPayment())
		{
			RemoveAds();
		}
		OnCurrencyAmountChange(changeValue);
	}*/

	private void RemoveAds()
	{
		TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.SetNoAds(newState: true);
	}

	public override void OnIAPBought(string id)
	{
		int changeValue = Singleton<UltimateCraftModelDownloader>.get.FindCurrencySizeOfPackage(id);
		OnCurrencyAmountChange(changeValue);
	}

	public override void OnModelDownloaded(int value)
	{
		if (PlayerPrefs.GetInt("first_download", 0) == 0)
		{
			PlayerPrefs.SetInt("first_download", 1);
		}
	}

	public override void OnCurrencyAmountChange(int changeValue)
	{
		UnityEngine.Debug.Log($"Cash changed for: {changeValue}");
		InformServerAboutChange(changeValue);
		InformGameplay(changeValue);
	}

	public override void InformGameplay(int changeValue)
	{
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.SOFT_CURRENCY_CHANGED, new CurrencyChangedContext
		{
			valueChanged = changeValue
		});
	}

	public override bool CanIBuyThis(int cost)
	{
		return GetCurrencyAmount() >= cost;
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
			UniversalNoMoneyResponse(price, onCancel);
		}
	}

	public void UniversalNoMoneyResponse(int moneyNeeded, Action onReturn)
	{
		Manager.Get<StateMachineManager>().PushState<CurrencyShopState>(new CurrencyShopParameter(noMoneyResponse: true, onReturn));
	}

	private void InformServerAboutChange(int change)
	{
		int diffIndex = AddToQueue(change);
		propableCurrency += change;
		string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
		SimpleRequestMaker.MakeRequest(homeURL, "SoftCurrency/CraftChangeCurrency", new Dictionary<string, object>
		{
			{
				"gameName",
				"xcraft.ultimate"
			},
			{
				"deviceId",
				PlayerId.GetId()
			},
			{
				"change",
				change
			}
		}, delegate(WWW result)
		{
			OnInformSuccess(result.text, diffIndex);
		}, OnError);
	}

	private int AddToQueue(int cost)
	{
		CurrencyDiff currencyDiff = new CurrencyDiff(cost);
		queueOfPurchases.Add(currencyDiff);
		SaveQueueToPrefs();
		return currencyDiff.id;
	}

	private void RemoveFromQueue(int id)
	{
		int num = queueOfPurchases.FindIndex((CurrencyDiff currencyDiff) => currencyDiff.id == id);
		if (num >= 0)
		{
			queueOfPurchases.RemoveAt(num);
		}
		SaveQueueToPrefs();
	}

	private void SaveQueueToPrefs()
	{
		PlayerPrefs.SetString("queue_currency_diffs", QueueToJson());
	}

	private string QueueToJson()
	{
		return JSONHelper.ToJSON(queueOfPurchases);
	}

	public void ProcessInitialQueue(string playerId, Action<int> intConsumer, int startingValue)
	{
		string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
		SimpleRequestMaker.MakePost(homeURL, "SoftCurrency/CraftChangeCurrencyDiffArray", new Dictionary<string, object>
		{
			{
				"gameName",
				"xcraft.ultimate"
			},
			{
				"deviceId",
				playerId
			},
			{
				"value",
				startingValue
			}
		}, QueueToJson(), delegate(UnityWebRequest result)
		{
			int result2 = 0;
			string s = result.downloadHandler.text.Trim('"');
			if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result2))
			{
				intConsumer(result2);
			}
			else
			{
				OnError("Cannot parse result");
			}
		}, OnError);
	}

	public void SetSoftCurrencyOnServer(string playerId, int value)
	{
		string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
		UnityEngine.Debug.LogError(value);
		SimpleRequestMaker.MakeRequest(homeURL, "SoftCurrency/CraftSetNewCurrency", new Dictionary<string, object>
		{
			{
				"gameName",
				"xcraft.ultimate"
			},
			{
				"deviceId",
				playerId
			},
			{
				"value",
				value
			}
		}, delegate(WWW result)
		{
			UnityEngine.Debug.LogError("Setting value was:" + result);
		}, OnError);
	}

	public void GetSoftCurrencyFromServer(string playerId, Action<int> intConsumer)
	{
		string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
		SimpleRequestMaker.MakeRequest(homeURL, "SoftCurrency/GetCraftCurrency", new Dictionary<string, object>
		{
			{
				"gameName",
				"xcraft.ultimate"
			},
			{
				"deviceId",
				playerId
			}
		}, delegate(WWW result)
		{
			int result2 = 0;
			string s = result.text.Trim('"');
			if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result2))
			{
				intConsumer(result2);
			}
			else
			{
				OnError("Cannot parse result");
			}
		}, OnError);
	}

	private void OnError(string error)
	{
		UnityEngine.Debug.LogError("Error in SC: " + error);
	}

	private void OnInformSuccess(string result, int changeId)
	{
		string s = result.Trim('"');
		int result2 = 0;
		if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result2))
		{
			SaveSoftCurrency(result2);
		}
		RemoveFromQueue(changeId);
	}

	public override int GetProbableCurrency()
	{
		return propableCurrency;
	}

	public override void SaveSoftCurrency(int value)
	{
		value = Mathf.Max(value, 0);
		propableCurrency = value;
		PlayerPrefs.SetInt("sc.value", value);
	}

	public override int GetCurrencyAmount()
	{
		return propableCurrency;
	}

	public override void OnGameSavedFrequent()
	{
	}

	public override void OnGameSavedInfrequent()
	{
	}

	public override void OnGameplayStarted()
	{
		OnModelDownloaded(Manager.Get<ModelManager>().worldsSettings.GetWorldsStartCurrency());
	}

	public override void OnGameplayRestarted()
	{
	}

	public override int BlockCost(ushort block)
	{
		return Singleton<UltimateCraftModelDownloader>.get.GetPriceOfBlock(block);
	}

	public override int ClothesCost(string id)
	{
		return Singleton<UltimateCraftModelDownloader>.get.GetPriceOfClothes(id);
	}
}
