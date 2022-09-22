// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.PeriodicFreeChestItemUI
using Common.Managers;
using System;
using UnityEngine.UI;

namespace Gameplay
{
	public class PeriodicFreeChestItemUI : ShopItemUI
	{
		public Text timerText;

		private string freeInTextTranslation;

		private bool isOpenTextActive;

		private ShopManager.ShopItem item;

		private Action<bool> onBuyCallback;

		private int price;

		public override void Init(ShopManager.ShopItem item, Action<bool> onBuyCallback)
		{
			base.Init(item, onBuyCallback);
			this.item = item;
			this.onBuyCallback = onBuyCallback;
			price = Manager.Get<ModelManager>().currencySettings.GetPriceFor(item.currencyId, item.productId, item.defaultPrice);
			freeInTextTranslation = Manager.Get<TranslationsManager>().GetText("chest.free.in", "Free in: {0}");
			UpdateTimer();
			UpdateButton();
			buyButton.onClick.RemoveAllListeners();
			buyButton.onClick.AddListener(delegate
			{
				OnBuyButton();
			});
		}

		private void Update()
		{
			if (item != null)
			{
				UpdateTimer();
				UpdateButton();
			}
		}

		private void UpdateTimer()
		{
			float nextItemCooldown = AutoRefreshingStock.GetNextItemCooldown("free.chest");
			if (nextItemCooldown == 0f)
			{
				timerText.text = string.Empty;
				return;
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(nextItemCooldown);
			string newValue = $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
			string text = freeInTextTranslation.Replace("{0}", newValue);
			timerText.text = text;
		}

		private void UpdateButton()
		{
			if (AutoRefreshingStock.GetStockCount("free.chest") >= 1)
			{
				if (!isOpenTextActive)
				{
					priceText.text = Manager.Get<TranslationsManager>().GetText("chest.free.open", "Open");
					isOpenTextActive = true;
				}
			}
			else if (isOpenTextActive)
			{
				priceText.text = price.ToString();
				isOpenTextActive = false;
			}
		}

		private void OnBuyButton()
		{
			if (AutoRefreshingStock.GetStockCount("free.chest") >= 1)
			{
				item.item.OnPickup(1);
				AutoRefreshingStock.DecrementStockCount("free.chest");
			}
			else
			{
				bool obj = item.Buy();
				onBuyCallback(obj);
			}
		}
	}
}
