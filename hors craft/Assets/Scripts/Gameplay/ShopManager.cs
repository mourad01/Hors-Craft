// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ShopManager
using Common.Managers;
using ItemVInventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityToolbag;

namespace Gameplay
{
	public class ShopManager : Manager
	{
		[Serializable]
		public class CurrencyItem
		{
			public string id;

			public Sprite sprite;

			public CurrencyScriptableObject currency;

			public GameObject currencyPrefab;
		}

		[Serializable]
		public class ShopItem
		{
			public string productId;

			public string currencyId;

			public string shopCategory;

			public int defaultPrice;

			public ItemDefinition item;

			public GameObject shopItemPrefab;

			public bool Unlocked()
			{
				return GetCurrency().CanIBuyThis(productId, defaultPrice);
			}

			public bool Buy()
			{
				if (GetCurrency().TryToBuySomething(productId, defaultPrice))
				{
					item.OnPickup(1);
					return true;
				}
				return false;
			}

			private ICurrency GetCurrency()
			{
				return Manager.Get<ShopManager>().GetCurrency(currencyId);
			}
		}

		public ICurrency defaultCurrency;

		[Reorderable]
		public List<CurrencyItem> currencies = new List<CurrencyItem>();

		[Reorderable]
		public List<ShopItem> shopItems = new List<ShopItem>();

		private Dictionary<string, ICurrency> itemToCurrencyDictionary = new Dictionary<string, ICurrency>();

		public override void Init()
		{
			currencies.ForEach(delegate(CurrencyItem i)
			{
				itemToCurrencyDictionary[i.id] = i.currency;
			});
			AddProductsToManager();
		}

		private void AddProductsToManager()
		{
		}

		public bool Buy(string id)
		{
			ShopItem item = shopItems.FirstOrDefault((ShopItem i) => i.productId == id);
			return Buy(item);
		}

		public bool Buy(ShopItem item)
		{
			ICurrency currency = GetCurrency(item.productId);
			if (currency.TryToBuySomething(item.productId, item.defaultPrice))
			{
				item.item.OnPickup(1);
				return true;
			}
			return false;
		}

		public ICurrency GetCurrency(string id)
		{
			if (itemToCurrencyDictionary.ContainsKey(id))
			{
				return itemToCurrencyDictionary[id];
			}
			UnityEngine.Debug.LogError("Couldn't find currency for id: " + id);
			return defaultCurrency;
		}

		public CurrencyItem GetCurrencyItem(ICurrency currency)
		{
			return currencies.FirstOrDefault((CurrencyItem c) => c.currency == currency as CurrencyScriptableObject);
		}

		public CurrencyItem GetCurrencyItem(string currencyId)
		{
			ICurrency currency = GetCurrency(currencyId);
			return currencies.FirstOrDefault((CurrencyItem c) => c.currency == currency as CurrencyScriptableObject);
		}
	}
}
