// DecompilerFi decompiler from Assembly-CSharp.dll class: States.OfferPackStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class OfferPackStateConnector : UIConnector
	{
		public GameObject blocksFrame;

		public GameObject worldFrame;

		public GameObject chestParent;

		public Text worldTitleText;

		public Text chestCostText;

		public Text realCost;

		public Button buyButton;

		public Image worldImage;

		public GameObject blocksParent;

		public GameObject itemPrefab;

		public Text timeText;

		private List<GameObject> itemsElements;

		public void Init(OfferPackDefinition definition, Action buyAction)
		{
			WorldItemData worldItem = definition.TryToGet<WorldItemData>();
			SoftCurrencyItemData sCItem = definition.TryToGet<SoftCurrencyItemData>();
			ClothesItemData clothesItem = definition.TryToGet<ClothesItemData>();
			BlocksItemData blockItem = definition.TryToGet<BlocksItemData>();
			SetWorldItem(worldItem);
			SetSCItem(sCItem);
			SetBlocksAndClothes(blockItem, clothesItem);
			SetRealButton(buyAction);
			int num = Mathf.CeilToInt((float)(Manager.Get<OfferPackManager>().CalculateTimeToEndOfOfferPack() / 3600.0));
			string text = Manager.Get<TranslationsManager>().GetText("offerpack.timevalid", "{0} HOURS ");
			timeText.text = string.Format(text, num);
		}

		private void SetRealButton(Action buyAction)
		{
		}

		private void SetWorldItem(WorldItemData worldItem)
		{
			worldFrame.SetActive(worldItem != null);
			if (worldItem != null)
			{
				WorldData worldFromModelById = Manager.Get<SavedWorldManager>().GetWorldFromModelById(worldItem.uniqueItemId);
				if (worldFromModelById != null)
				{
					worldTitleText.text = Manager.Get<TranslationsManager>().GetText(worldFromModelById.titleId, "World");
					worldFromModelById.TryToGetThumbnail(delegate(Sprite sprite)
					{
						worldImage.sprite = sprite;
					}, null);
				}
			}
		}

		private void SetSCItem(SoftCurrencyItemData scItem)
		{
			chestParent.SetActive(scItem != null);
			if (scItem != null)
			{
				chestCostText.text = scItem.value.ToString();
			}
		}

		private void SetBlocksAndClothes(BlocksItemData blockItem, ClothesItemData clothesItem)
		{
			blocksFrame.SetActive(clothesItem != null || blockItem != null);
			if (clothesItem != null && blockItem != null)
			{
				List<Sprite> sprites = new List<Sprite>();
				blockItem.blocks.ForEach(delegate(ushort blockId)
				{
					sprites.Add(VoxelSprite.GetVoxelSprite(blockId));
				});
				clothesItem.clothesIds.ForEach(delegate(int id)
				{
					sprites.Add(SkinList.instance.getSprite(BodyPart.Head, id));
					sprites.Add(SkinList.instance.getSprite(BodyPart.Body, id));
					sprites.Add(SkinList.instance.getSprite(BodyPart.Legs, id));
				});
				CreateItemElements(sprites);
			}
		}

		private void CreateItemElements(List<Sprite> sprites)
		{
			itemsElements = new List<GameObject>();
			sprites.ForEach(delegate(Sprite sprite)
			{
				itemsElements.Add(CreateItem(sprite));
			});
		}

		private GameObject CreateItem(Sprite sprite)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(itemPrefab, blocksParent.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.GetComponent<SimpleOfferItem>().image.sprite = sprite;
			return gameObject;
		}
	}
}
