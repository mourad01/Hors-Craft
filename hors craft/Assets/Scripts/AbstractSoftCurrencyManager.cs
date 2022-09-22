// DecompilerFi decompiler from Assembly-CSharp.dll class: AbstractSoftCurrencyManager
using Common.Managers;
using Gameplay;
using System;
using UnityEngine.Purchasing;

public abstract class AbstractSoftCurrencyManager : Manager, IGameCallbacksListener, ICurrency
{
	public virtual void OnCurrencyAmountChange(int changeValue)
	{
	}

	public virtual void OnModelDownloaded(int value)
	{
	}

	public override void Init()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	public virtual void InitializeAtModelDownloaded()
	{
	}

	/*public virtual void OnIAPBought(Product product)
	{
	}*/

	public virtual void OnIAPBought(string id)
	{
	}

	public virtual void InformGameplay(int changeValue)
	{
	}

	public virtual void TryToBuySomething(int price, Action onSuccess, Action onError = null, Action onCancel = null)
	{
	}

	public virtual bool TryToBuySomething(string key, int defaultPrice = 0)
	{
		return true;
	}

	private void OnInformSuccess(string result, int changeId)
	{
	}

	public virtual int GetProbableCurrency()
	{
		return 0;
	}

	public virtual void SaveSoftCurrency(int value)
	{
	}

	public virtual int GetCurrencyAmount()
	{
		return 0;
	}

	public virtual int BlockCost(ushort block)
	{
		return 0;
	}

	public virtual int ClothesCost(string id)
	{
		return 0;
	}

	public virtual bool CanIBuyThis(int cost)
	{
		return GetCurrencyAmount() >= cost;
	}

	public virtual bool CanIBuyThis(string key, int defaultPrice)
	{
		return GetCurrencyAmount() >= defaultPrice;
	}

	public virtual int GetPrice(string key, int defaultPrice = 0)
	{
		return defaultPrice;
	}

	public virtual bool CanGetMoreCurrency()
	{
		return true;
	}

	public virtual void GetMoreCurrency()
	{
	}

	public virtual void OnGameSavedFrequent()
	{
	}

	public virtual void OnGameSavedInfrequent()
	{
	}

	public virtual void OnGameplayStarted()
	{
	}

	public virtual void OnGameplayRestarted()
	{
	}
}
