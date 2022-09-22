// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DressupStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class DressupStateConnector : UIConnector
	{
		public delegate void OnClick();

		public GameObject changeClothesContainer;

		public Camera cameraPlayerPreview;

		public ModelDragRotator rotator;

		public Button returnButton;

		public Button leaderboardsButton;

		public Button skinColorButton;

		public Button playButton;

		public Button eventButton;

		public TranslateText dailyEventNameText;

		public Text wardrobeValue;

		public Button[] categoryButtons;

		public OnClick returnButtonClicked;

		public OnClick leaderboardsButtonClicked;

		public OnClick skinColorButtonClicked;

		public OnClick playButtonClicked;

		public OnClick eventButtonClicked;

		public OnClick collectClicked;

		public OnClick watchDoubleClicked;

		public OnClick watchAdForCoinClicked;

		public GameObject eventPopUp;

		public TranslateText gotitButtonText;

		public TranslateText eventDescription;

		public TranslateText eventTitle;

		public GameObject eventOverPopup;

		public Button watchDoubleAdsButton;

		public Button collectButton;

		public Text glamValueText;

		public Text coinValueText;

		public GameObject coinsPopup;

		public Text coinsForAdValue;

		public Button watchAdForCoins;

		public GameObject tutorialPopup;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				if (returnButtonClicked != null)
				{
					returnButtonClicked();
				}
			});
			leaderboardsButton.onClick.AddListener(delegate
			{
				if (leaderboardsButtonClicked != null)
				{
					leaderboardsButtonClicked();
				}
			});
			skinColorButton.onClick.AddListener(delegate
			{
				if (skinColorButtonClicked != null)
				{
					skinColorButtonClicked();
				}
			});
			playButton.onClick.AddListener(delegate
			{
				if (playButtonClicked != null)
				{
					playButtonClicked();
				}
			});
			eventButton.onClick.AddListener(delegate
			{
				if (eventButtonClicked != null)
				{
					eventButtonClicked();
				}
			});
			collectButton.onClick.AddListener(delegate
			{
				if (collectClicked != null)
				{
					collectClicked();
				}
			});
			watchDoubleAdsButton.onClick.AddListener(delegate
			{
				if (watchDoubleClicked != null)
				{
					watchDoubleClicked();
				}
			});
			watchAdForCoins.onClick.AddListener(delegate
			{
				if (watchAdForCoinClicked != null)
				{
					watchAdForCoinClicked();
				}
			});
			categoryButtons[3].GetComponentsInChildren<Image>()[1].sprite = DressupSkinList.instance.oldSkinList.getSprite(BodyPart.Body, 0);
			categoryButtons[4].GetComponentsInChildren<Image>()[1].sprite = DressupSkinList.instance.oldSkinList.getSprite(BodyPart.Legs, 0);
			eventPopUp.SetActive(value: false);
			gotitButtonText.translationKey = "rateus.ok";
			eventDescription.translationKey = "dressup.event.description." + (int)Manager.Get<DailyEventManager>().currentDay;
			eventTitle.translationKey = "dressup.event.name." + (int)Manager.Get<DailyEventManager>().currentDay;
			dailyEventNameText.translationKey = "dressup.event.name." + (int)Manager.Get<DailyEventManager>().currentDay;
			dailyEventNameText.ForceRefresh();
			coinValueText.text = "+" + Manager.Get<ModelManager>().dressupSettings.GetEventGoldValue();
			glamValueText.text = "+" + Manager.Get<ModelManager>().dressupSettings.GetEventGlamourValue();
			int level = 1;
			if (Manager.Contains<ProgressManager>())
			{
				level = Manager.Get<ProgressManager>().level;
			}
			int num = Manager.Get<ModelManager>().currencySettings.SoftCurrencyPerAd(level);
			coinsForAdValue.text = "+" + num.ToString();
		}
	}
}
