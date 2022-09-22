// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CurrencyScriptableObject
using System;
using UnityEngine;

namespace Gameplay
{
	public abstract class CurrencyScriptableObject : ScriptableObject, ICurrency
	{
		public abstract bool TryToBuySomething(string id, int defaultPrice = 0);

		public abstract void TryToBuySomething(int price, Action onSuccess);

		public abstract void OnCurrencyAmountChange(int changeValue);

		public abstract int GetCurrencyAmount();

		public abstract bool CanIBuyThis(string id, int defaultPrice = 0);

		public abstract int GetPrice(string id, int defaultPrice = 0);

		public virtual bool CanGetMoreCurrency()
		{
			return false;
		}

		public virtual void GetMoreCurrency()
		{
		}
	}
}
