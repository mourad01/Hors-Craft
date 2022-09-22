// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ICurrency
namespace Gameplay
{
	public interface ICurrency
	{
		bool TryToBuySomething(string key, int defaultPrice = 0);

		void OnCurrencyAmountChange(int changeValue);

		int GetCurrencyAmount();

		bool CanIBuyThis(string key, int defaultPrice = 0);

		int GetPrice(string key, int defaultPrice = 0);

		bool CanGetMoreCurrency();

		void GetMoreCurrency();
	}
}
