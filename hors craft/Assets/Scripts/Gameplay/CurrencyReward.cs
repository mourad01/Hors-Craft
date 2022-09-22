// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CurrencyReward
using Common.Gameplay;
using Common.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class CurrencyReward : Reward
	{
		public CurrencyScriptableObject currency;

		public override void ClaimReward()
		{
			UpdateSprite();
			currency.OnCurrencyAmountChange(amount);
		}

		public override List<Sprite> GetSprites()
		{
			UpdateSprite();
			return baseSprite.AsList();
		}

		private void UpdateSprite()
		{
			if (Manager.Contains<ShopManager>() && Manager.Get<ShopManager>().currencies.FirstOrDefault((ShopManager.CurrencyItem c) => c.id == key) != null)
			{
				ShopManager.CurrencyItem currencyItem = Manager.Get<ShopManager>().GetCurrencyItem(key);
				if (currencyItem != null)
				{
					currency = currencyItem.currency;
					baseSprite = currencyItem.sprite;
				}
			}
		}
	}
}
