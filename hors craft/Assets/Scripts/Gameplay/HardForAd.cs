// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.HardForAd
using Common.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class HardForAd : MonoBehaviour
	{
		public Button button;

		public Image image;

		public Sprite activeSprite;

		public Sprite disabledSprite;

		private HardCurrencyManager hcManager;

		private void Start()
		{
			hcManager = Manager.Get<HardCurrencyManager>();
			button.onClick.AddListener(OnHardForAd);
		}

		private void Update()
		{
			if (hcManager.CanGetMoreCurrencyForAd())
			{
				image.sprite = activeSprite;
			}
			else
			{
				image.sprite = disabledSprite;
			}
		}

		private void OnHardForAd()
		{
			if (hcManager.CanGetMoreCurrencyForAd())
			{
				hcManager.GetMoreCurrencyForAd(decrementStock: true);
				return;
			}
			float nextItemCooldown = AutoRefreshingStock.GetNextItemCooldown("hardForAd");
			TimeSpan timeSpan = TimeSpan.FromSeconds(nextItemCooldown);
			string newValue = $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
			string text = Manager.Get<TranslationsManager>().GetText("free.coins.available.in", "Free coins will be available in: {0}").Replace("{0}", newValue);
			Manager.Get<ToastManager>().ShowToast(text);
		}
	}
}
