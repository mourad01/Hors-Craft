// DecompilerFi decompiler from Assembly-CSharp.dll class: LoveManager
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoveManager : Manager
{
	[Serializable]
	public class RelationshipStage
	{
		public Sprite tamePanelSprite;

		public RelationshipStageSetup stageSetup;

		public string stageNameKey;

		public string stageNameDefault;
	}

	public CustomSkinConfig skinConfig;

	public List<GameObject> lovableNPCs = new List<GameObject>();

	public List<GiftCraftable> gifts = new List<GiftCraftable>();

	public List<RelationshipStage> relationshipStages = new List<RelationshipStage>();

	public AbstractSTModule loveModule;

	public GameObject smsReceivedGO;

	public int numberOfDates = 3;

	public bool allowBreakUp = true;

	private const float UPDATE_LEADERBOARD_EVERY = 10f;

	private float timer;

	public bool isTutorialOn;

	public LovedOne lovedOne
	{
		get;
		private set;
	}

	public LoveModule loveSettings => Manager.Get<ModelManager>().loveSettings;

	public override void Init()
	{
		foreach (GameObject lovableNPC in lovableNPCs)
		{
			Manager.Get<AbstractSaveTransformManager>().AddRuntimePrefab(lovableNPC);
		}
		MonoBehaviourSingleton<ProgressCounter>.get.AddCallback(ProgressCounter.Countables.MINIGAMES_PLAYED, OnMinigamePlayed);
	}

	private void OnMinigamePlayed(int count)
	{
		if (lovedOne != null)
		{
			lovedOne.AddLoveValue(loveSettings.GetLovePerMinigame());
		}
	}

	public bool IsTargetLovable(GameObject go)
	{
		return lovableNPCs.Any((GameObject npc) => npc.name == go.name.Replace("(Clone)", string.Empty));
	}

	public bool IsTargetUnLovable(GameObject go)
	{
		return lovedOne != null && !allowBreakUp && lovableNPCs.Any((GameObject npc) => npc.name == go.name.Replace("(Clone)", string.Empty));
	}

	public void TryToDate(GameObject mob)
	{
		if (lovedOne == null)
		{
			Pickup(mob);
		}
		else if (lovedOne.gameObject != mob)
		{
			ShowBreakUpToPickUpPopup(mob);
		}
		else
		{
			TryToDate();
		}
	}

	private void ShowBreakUpToPickUpPopup(GameObject mob)
	{
		Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
		{
			configureMessage = delegate(TranslateText t)
			{
				t.translationKey = "date.lose.partner";
				t.defaultText = "Do you want to date this person? You wlil lose your current partner";
			},
			configureLeftButton = delegate(Button b, TranslateText t)
			{
				b.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
				});
				t.translationKey = "menu.cancel";
				t.defaultText = "Cancel";
			},
			configureRightButton = delegate(Button b, TranslateText t)
			{
				b.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
					BreakUp();
					Pickup(mob);
				});
				t.translationKey = "menu.ok";
				t.defaultText = "Ok";
			}
		});
	}

	public void Pickup(GameObject mob)
	{
		lovedOne = mob.AddComponent<LovedOne>();
		lovedOne.ConfigNewLover();
		Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
		{
			configureMessage = delegate(TranslateText t)
			{
				t.translationKey = "love.start";
				t.defaultText = "You started a relationship!";
			},
			configureLeftButton = delegate(Button b, TranslateText t)
			{
				b.gameObject.SetActive(value: false);
			},
			configureRightButton = delegate(Button b, TranslateText t)
			{
				b.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
					relationshipStages[0].stageSetup.StartRelationshipStage();
				});
				t.translationKey = "menu.ok";
				t.defaultText = "ok";
			}
		});
	}

	public void LoadLover(GameObject go, LovedOne.RelationshipInfo info)
	{
		lovedOne = go.GetComponent<LovedOne>();
		if (lovedOne == null)
		{
			lovedOne = go.AddComponent<LovedOne>();
		}
		lovedOne.LoadData(info);
	}

	public float GetDateCooldown()
	{
		return AutoRefreshingStock.GetNextItemCooldown("dating", loveSettings.GetDateCooldown(), loveSettings.GetMaxDates());
	}

	public void TryToDate()
	{
		int stockCount = AutoRefreshingStock.GetStockCount("dating", loveSettings.GetDateCooldown(), loveSettings.GetMaxDates());
		if (stockCount > 0)
		{
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			lovedOne.Date();
			AutoRefreshingStock.DecrementStockCount("dating");
		}
		else
		{
			ShowNoDateAvaiblePopup();
		}
	}

	private void ShowNoDateAvaiblePopup()
	{
		string text = Manager.Get<TranslationsManager>().GetText("watch.ad.get.dates", "Watch ad to be able to date again!");
		Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
		{
			numberOfAdsNeeded = 1,
			translationKey = string.Empty,
			description = text,
			reason = StatsManager.AdReason.XCRAFT_LOVE_DATE,
			immediatelyAd = false,
			type = AdsCounters.None,
			onSuccess = delegate(bool b)
			{
				if (b)
				{
					AutoRefreshingStock.RefillStock("dating");
				}
			},
			configWatchButton = delegate(GameObject go)
			{
				TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
				componentInChildren.translationKey = "menu.watch";
				componentInChildren.defaultText = "Watch";
				componentInChildren.ForceRefresh();
			}
		});
	}

	public void OpenGifts()
	{
		Manager.Get<StateMachineManager>().PushState<CraftingPopupState>(new CraftingPopupStateStartParameter
		{
			type = typeof(GiftCraftable)
		});
	}

	public void GiveGift(GiftCraftable gift)
	{
		if (!gifts.Contains(gift))
		{
			UnityEngine.Debug.LogError("You need to add this gift " + gift.gameObject.name + " to Love Manager");
		}
		else
		{
			lovedOne.GiveGift(gift);
		}
	}

	public void BreakUp()
	{
		lovedOne.GetComponentInParent<AnimalMob>().logic = AnimalMob.AnimalLogic.WANDER;
		lovedOne.GetComponentInParent<AnimalMob>().ReconstructBehaviourTree();
		HumanMobNavigator componentInChildren = lovedOne.GetComponentInChildren<HumanMobNavigator>();
		if (componentInChildren != null)
		{
			componentInChildren.enableAnimations = true;
		}
		UnityEngine.Object.Destroy(lovedOne);
		lovedOne = null;
	}

	public float GetSmsCooldown()
	{
		return AutoRefreshingStock.GetNextItemCooldown("sms", loveSettings.GetDateCooldown(), loveSettings.GetMaxDates());
	}

	public void TrySendSms()
	{
		if (GetSmsCooldown() == 0f)
		{
			Manager.Get<StateMachineManager>().PushState<SendMessageState>(new SendMessageStartParameter
			{
				onMessageReceived = ReceivedSms
			});
		}
		else
		{
			ShowNoSmsAvaiblePopup();
		}
	}

	private void ShowNoSmsAvaiblePopup()
	{
		string text = Manager.Get<TranslationsManager>().GetText("watch.ad.get.sms", "Watch ad to fill your mobile phone account");
		Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
		{
			numberOfAdsNeeded = 1,
			translationKey = string.Empty,
			description = text,
			reason = StatsManager.AdReason.XCRAFT_LOVE_SMS,
			immediatelyAd = false,
			type = AdsCounters.None,
			onSuccess = delegate(bool b)
			{
				if (b)
				{
					AutoRefreshingStock.RefillStock("sms");
				}
			},
			configWatchButton = delegate(GameObject go)
			{
				TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
				componentInChildren.translationKey = "menu.watch";
				componentInChildren.defaultText = "Watch";
				componentInChildren.ForceRefresh();
			}
		});
	}

	private void ReceivedSms(string t)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(smsReceivedGO);
		gameObject.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.SetAsLastSibling();
		lovedOne.AddLoveValue(loveSettings.GetLovePerTap(lovedOne.info.relationshipStage));
		AutoRefreshingStock.DecrementStockCount("sms");
	}
}
