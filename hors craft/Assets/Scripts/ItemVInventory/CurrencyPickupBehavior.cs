// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.CurrencyPickupBehavior
using Common.Managers;

namespace ItemVInventory
{
	public class CurrencyPickupBehavior : AbstractPickupBehaviour
	{
		private AbstractSoftCurrencyManager _softCurrency;

		protected AbstractSoftCurrencyManager softCurrency => _softCurrency ?? (_softCurrency = Manager.Get<AbstractSoftCurrencyManager>());

		public override void OnFailPickup(int amount, int level)
		{
			softCurrency.OnCurrencyAmountChange(-amount);
		}

		public override bool Pickup(int amount, int level)
		{
			softCurrency.OnCurrencyAmountChange(amount);
			return true;
		}
	}
}
