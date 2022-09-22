// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DynamicStarterPackStateConnector
using Common.Managers;
using Gameplay;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class DynamicStarterPackStateConnector : StarterPackStateConnector
	{
		public Button closeButton;

		public Button buyPackButton;

		public Text clockText;

		public Image packBackground;

		private DynamicOfferPackManager offerPackManager;

		private OfferSettingsModule model => Manager.Get<ModelManager>().offerPackSettings;

		public override void Init(OfferPackDefinition definition, Skin.Gender gender, Action buyAction)
		{
			offerPackManager = Manager.Get<DynamicOfferPackManager>();
			SetButtons(buyAction);
			SetCorrectPack();
		}

		private void SetCorrectPack()
		{
			if (!(offerPackManager == null))
			{
				int index = offerPackManager.LoadCurrentDynamicStarterPack();
				string path = $"{DynamicOfferPackManager.PATH_PACKS_BACKGROUND_FOLDER}{model.GetDynamicStarterBackgroundImage(index)}";
				Sprite sprite = LoadBackgroundResources(path);
				if (sprite != null)
				{
					packBackground.sprite = sprite;
				}
			}
		}

		public override void UpdateClock()
		{
			if (!(offerPackManager == null))
			{
				clockText.text = offerPackManager.GetFormattedTimeToEnd(longFormat: true);
			}
		}

		private void SetButtons(Action buyAction)
		{
			SetCloseButton();
			SetBuyButton(buyAction);
		}

		private void SetCloseButton()
		{
			closeButton.onClick.AddListener(delegate
			{
				Manager.Get<StateMachineManager>().PopState();
			});
		}

		private void SetBuyButton(Action buyAction)
		{
			buyPackButton.onClick.AddListener(delegate
			{
				buyAction();
			});
		}

		protected override Sprite LoadBackgroundResources(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			if (File.Exists(path))
			{
				byte[] data = File.ReadAllBytes(path);
				Texture2D texture2D = new Texture2D(4, 4, TextureFormat.RGBA32, mipChain: false);
				texture2D.LoadImage(data);
				return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			}
			return null;
		}
	}
}
