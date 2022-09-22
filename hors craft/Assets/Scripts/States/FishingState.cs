// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FishingState
using com.ootii.Cameras;
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Gameplay;
using Uniblocks;
using UnityEngine;

namespace States
{
	public class FishingState : XCraftUIState<FishingStateConnector>
	{
		public float fishTargetValue;

		public float rodTargetValue;

		public float fishCurValue;

		public float rodCurValue;

		public Sprite fisherTutorSprite;

		private int fishId;

		private int rodId;

		private TopNotification _notificationInstance;

		public TopNotification notificationPrefab;

		protected FishingManager fishingManager;

		protected FishingModule fishingSettings;

		private bool previousWasPull;

		private float seconds;

		private float time;

		private Fish debugFish;

		private bool tutorial2Showed;

		private bool tutorial3Showed;

		private bool tutorial6Showed;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = false;
				autoAdsConfig.properAdReason = StatsManager.AdReason.XCRAFT_FISHING;
				return autoAdsConfig;
			}
		}

		private TopNotification notificationInstance
		{
			get
			{
				if (_notificationInstance == null && notificationPrefab != null)
				{
					_notificationInstance = Object.Instantiate(notificationPrefab.gameObject).GetComponent<TopNotification>();
					_notificationInstance.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
				}
				return _notificationInstance;
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			PrepeareFishing(parameter);
		}

		protected virtual void PrepeareFishing(StartParameter parameter)
		{
			base.connector.SetTutorialFisherTutorSprite(fisherTutorSprite);
			TimeScaleHelper.value = 1f;
			fishingSettings = Manager.Get<ModelManager>().fishingSettings;
			base.connector.miniGamePanel.updateMode = AnimatorUpdateMode.UnscaledTime;
			base.connector.resourceGatherSpot.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
			base.connector.onReturnButtonClicked = OnReturn;
			base.connector.onFishingButtonClicked = OnFishing;
			fishingManager = Manager.Get<FishingManager>();
			fishingManager.playerFishingController.StartFishing();
			fishingManager.fishingConnector = base.connector;
			base.connector.debugObjectsController.SetActive(fishingManager.debugMode);
			base.connector.onPrevChangeFishClicked = OnPrevFish;
			base.connector.onNextChangeFishClicked = OnNextFish;
			base.connector.onPrevChangeRodClicked = OnPrevRod;
			base.connector.onNextChangeRodClicked = OnNextRod;
			base.connector.fightFishingButton.gameObject.SetActive(value: false);
			fishTargetValue = (fishCurValue = base.connector.miniGameSlider.maxValue * 0.8f);
			Vector3 eulerAngles = CameraController.instance.MainCamera.transform.parent.rotation.eulerAngles;
			eulerAngles.x = 10f;
			CameraController.instance.MainCamera.transform.parent.rotation = Quaternion.Euler(eulerAngles);
			if (!fishingManager.isTutorialFinished)
			{
				base.connector.SetTutorialSlider();
				base.connector.onTutorialMoveNextButton2Clicked = OnTutorialState2;
				base.connector.onTutorialMoveNextButton3Clicked = OnTutorialState3;
				base.connector.onTutorialMoveNextButton4Clicked = OnTutorialState4;
				base.connector.onTutorialMoveNextButton5Clicked = OnTutorialState5;
				base.connector.onTutorialMoveNextButton6Clicked = OnTutorialState6;
				base.connector.onTutorialMoveNextButton7Clicked = OnTutorialState7;
				base.connector.tutorialContent.SetActive(value: true);
			}
			else
			{
				base.connector.tutorialContent.SetActive(value: false);
			}
			OnFishing();
		}

		private void UpdateDebugObjects()
		{
			base.connector.fishOnHookCodeName.text = fishingManager.fishConfigOnHook.codeName;
			base.connector.fishOnHookName.text = fishingManager.fishConfigOnHook.name;
			base.connector.fishOnHookRarity.text = fishingManager.fishConfigOnHook.rarity.ToString();
			base.connector.fishOnHookWeight.text = fishingManager.fishConfigOnHook.catchedWeight.ToString();
			base.connector.fishOnHookPoints.text = fishingManager.fishConfigOnHook.catchedPoints.ToString();
			time = fishingManager.waitingTimeLeft;
			seconds = time % 60f;
			base.connector.leftToSuccess.text = "{0}".Formatted(seconds.ToString("F2"));
			debugFish = fishingManager.GetFishById(fishId);
			base.connector.choosedFishName.text = debugFish.name;
			base.connector.choosedFishRarity.text = "Rarity {0}".Formatted(debugFish.rarity);
			base.connector.choosedRodName.text = fishingManager.rodConfigCurrent.name;
		}

		protected void OnPrevFish()
		{
			if (fishId <= 0)
			{
				fishId = 0;
			}
			else
			{
				fishId--;
			}
			fishingManager.SetFishId(fishId);
		}

		protected void OnNextFish()
		{
			if (fishId >= fishingManager.fishesConfig.Length - 1)
			{
				fishId = fishingManager.fishesConfig.Length - 1;
			}
			else
			{
				fishId++;
			}
			fishingManager.SetFishId(fishId);
		}

		protected void OnPrevRod()
		{
			if (rodId <= 0)
			{
				rodId = 0;
			}
			else
			{
				rodId--;
			}
			fishingManager.SetRodConfig(rodId);
		}

		protected void OnNextRod()
		{
			if (rodId >= fishingManager.rodsConfig.Length - 1)
			{
				rodId = fishingManager.rodsConfig.Length - 1;
			}
			else
			{
				rodId++;
			}
			fishingManager.SetRodConfig(rodId);
		}

		public override void UpdateState()
		{
			if (!fishingManager.isTutorialFinished && fishingManager.fishFighting)
			{
				ShowTutorial();
			}
			if (!fishingManager.isTutorialFinished && base.connector.tutorialMoveNextButton4.pressed)
			{
				OnTutorialState4();
				OnFishing();
			}
			if (fishingManager.tutorialPause)
			{
				return;
			}
			if (fishingManager.fishFighting)
			{
				base.connector.miniGamePanel.SetBool("PanelOn", value: true);
				if (!fishingManager.isTutorialFinished)
				{
					base.connector.SetRodWidth(fishingManager.rodConfigCurrent.baseWidth * 1.3f, fishingManager.fishConfigOnHook.rarity);
				}
				else if (fishingManager.hookUpgrade)
				{
					base.connector.SetRodWidth(fishingManager.rodConfigCurrent.baseWidth * fishingManager.hookUpgradeValue, fishingManager.fishConfigOnHook.rarity);
				}
				else
				{
					base.connector.SetRodWidth(fishingManager.rodConfigCurrent.baseWidth, fishingManager.fishConfigOnHook.rarity);
				}
			}
			else
			{
				base.connector.miniGamePanel.SetBool("PanelOn", value: false);
			}
			if (base.connector.fightFishingButton.pressed)
			{
				OnFishing();
			}
			if (fishingManager.debugMode)
			{
				UpdateDebugObjects();
			}
			UpdateFishingStatus();
			if (fishingManager.fishFighting)
			{
				base.connector.fightFishingButton.gameObject.SetActive(value: true);
				if (fishCurValue == fishTargetValue)
				{
					fishTargetValue = fishCurValue + Random.Range(-0.3f, 0.3f);
					fishTargetValue = Mathf.Clamp01(fishTargetValue);
				}
				fishCurValue = Mathf.MoveTowards(fishCurValue, fishTargetValue, Time.unscaledDeltaTime * (0.1f * (float)fishingManager.fishConfigOnHook.rarity / (float)fishingManager.rodConfigCurrent.quality));
				rodTargetValue -= 0.05f;
				rodTargetValue = Mathf.Clamp01(rodTargetValue);
				rodCurValue = Mathf.MoveTowards(rodCurValue, rodTargetValue, Time.unscaledDeltaTime * 0.45f);
				base.connector.miniGameSlider.value = rodCurValue;
				base.connector.SetFishPivotPosition(fishCurValue);
				base.connector.SetRodAreaColor();
			}
		}

		protected void UpdateFishingStatus()
		{
			bool flag = false;
			if (!fishingManager.fishOnHook)
			{
				if (fishingManager.fishCatched)
				{
					base.connector.fishingButtonText.translationKey = fishingSettings.keyFishingStatusThrow();
					base.connector.fishingButtonText.defaultText = fishingSettings.FishingStatusThrow();
					base.connector.fishingAreaButtonText.translationKey = fishingSettings.keyFishingStatusThrow();
					base.connector.fishingAreaButtonText.defaultText = fishingSettings.FishingStatusThrow();
					if (fishingManager.fishConfigOnHook.rarity == 1)
					{
						base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusSuccessTrash();
						base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusSuccessTrash();
					}
					else
					{
						base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusSuccess();
						base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusSuccess();
					}
				}
				else if (fishingManager.fishBreaks)
				{
					base.connector.fishingButtonText.translationKey = fishingSettings.keyFishingStatusThrow();
					base.connector.fishingButtonText.defaultText = fishingSettings.FishingStatusThrow();
					base.connector.fishingAreaButtonText.translationKey = fishingSettings.keyFishingStatusThrow();
					base.connector.fishingAreaButtonText.defaultText = fishingSettings.FishingStatusThrow();
					if (fishingManager.fishConfigOnHook.rarity == 1)
					{
						base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusBreakTrash();
						base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusBreakTrash();
					}
					else
					{
						base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusBreak();
						base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusBreak();
					}
				}
				else
				{
					base.connector.fishingButtonText.translationKey = fishingSettings.keyFishingStatusWait();
					base.connector.fishingButtonText.defaultText = fishingSettings.FishingStatusWait();
					base.connector.fishingAreaButtonText.translationKey = fishingSettings.keyFishingStatusWait();
					base.connector.fishingAreaButtonText.defaultText = fishingSettings.FishingStatusWait();
					base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusWait();
					base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusWait();
				}
			}
			else if (fishingManager.fishFighting)
			{
				base.connector.fishingButtonText.translationKey = fishingSettings.keyFishingStatusFight();
				base.connector.fishingButtonText.defaultText = fishingSettings.FishingStatusFight();
				base.connector.fishingAreaButtonText.translationKey = fishingSettings.keyFishingStatusFight();
				base.connector.fishingAreaButtonText.defaultText = fishingSettings.FishingStatusFight();
				base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusFight();
				base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusFight();
			}
			else if (fishingManager.fishCatched)
			{
				base.connector.fishingButtonText.translationKey = fishingSettings.keyFishingStatusThrow();
				base.connector.fishingButtonText.defaultText = fishingSettings.FishingStatusThrow();
				base.connector.fishingAreaButtonText.translationKey = fishingSettings.keyFishingStatusThrow();
				base.connector.fishingAreaButtonText.defaultText = fishingSettings.FishingStatusThrow();
				if (fishingManager.fishConfigOnHook.rarity == 1)
				{
					base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusSuccessTrash();
					base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusSuccessTrash();
				}
				else
				{
					base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusSuccess();
					base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusSuccess();
				}
			}
			else if (fishingManager.fishBreaks)
			{
				base.connector.fishingButtonText.translationKey = fishingSettings.keyFishingStatusThrow();
				base.connector.fishingButtonText.defaultText = fishingSettings.FishingStatusThrow();
				base.connector.fishingAreaButtonText.translationKey = fishingSettings.keyFishingStatusThrow();
				base.connector.fishingAreaButtonText.defaultText = fishingSettings.FishingStatusThrow();
				base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusBreak();
				base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusBreak();
			}
			else
			{
				if (!fishingManager.isTutorialFinished)
				{
					ShowTutorial();
				}
				if (!previousWasPull)
				{
					base.connector.pullPulse.StartEffect();
				}
				flag = true;
				base.connector.fishingButtonText.translationKey = fishingSettings.keyFishingStatusPull();
				base.connector.fishingButtonText.defaultText = fishingSettings.FishingStatusPull();
				base.connector.fishingAreaButtonText.translationKey = fishingSettings.keyFishingStatusPull();
				base.connector.fishingAreaButtonText.defaultText = fishingSettings.FishingStatusPull();
				base.connector.fishingStateText.translationKey = fishingSettings.keyFishingStatusPull();
				base.connector.fishingStateText.defaultText = fishingSettings.FishingStatusPull();
			}
			if (!fishingManager.isTutorialFinished)
			{
				if (fishingManager.fishOnHook)
				{
					if (time <= 0f && !tutorial2Showed)
					{
						base.connector.tutorialPopup2.SetActive(value: true);
						fishingManager.tutorialPause = true;
						tutorial2Showed = true;
					}
				}
				else if (fishingManager.fishCatched && !tutorial6Showed)
				{
					tutorial6Showed = true;
					base.connector.tutorialPopup5.SetActive(value: false);
					base.connector.tutorialPopup6.SetActive(value: true);
					fishingManager.tutorialPause = true;
				}
			}
			if (!flag)
			{
				base.connector.pullPulse.StopEffect();
			}
			base.connector.fishingButtonText.ForceRefresh();
			base.connector.fishingAreaButtonText.ForceRefresh();
			base.connector.fishingStateText.ForceRefresh();
			previousWasPull = flag;
		}

		public override void FreezeState()
		{
			HideNotificationImmediately();
			HideBlockAnimationImmiediately();
			if (CameraController.instance.Anchor != null)
			{
				PlayerMovement component = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
				if (component != null)
				{
					component.EnableMovement(enable: false);
				}
			}
			base.FreezeState();
		}

		private void HideNotificationImmediately()
		{
			if (notificationInstance != null)
			{
				notificationInstance.HideImmediately();
			}
		}

		private void HideBlockAnimationImmiediately()
		{
			if (base.connector.resourceGatherSpot != null)
			{
				base.connector.resourceGatherSpot.HideImmediately();
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
		}

		public override void FinishState()
		{
			base.FinishState();
		}

		public virtual void OnReturn()
		{
			fishingManager.EndFishing();
			HideNotificationImmediately();
			HideBlockAnimationImmiediately();
			Manager.Get<StateMachineManager>().PopState();
			if (!fishingManager.isTutorialPopupFinished && Manager.Get<StateMachineManager>().ContainsState(typeof(AfterTutorialPopUpState)))
			{
				Manager.Get<StateMachineManager>().PushState<AfterTutorialPopUpState>();
			}
		}

		public virtual void OnFishing()
		{
			fishingManager.FishingButtonClicked();
			if (fishingManager.fishFighting)
			{
				rodTargetValue += 0.08f;
			}
		}

		private void FishBreaks()
		{
		}

		public void OnResourceGathered(int resourceId)
		{
			if (base.connector.resourceGatherSpot != null || !Manager.Get<ModelManager>().craftingSettings.IsCraftingEnabled())
			{
				base.connector.resourceGatherSpot.Show(new ResourceGatheredAnimator.ResourceInformation(resourceId));
			}
		}

		public void ShowFishNotification(string text, Sprite icon)
		{
			if (!(notificationInstance == null) && Manager.Get<ModelManager>().fishingSettings.IsFishingEnabled())
			{
				FishingNotification.CollectionUnlockedInformation collectionUnlockedInformation = new FishingNotification.CollectionUnlockedInformation();
				collectionUnlockedInformation.information = text;
				collectionUnlockedInformation.icon = icon;
				collectionUnlockedInformation.setOnTop = true;
				collectionUnlockedInformation.timeToHide = 3f;
				FishingNotification.CollectionUnlockedInformation information = collectionUnlockedInformation;
				notificationInstance.Show(information);
				notificationInstance.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
				Vector3 position = base.connector.returnButton.transform.position;
				float y = position.y;
				notificationInstance.gameObject.transform.SetPositionY(y);
			}
		}

		private void ShowTutorial()
		{
			if (base.connector.miniGamePanel.GetCurrentAnimatorStateInfo(0).IsName("PanelIdle") && !tutorial3Showed)
			{
				fishingManager.tutorialPause = true;
				base.connector.tutorialPopup3.SetActive(value: true);
				tutorial3Showed = true;
			}
		}

		private void OnTutorialState2()
		{
			base.connector.tutorialOverlay.raycastTarget = false;
			OnFishing();
			base.connector.tutorialPopup2.SetActive(value: false);
			fishingManager.tutorialPause = false;
		}

		private void OnTutorialState3()
		{
			base.connector.tutorialOverlay.raycastTarget = true;
			base.connector.tutorialPopup3.SetActive(value: false);
			base.connector.tutorialPopup4.SetActive(value: true);
		}

		private void OnTutorialState4()
		{
			base.connector.tutorialOverlay.raycastTarget = false;
			fishingManager.tutorialPause = false;
		}

		private void OnTutorialState5()
		{
			base.connector.tutorialPopup5.SetActive(value: false);
			fishingManager.tutorialPause = false;
		}

		private void OnTutorialState6()
		{
			base.connector.tutorialPopup6.SetActive(value: false);
			base.connector.tutorialPopup7.SetActive(value: true);
		}

		private void OnTutorialState7()
		{
			base.connector.tutorialPopup7.SetActive(value: false);
			base.connector.tutorialContent.SetActive(value: false);
			fishingManager.isTutorialFinished = true;
			fishingManager.tutorialPause = false;
		}
	}
}
