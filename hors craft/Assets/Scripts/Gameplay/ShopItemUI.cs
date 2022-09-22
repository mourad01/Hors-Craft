// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ShopItemUI
using Common.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class ShopItemUI : MonoBehaviour
	{
		public Text nameText;

		public Button buyButton;

		public Image image;

		public Image currencyImage;

		public Text priceText;

		public GameObject[] lockedGOs;

		public GameObject[] unlockedGOs;

		public virtual void Init(ShopManager.ShopItem item, Action<bool> onBuyCallback)
		{
			image.sprite = item.item.itemSprite;
			image.type = Image.Type.Simple;
			nameText.text = Manager.Get<TranslationsManager>().GetText(item.productId, item.productId);
			if (currencyImage != null)
			{
				ShopManager.CurrencyItem currencyItem = Manager.Get<ShopManager>().GetCurrencyItem(item.currencyId);
				currencyImage.sprite = currencyItem.sprite;
			}
			if (priceText != null)
			{
				priceText.text = Manager.Get<ModelManager>().currencySettings.GetPriceFor(item.currencyId, item.productId, item.defaultPrice).ToString();
			}
			item.Unlocked();
			buyButton.onClick.RemoveAllListeners();
			buyButton.onClick.AddListener(delegate
			{
				bool obj = item.Buy();
				onBuyCallback(obj);
			});
		}
	}
}
