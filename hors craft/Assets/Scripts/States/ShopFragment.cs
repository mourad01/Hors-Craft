// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ShopFragment
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ShopFragment : Fragment
	{
		[Serializable]
		public class Slot
		{
			public Transform parent;

			public int capacity;

			public bool HasSlot()
			{
				return capacity > parent.childCount;
			}
		}

		[Serializable]
		public class CurrencySlot : Slot
		{
			public CurrencyScriptableObject currency;
		}

		[Serializable]
		public class ShopItemSlot : Slot
		{
			public string category;
		}

		public Button backButton;

		public List<CurrencySlot> currencySlots = new List<CurrencySlot>();

		public List<ShopItemSlot> itemSlots = new List<ShopItemSlot>();

		public GameObject hardForAd;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			UpdateShop();
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			UpdateShop();
		}

		private void UpdateShop()
		{
			ClearShop();
			List<ShopManager.ShopItem> shopItems = Manager.Get<ShopManager>().shopItems;
			shopItems.ForEach(delegate(ShopManager.ShopItem i)
			{
				AddItem(i);
			});
			List<ShopManager.CurrencyItem> currencies = Manager.Get<ShopManager>().currencies;
			currencies.ForEach(delegate(ShopManager.CurrencyItem c)
			{
				AddItem(c);
			});
			UpdateBackButton();
			UpdateHardForAdButton();
		}

		private void AddItem(ShopManager.ShopItem item)
		{
			ShopItemSlot shopItemSlot = itemSlots.FirstOrDefault((ShopItemSlot s) => s.HasSlot() && s.category == item.shopCategory);
			if (shopItemSlot == null)
			{
				UnityEngine.Debug.LogError("Couldn't add item " + item.productId + " to shop.");
				return;
			}
			shopItemSlot.parent.gameObject.SetActive(value: true);
			GameObject gameObject = UnityEngine.Object.Instantiate(item.shopItemPrefab, shopItemSlot.parent, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			ShopItemUI component = gameObject.GetComponent<ShopItemUI>();
			component.Init(item, OnItemBuy);
		}

		private void AddItem(ShopManager.CurrencyItem item)
		{
			CurrencySlot currencySlot = currencySlots.FirstOrDefault((CurrencySlot s) => s.HasSlot() && s.currency == item.currency);
			if (currencySlot == null)
			{
				UnityEngine.Debug.LogError("Couldn't add currency " + item.currency.name + " to shop.");
				return;
			}
			currencySlot.parent.gameObject.SetActive(value: true);
			GameObject gameObject = UnityEngine.Object.Instantiate(item.currencyPrefab, currencySlot.parent, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			CurrencyItemUI component = gameObject.GetComponent<CurrencyItemUI>();
			object obj;
			if (item.currency.CanGetMoreCurrency())
			{
				CurrencyScriptableObject currency = item.currency;
				obj = new Action(currency.GetMoreCurrency);
			}
			else
			{
				obj = null;
			}
			Action onAdd = (Action)obj;
			component.Init(item.currency, onAdd);
		}

		private void ClearShop()
		{
			currencySlots.ForEach(delegate(CurrencySlot s)
			{
				ClearParent(s.parent);
			});
			itemSlots.ForEach(delegate(ShopItemSlot s)
			{
				ClearParent(s.parent);
			});
		}

		private void ClearParent(Transform parent)
		{
			while (parent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(parent.GetChild(0).gameObject);
			}
			parent.gameObject.SetActive(value: false);
		}

		private void OnItemBuy(bool success)
		{
			if (!success)
			{
				Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("shop.no.coins", "You don't have enough coins"));
			}
		}

		private void UpdateBackButton()
		{
			if (Manager.Get<StateMachineManager>().currentState is PauseState)
			{
				backButton.gameObject.SetActive(value: false);
				return;
			}
			backButton.gameObject.SetActive(value: true);
			backButton.onClick.RemoveAllListeners();
			backButton.onClick.AddListener(delegate
			{
				Manager.Get<StateMachineManager>().PopState();
			});
		}

		private void UpdateHardForAdButton()
		{
			if (Manager.Get<ModelManager>().lootSettings.IsFreeHcEnabled())
			{
				hardForAd.SetActive(value: true);
			}
		}
	}
}
