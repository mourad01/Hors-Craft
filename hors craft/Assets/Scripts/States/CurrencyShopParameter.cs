// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CurrencyShopParameter
using Common.Managers.States;
using System;

namespace States
{
	public class CurrencyShopParameter : StartParameter
	{
		public bool noMoneyResponse;

		public Action onCancel;

		public CurrencyShopParameter(bool noMoneyResponse, Action onCancel = null)
		{
			this.noMoneyResponse = noMoneyResponse;
			this.onCancel = onCancel;
		}
	}
}
