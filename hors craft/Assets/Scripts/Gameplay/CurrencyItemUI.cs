// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.CurrencyItemUI
using Common.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class CurrencyItemUI : MonoBehaviour
	{
		public Text amountText;

		public Button addButton;

		[Header("Optional")]
		public Image image;

		public Text nameText;

		public CurrencyScriptableObject currencySO;

		private ICurrency currency;

		private string currencyName;

		private int previousAmount = -1;

		private void Awake()
		{
			if (currencySO != null)
			{
				Action onAdd = null;
				if (currencySO.CanGetMoreCurrency())
				{
					CurrencyScriptableObject currencyScriptableObject = currencySO;
					onAdd = currencyScriptableObject.GetMoreCurrency;
				}
				Init(currencySO, onAdd);
			}
		}

		public void Init(ICurrency currency, Action onAdd = null)
		{
			this.currency = currency;
			ShopManager.CurrencyItem currencyItem = Manager.Get<ShopManager>()?.GetCurrencyItem(currency);
			if (currencyItem != null)
			{
				if (image != null && currencyItem.sprite != null)
				{
					image.sprite = currencyItem.sprite;
				}
				currencyName = Manager.Get<TranslationsManager>().GetText(currencyItem.id, currencyItem.id);
				if (nameText != null)
				{
					nameText.text = currencyName;
				}
			}
			UpdateCurrencies();
			addButton.onClick.RemoveAllListeners();
			if (onAdd == null)
			{
				addButton.gameObject.SetActive(value: false);
				return;
			}
			addButton.gameObject.SetActive(value: true);
			addButton.onClick.AddListener(delegate
			{
				onAdd();
			});
		}

		private void Update()
		{
			if (currency != null)
			{
				UpdateCurrencies();
			}
		}

		private void UpdateCurrencies()
		{
			int currencyAmount = currency.GetCurrencyAmount();
			if (currencyAmount != previousAmount)
			{
				amountText.text = currencyAmount.ToString();
				previousAmount = currencyAmount;
			}
		}
	}
}
