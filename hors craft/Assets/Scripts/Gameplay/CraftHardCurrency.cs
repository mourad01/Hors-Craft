// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CraftHardCurrency
using Common.Managers;
using System;

namespace Gameplay
{
	public class CraftHardCurrency : CurrencyScriptableObject
	{
		public override bool TryToBuySomething(string id, int defaultPrice = 0)
		{
			return Manager.Get<HardCurrencyManager>().TryToBuySomething(id, defaultPrice);
		}

		public override void TryToBuySomething(int price, Action onSuccess)
		{
			Manager.Get<HardCurrencyManager>().TryToBuySomething(price, onSuccess);
		}

		public override void OnCurrencyAmountChange(int changeValue)
		{
			Manager.Get<HardCurrencyManager>().OnCurrencyAmountChange(changeValue);
		}

		public override int GetCurrencyAmount()
		{
			return Manager.Get<HardCurrencyManager>().GetCurrencyAmount();
		}

		public override bool CanIBuyThis(string id, int defaultPrice = 0)
		{
			return Manager.Get<HardCurrencyManager>().CanIBuyThis(id, defaultPrice);
		}

		public override int GetPrice(string id, int defaultPrice = 0)
		{
			return Manager.Get<HardCurrencyManager>().GetPrice(id, defaultPrice);
		}

		public override bool CanGetMoreCurrency()
		{
			return Manager.Get<HardCurrencyManager>().CanGetMoreCurrency();
		}

		public override void GetMoreCurrency()
		{
			Manager.Get<HardCurrencyManager>().GetMoreCurrency();
		}
	}
}
