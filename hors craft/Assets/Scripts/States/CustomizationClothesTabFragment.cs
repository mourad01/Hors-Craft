// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CustomizationClothesTabFragment
using Common.Managers;
using Gameplay;
using GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CustomizationClothesTabFragment : Fragment
	{
		public GameObject headScrollParent;

		public GameObject bodyScrollParent;

		public GameObject legsScrollParent;

		public GameObject legsParent;

		public GameObject clothesElementPrefab;

		private bool _hasShowLegs = true;

		private CustomizationFragment.CustomizationStartParameter startParam;

		public bool hasShowLegs
		{
			get
			{
				return _hasShowLegs;
			}
			set
			{
				_hasShowLegs = value;
				legsScrollParent.SetActive(_hasShowLegs);
				legsParent.SetActive(_hasShowLegs);
			}
		}

		public static int AdsNeededForItem(int id)
		{
			int num = PlayerPrefs.GetInt("numberOfWatchedRewardedAdsClothes", 0);
			if (PlayerPrefs.GetInt("overrideclothesadsnumber", 0) == 1)
			{
				num = 99999;
			}
			int clothesPerAds = Manager.Get<ModelManager>().clothesSetting.GetClothesPerAds();
			int freeClothes = Manager.Get<ModelManager>().clothesSetting.GetFreeClothes();
			return Mathf.FloorToInt((float)(id - freeClothes) / (float)clothesPerAds) + 1 - num;
		}

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			startParam = (parameter as CustomizationFragment.CustomizationStartParameter);
			int @int = PlayerPrefs.GetInt("numberOfWatchedRewardedAdsClothes", 0);
			SkinList skinList = (!(SkinList.customPlayerSkinList != null)) ? SkinList.instance : SkinList.customPlayerSkinList;
			InitializeItems(skinList.shopSprites, @int);
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			RevalidateLists();
			hasShowLegs = PlayerGraphic.GetControlledPlayerInstance().hasLegs;
			PlayerGraphic.GetControlledPlayerInstance().ShowHands();
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			RevalidateLists();
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
		}

		private void InitializeItems(Dictionary<BodyPart, Dictionary<string, SkinList.SpriteP>> clothesSprites, int watchedAds)
		{
			fillWitItems(headScrollParent, clothesSprites[BodyPart.Head], BodyPart.Head, watchedAds);
			fillWitItems(bodyScrollParent, clothesSprites[BodyPart.Body], BodyPart.Body, watchedAds);
			fillWitItems(legsScrollParent, clothesSprites[BodyPart.Legs], BodyPart.Legs, watchedAds);
		}

		private void SetBodyElement(BodyPart part, int index)
		{
			if (Manager.Get<ModelManager>().clothesSetting.GetUnlockType() == ItemsUnlockModel.Ads)
			{
				SetBodyElementAds(part, index);
			}
			else
			{
				SetBodyElementCurrency(part, index);
			}
		}

		private void revalidateListItems(BodyPart part, GameObject list)
		{
			IEnumerator enumerator = list.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					ClothItem component = transform.GetComponent<ClothItem>();
					if (component != null)
					{
						component.Revalidate(GetValueForClothes(part, component.skin.id));
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		private GameObject GetItemList(BodyPart part)
		{
			switch (part)
			{
			case BodyPart.Head:
				return headScrollParent;
			case BodyPart.Body:
				return bodyScrollParent;
			case BodyPart.Legs:
				return legsScrollParent;
			default:
				return headScrollParent;
			}
		}

		private void SetBodyElementCurrency(BodyPart part, int index)
		{
			if (canWearItem(part, index))
			{
				startParam.parentFragment.SetBodyPart(part, index);
			}
			else
			{
				TryToBuyItem(part, index);
			}
		}

		private bool canWearItem(BodyPart part, int value)
		{
			return GetValueForClothes(part, value) <= 0 || Singleton<PlayerData>.get.playerItems.IsItemUnlocked($"{value}.{part}");
		}

		private void TryToBuyItem(BodyPart part, int index)
		{
			Manager.Get<AbstractSoftCurrencyManager>().TryToBuySomething(GetValueForClothes(part, index), delegate
			{
				string clothesId = Singleton<PlayerData>.get.playerItems.OnItemUnlock(part, index);
				startParam.parentFragment.SetBodyPart(part, index);
				revalidateListItems(part, GetItemList(part));
				Manager.Get<StatsManager>().ClothBought(clothesId, Manager.Get<AbstractSoftCurrencyManager>().GetCurrencyAmount(), 0);
			});
		}

		private int GetValueForClothes(BodyPart part, int itemId)
		{
			bool flag = Manager.Get<ModelManager>().clothesSetting.GetUnlockType() == ItemsUnlockModel.Ads;
			string text = $"{itemId}.{part}";
			if (Singleton<PlayerData>.get.playerItems.IsItemUnlocked(text))
			{
				return 0;
			}
			return (!flag) ? Manager.Get<AbstractSoftCurrencyManager>().ClothesCost(text) : AdsNeededForItem(itemId);
		}

		private void SetBodyElementAds(BodyPart part, int index)
		{
			if (canWearItem(index))
			{
				startParam.parentFragment.SetBodyPart(part, index);
				return;
			}
			WatchXAdsPopUpStateStartParameter watchXAdsPopUpStateStartParameter = new WatchXAdsPopUpStateStartParameter();
			SkinList skinList = (!(SkinList.customPlayerSkinList != null)) ? SkinList.instance : SkinList.customPlayerSkinList;
			watchXAdsPopUpStateStartParameter.voxelSpritesToUnlock = skinList.GetNextToUnlockSprites();
			watchXAdsPopUpStateStartParameter.type = AdsCounters.Clothes;
			watchXAdsPopUpStateStartParameter.reason = StatsManager.AdReason.XCRAFT_CLOTHES;
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(watchXAdsPopUpStateStartParameter);
		}

		private bool canWearItem(int value)
		{
			return AdsNeededForItem(value) <= 0;
		}

		private void RevalidateLists()
		{
			revalidateListItems(BodyPart.Head, headScrollParent);
			revalidateListItems(BodyPart.Body, bodyScrollParent);
			revalidateListItems(BodyPart.Legs, legsScrollParent);
		}

		private void fillWitItems(GameObject parent, Dictionary<string, SkinList.SpriteP> elements, BodyPart part, int watchedAds)
		{
			int num = 0;
			bool ads = Manager.Get<ModelManager>().clothesSetting.GetUnlockType() == ItemsUnlockModel.Ads;
			foreach (KeyValuePair<string, SkinList.SpriteP> element in elements)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(clothesElementPrefab, parent.transform);
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.SetAsLastSibling();
				ClothItem component = gameObject.GetComponent<ClothItem>();
				component.clothImage.sprite = element.Value.sprite;
				BodyPart part2 = part;
				int idDel = num;
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					SetBodyElement(part, idDel);
				});
				component.SetValues(element.Value.skin, GetValueForClothes(part2, element.Value.skin.id), ads);
				num++;
			}
			float width = clothesElementPrefab.GetComponent<RectTransform>().rect.width;
			width = width * (float)elements.Count + parent.GetComponent<HorizontalLayoutGroup>().spacing * (float)(elements.Count - 1);
			parent.transform.localPosition = parent.transform.localPosition.WithX(width * 0.5f);
		}

		private void OnEnable()
		{
			RevalidateLists();
		}
	}
}
