// DecompilerFi decompiler from Assembly-CSharp.dll class: States.StarterPackStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class StarterPackStateConnector : UIConnector
	{
		public Image background;

		public Image[] colorItems;

		public Color boyColor;

		public Color girlColor;

		public string boyBGpath;

		public string girlBGpath;

		public Text packageWorth;

		public Text[] items;

		public Text realCost;

		public Button buyButton;

		public virtual void Init(OfferPackDefinition definition, Skin.Gender gender, Action buyAction)
		{
			SetStarterPackGender(LoadBackgroundResources((gender != Skin.Gender.FEMALE) ? boyBGpath : girlBGpath), (gender != Skin.Gender.FEMALE) ? boyColor : girlColor);
			SetItemsTexts(definition);
			SetRealButton(buyAction);
		}

		private void SetRealButton(Action buyAction)
		{
		}

		public void UnloadBackground()
		{
			if (!(background == null) && background.sprite != null)
			{
				Resources.UnloadAsset(background.sprite);
			}
		}

		protected virtual Sprite LoadBackgroundResources(string path)
		{
			return Resources.Load<Sprite>(path);
		}

		private void SetItemsTexts(OfferPackDefinition definition)
		{
			if (definition != null)
			{
				WorldItemData worldItemData = definition.TryToGet<WorldItemData>();
				SoftCurrencyItemData softCurrencyItemData = definition.TryToGet<SoftCurrencyItemData>();
				ClothesItemData clothesItemData = definition.TryToGet<ClothesItemData>();
				BlocksItemData blocksItemData = definition.TryToGet<BlocksItemData>();
				WorldData worldFromModelById = Manager.Get<SavedWorldManager>().GetWorldFromModelById(worldItemData.uniqueItemId);
				if (worldFromModelById != null)
				{
					items[0].text = string.Format("1 {0}", Manager.Get<TranslationsManager>().GetText(worldFromModelById.titleId, "World"));
					int count = blocksItemData.blocks.Count;
					int count2 = clothesItemData.clothesIds.Count;
					items[1].text = string.Format(Manager.Get<TranslationsManager>().GetText("starterpack.blocksandclothes", "{0} NEW BLOCKS & {1} NEW CLOTHES"), count, count2);
					items[2].text = Manager.Get<TranslationsManager>().GetText("starterpack.golddoubler", "DOUBLE COINS FROM FREE CHEST");
					items[3].text = string.Format(Manager.Get<TranslationsManager>().GetText("starterpack.golddoubler", "{0} GOLD COINS"), softCurrencyItemData.value);
				}
			}
		}

		private void SetStarterPackGender(Sprite bg, Color color)
		{
			if (bg != null)
			{
				background.color = Color.white;
			}
			background.sprite = bg;
			Array.ForEach(colorItems, delegate(Image item)
			{
				item.color = color;
			});
		}

		public virtual void UpdateClock()
		{
		}
	}
}
