// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CraftSoftCurrency
using Common.Managers;
using System;

namespace Gameplay
{
	public class CraftSoftCurrency : CurrencyScriptableObject
	{
		public override bool TryToBuySomething(string id, int defaultPrice = 0)
		{
			return Manager.Get<CraftSoftCurrencyManager>().TryToBuySomething(id, defaultPrice);
		}

		public override void TryToBuySomething(int price, Action onSuccess)
		{
			Manager.Get<CraftSoftCurrencyManager>().TryToBuySomething(price, onSuccess);
		}

		public override void OnCurrencyAmountChange(int changeValue)
		{
			Manager.Get<CraftSoftCurrencyManager>().OnCurrencyAmountChange(changeValue);
		}

		public override int GetCurrencyAmount()
		{
			return Manager.Get<CraftSoftCurrencyManager>().GetCurrencyAmount();
		}

		public override bool CanIBuyThis(string id, int defaultPrice = 0)
		{
			return Manager.Get<CraftSoftCurrencyManager>().CanIBuyThis(id, defaultPrice);
		}

		public override int GetPrice(string id, int defaultPrice = 0)
		{
			return Manager.Get<CraftSoftCurrencyManager>().GetPrice(id, defaultPrice);
		}

		public override bool CanGetMoreCurrency()
		{
			return true;
		}

		public override void GetMoreCurrency()
		{
			Manager.Get<CraftSoftCurrencyManager>().GetMoreCurrency();
		}
	}
}
