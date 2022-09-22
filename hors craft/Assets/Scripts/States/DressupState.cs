// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DressupState
using com.ootii.Cameras;
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using GameUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class DressupState : XCraftUIState<DressupStateConnector>
	{
		public GameObject clothesElementPrefab;

		public Camera cameraPlayerPreview;

		public ModelDragRotator rotator;

		private Vector3 playerOffset = new Vector3(-3.7f, -0.8f, 6f);

		private HumanRepresentation humanRep;

		private List<GameObject> instantiatedPrefabs = new List<GameObject>();

		public int bodyCategory;

		public int legsCategory;

		private bool doubleClicked;

		public bool giveStartResources;

		public bool removeGlamour;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			doubleClicked = false;
			SetOffsetToAspect();
			cameraPlayerPreview = base.connector.cameraPlayerPreview;
			rotator = base.connector.rotator;
			base.connector.returnButtonClicked = OnReturn;
			base.connector.leaderboardsButtonClicked = OnLeaderboards;
			base.connector.skinColorButtonClicked = OnSkinColor;
			base.connector.playButtonClicked = OnPlay;
			base.connector.collectClicked = OnCollect;
			base.connector.eventButtonClicked = OnEventClicked;
			base.connector.watchAdForCoinClicked = OnWatchAdForCoins;
			base.connector.watchDoubleClicked = OnWatchForDouble;
			InitHumanRepresentation();
			FillWithItems(base.connector.changeClothesContainer, 0);
			base.connector.watchDoubleAdsButton.gameObject.SetActive(value: true);
			base.connector.tutorialPopup.SetActive(hasToShowTutorial());
			PlayerPrefs.SetInt("hasShownTutorial", 1);
			for (int j = 0; j < base.connector.categoryButtons.Length; j++)
			{
				int i = j;
				base.connector.categoryButtons[j].onClick.AddListener(delegate
				{
					FillWithItems(base.connector.changeClothesContainer, i);
					Button[] categoryButtons = base.connector.categoryButtons;
					foreach (Button button in categoryButtons)
					{
						button.GetComponent<Image>().color = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.MAIN_COLOR);
					}
					base.connector.categoryButtons[i].GetComponent<Image>().color = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.HIGHLIGHT_COLOR);
				});
			}
			if (Manager.Get<DailyEventManager>().ShouldShowEventOver() && Manager.Contains<ProgressManager>())
			{
				base.connector.eventOverPopup.SetActive(value: true);
			}
			DressupClothesSaveLoad dressupClothesSaveLoad = new DressupClothesSaveLoad();
			humanRep.graphic.SetBodyPartMaterial(BodyPart.Body, dressupClothesSaveLoad.GetSkinIndex(BodyPart.Body));
			humanRep.graphic.SetBodyPartMaterial(BodyPart.Legs, dressupClothesSaveLoad.GetSkinIndex(BodyPart.Legs));
			humanRep.graphic.SetBodyPartMaterial(BodyPart.Head, dressupClothesSaveLoad.GetSkinIndex(BodyPart.Head));
			PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<DressupClothesPlacement>().ShowHat();
			base.connector.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 1f;
			if (giveStartResources && !PlayerPrefs.HasKey("dressup.start.resources.given"))
			{
				Singleton<PlayerData>.get.playerItems.AddResource(8, 30);
				Singleton<PlayerData>.get.playerItems.AddResource(1, 10);
				Singleton<PlayerData>.get.playerItems.AddResource(3, 2);
				PlayerPrefs.SetInt("dressup.start.resources.given", 1);
				PlayerPrefs.Save();
			}
			if (removeGlamour)
			{
				base.connector.GetComponentInChildren<ExperienceBar>().gameObject.SetActive(value: false);
				base.connector.leaderboardsButton.gameObject.SetActive(value: false);
				base.connector.eventButton.gameObject.SetActive(value: false);
			}
		}

		private bool hasToShowTutorial()
		{
			return PlayerPrefs.GetInt("hasShownTutorial", 0) == 0;
		}

		public override void UpdateState()
		{
			base.UpdateState();
			base.connector.wardrobeValue.text = string.Empty + GetWardrobeValue();
		}

		public void OnWatchAdForCoins()
		{
			if (Manager.Contains<AbstractSoftCurrencyManager>())
			{
				Manager.Get<AbstractSoftCurrencyManager>().GetMoreCurrency();
			}
		}

		public void OnWatchForDouble()
		{
			Manager.Get<RewardedAdsManager>().ShowRewardedAd(StatsManager.AdReason.XCRAFT_DOUBLE_REWARD, delegate(bool success)
			{
				if (success)
				{
					WatchedRewardedAd();
				}
			});
		}

		private void WatchedRewardedAd()
		{
			doubleClicked = true;
			base.connector.watchDoubleAdsButton.gameObject.SetActive(value: false);
			base.connector.glamValueText.text = "+" + Manager.Get<ModelManager>().dressupSettings.GetEventGlamourValue() * 2;
			base.connector.coinValueText.text = "+" + Manager.Get<ModelManager>().dressupSettings.GetEventGoldValue() * 2;
		}

		public void OnEventClicked()
		{
			base.connector.eventPopUp.SetActive(value: true);
		}

		public void OnCollect()
		{
			Manager.Get<ProgressManager>().IncreaseExperience(Manager.Get<ModelManager>().dressupSettings.GetEventGlamourValue());
			Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(Manager.Get<ModelManager>().dressupSettings.GetEventGoldValue());
			if (doubleClicked)
			{
				Manager.Get<ProgressManager>().IncreaseExperience(Manager.Get<ModelManager>().dressupSettings.GetEventGlamourValue());
				Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(Manager.Get<ModelManager>().dressupSettings.GetEventGoldValue());
			}
			doubleClicked = false;
			base.connector.eventOverPopup.SetActive(value: false);
		}

		public void OnReturn()
		{
			ResetPlayer();
			Manager.Get<StateMachineManager>().PopState();
		}

		private void ResetPlayer()
		{
			DressupClothesPlacement component = humanRep.graphic.GetComponent<DressupClothesPlacement>();
			component.LoadClothesFromPrefs();
			DressupClothesSaveLoad dressupClothesSaveLoad = new DressupClothesSaveLoad();
			humanRep.graphic.SetBodyPartMaterial(BodyPart.Body, dressupClothesSaveLoad.GetSkinIndex(BodyPart.Body));
			humanRep.graphic.SetBodyPartMaterial(BodyPart.Legs, dressupClothesSaveLoad.GetSkinIndex(BodyPart.Legs));
			component.UpdateShoesMaterial();
			humanRep.UIModeOff();
			PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<DressupClothesPlacement>().HideHat();
			ResetHumanLayer();
			PlayerGraphic.GetControlledPlayerInstance().SaveClothesToPlayerPrefs();
		}

		public void OnLeaderboards()
		{
			ResetPlayer();
			Manager.Get<StateMachineManager>().PopState();
			if (Manager.Contains<RankingManager>())
			{
				Manager.Get<StateMachineManager>().GetStateInstance<PauseState>().TrySwitchFragmenTo("Ranking");
			}
			else
			{
				Manager.Get<SocialPlatformManager>().social.ShowRankings();
			}
		}

		public void OnSkinColor()
		{
		}

		public void OnPlay()
		{
			DressupClothesPlacement component = humanRep.graphic.GetComponent<DressupClothesPlacement>();
			component.LoadClothesFromPrefs();
			DressupClothesSaveLoad dressupClothesSaveLoad = new DressupClothesSaveLoad();
			humanRep.graphic.SetBodyPartMaterial(BodyPart.Body, dressupClothesSaveLoad.GetSkinIndex(BodyPart.Body));
			humanRep.graphic.SetBodyPartMaterial(BodyPart.Legs, dressupClothesSaveLoad.GetSkinIndex(BodyPart.Legs));
			component.UpdateShoesMaterial();
			humanRep.UIModeOff();
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<DressupClothesPlacement>().HideHat();
			ResetHumanLayer();
			PlayerGraphic.GetControlledPlayerInstance().SaveClothesToPlayerPrefs();
		}

		private void ResetHumanLayer()
		{
			PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
			controlledPlayerInstance.graphicRepresentation.SetLayerRecursively(0);
		}

		private void FillWithItems(GameObject parent, int category)
		{
			ClearPrefabs();
			DressupClothesSaveLoad dressupClothesSaveLoad = new DressupClothesSaveLoad();
			List<DressupSkin> possibleSkins = DressupSkinList.instance.possibleSkins;
			List<Skin> possibleSkins2 = DressupSkinList.instance.oldSkinList.possibleSkins;
			foreach (DressupSkin item in possibleSkins)
			{
				if (item.category == category)
				{
					GameObject gameObject = Object.Instantiate(clothesElementPrefab, parent.transform);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.SetAsLastSibling();
					instantiatedPrefabs.Add(gameObject);
					DressupClothingItem component = gameObject.GetComponent<DressupClothingItem>();
					component.itemImage.sprite = item.sprite;
					component.dressupSkin = item;
					component.glamourRequired.text = string.Empty + GetGlamourForClothes(item.id);
					component.coinPrice.text = string.Empty + GetPriceForClothes(item.id);
					component.boughtImage.SetActive(dressupClothesSaveLoad.IsSelected(item.id));
					if (IsItemUnlocked(item.id))
					{
						component.UnlockItem();
					}
					else
					{
						component.LockItem(GetPriceForClothes(item.id), GetGlamourForClothes(item.id), removeGlamour);
					}
					component.SetValues(IsItemUnlocked(item.id), item.id);
				}
			}
			base.connector.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 1f;
			if (category == bodyCategory)
			{
				category = 1;
			}
			else
			{
				if (category != legsCategory)
				{
					return;
				}
				category = 2;
			}
			foreach (Skin item2 in possibleSkins2)
			{
				if (category == 1 || category == 2)
				{
					GameObject gameObject2 = Object.Instantiate(clothesElementPrefab, parent.transform);
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.SetAsLastSibling();
					instantiatedPrefabs.Add(gameObject2);
					DressupClothingItem component2 = gameObject2.GetComponent<DressupClothingItem>();
					component2.itemImage.sprite = DressupSkinList.instance.oldSkinList.getSprite((BodyPart)category, item2.id);
					component2.normalSkin = item2;
					component2.glamourRequired.text = string.Empty + GetGlamourForClothes(item2.id);
					component2.coinPrice.text = string.Empty + GetPriceForClothes(item2.id);
					component2.boughtImage.SetActive(dressupClothesSaveLoad.IsSelected(item2.id, (BodyPart)category));
					if (IsItemUnlocked((BodyPart)category, item2.id))
					{
						component2.UnlockItem();
					}
					else
					{
						component2.LockItem(GetPriceForClothes(item2.id), GetGlamourForClothes(item2.id), removeGlamour);
					}
					component2.SetValues(IsItemUnlocked(item2.id), item2.id, (BodyPart)category);
				}
			}
			base.connector.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 1f;
		}

		private void ClearPrefabs()
		{
			foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
			{
				UnityEngine.Object.Destroy(instantiatedPrefab);
			}
		}

		private void SetOffsetToAspect()
		{
			if (CameraController.instance.MainCamera.aspect > 1.8f)
			{
				playerOffset = new Vector3(-3.7f, -0.8f, 6f);
			}
			else if (CameraController.instance.MainCamera.aspect > 1.5f && CameraController.instance.MainCamera.aspect <= 1.8f)
			{
				playerOffset = new Vector3(-2.65f, -0.8f, 4.5f);
			}
			else if (CameraController.instance.MainCamera.aspect > 1.4f)
			{
				playerOffset = new Vector3(-2.7f, -0.8f, 4.5f);
			}
			else if (CameraController.instance.MainCamera.aspect <= 1.4f)
			{
				playerOffset = new Vector3(-2.3f, -0.8f, 4.5f);
			}
		}

		public void InitHumanRepresentation()
		{
			humanRep = new HumanRepresentation(PlayerGraphic.GetControlledPlayerInstance());
			humanRep.UIModeOn(SetPlayerRepresentationPlace);
			humanRep.graphic.ShowBodyAndLegs();
			humanRep.graphic.ShowHands();
		}

		public HumanRepresentation GetHumanRep()
		{
			return humanRep;
		}

		public bool TryToBuyItem(int id, DressupSkin dSkin = null, Skin skin = null, BodyPart part = BodyPart.Head)
		{
			bool success = false;
			Manager.Get<AbstractSoftCurrencyManager>().TryToBuySomething(GetPriceForClothes(id), delegate
			{
				if (dSkin.modelPrefab != null)
				{
					Singleton<PlayerData>.get.playerItems.OnItemUnlock(id);
				}
				if (skin.texture != null)
				{
					Singleton<PlayerData>.get.playerItems.OnItemUnlock(part, id);
				}
				success = true;
				if (Manager.Contains<AbstractAchievementManager>())
				{
					Manager.Get<AbstractAchievementManager>().RegisterEvent("clothes");
				}
			}, delegate
			{
				Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("dressup.not.enough.coins", "You don't have enough coins!"));
			});
			return success;
		}

		public void ShowSelectedOnModel(int index, DressupSkin dskin = null, Skin skin = null, BodyPart part = BodyPart.Head)
		{
			DressupClothesSaveLoad dressupClothesSaveLoad = new DressupClothesSaveLoad();
			DressupClothesPlacement component = humanRep.graphic.GetComponent<DressupClothesPlacement>();
			if (skin != null)
			{
				humanRep.graphic.SetBodyPartMaterial(part, index);
				if (IsItemUnlocked(part, index))
				{
					dressupClothesSaveLoad.SaveSkin(part, index);
				}
			}
			if (dskin != null && dskin.modelPrefab != null)
			{
				if (dskin.placement != DressupSkin.Placement.Hair && dressupClothesSaveLoad.IsSelected(dskin.id))
				{
					component.TakeItemOff(dskin);
				}
				else
				{
					if (dskin.placement == DressupSkin.Placement.Hair)
					{
						int index2 = (dskin.id - 20) % 6;
						dressupClothesSaveLoad.SaveSkin(BodyPart.Head, index2);
						humanRep.graphic.SetBodyPartMaterial(BodyPart.Head, dressupClothesSaveLoad.GetSkinIndex(BodyPart.Head));
					}
					component.SpawnItem(dskin, IsItemUnlocked(dskin.id));
				}
			}
			humanRep.UIModeOn(SetPlayerRepresentationPlace);
			component.UpdateShoesMaterial();
		}

		private void SetPlayerRepresentationPlace(GameObject player)
		{
			player.transform.position = cameraPlayerPreview.transform.position + playerOffset;
			player.transform.localRotation = Quaternion.Euler(0f, 146f, 0f);
			rotator.modelToRotate = player;
		}

		private int GetPriceForClothes(int id)
		{
			return Manager.Get<ModelManager>().dressupSettings.GetClothesItemBasePriceValue(id);
		}

		private int GetGlamourForClothes(int id)
		{
			return Manager.Get<ModelManager>().dressupSettings.GetClothesItemPrestigeRequired(id);
		}

		private bool IsItemUnlocked(int itemId)
		{
			if (GetPriceForClothes(itemId) <= 0)
			{
				Singleton<PlayerData>.get.playerItems.OnItemUnlock(itemId);
			}
			string skinId = $"{itemId}";
			return Singleton<PlayerData>.get.playerItems.IsItemUnlocked(skinId);
		}

		private bool IsItemUnlocked(BodyPart part, int itemId)
		{
			if (GetPriceForClothes(itemId) <= 0)
			{
				Singleton<PlayerData>.get.playerItems.OnItemUnlock(part, itemId);
			}
			string skinId = $"{itemId}.{part}";
			return Singleton<PlayerData>.get.playerItems.IsItemUnlocked(skinId);
		}

		private int GetWardrobeValue()
		{
			int num = 0;
			List<DressupSkin> possibleSkins = DressupSkinList.instance.possibleSkins;
			List<Skin> possibleSkins2 = DressupSkinList.instance.oldSkinList.possibleSkins;
			foreach (DressupSkin item in possibleSkins)
			{
				if (IsItemUnlocked(item.id))
				{
					num += GetPriceForClothes(item.id);
				}
			}
			foreach (Skin item2 in possibleSkins2)
			{
				if (IsItemUnlocked(BodyPart.Body, item2.id))
				{
					num += GetPriceForClothes(item2.id);
				}
				if (IsItemUnlocked(BodyPart.Legs, item2.id))
				{
					num += GetPriceForClothes(item2.id);
				}
			}
			return num;
		}
	}
}
