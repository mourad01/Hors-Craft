// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WatchXAdsPopUpState
using Common.Managers;
using Common.Managers.States;
using Common.Waterfall;
using Gameplay;
using System;
using System.Collections.Generic;
using TsgCommon;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class WatchXAdsPopUpState : XCraftUIState<WatchXAdsPopUpStateConnector>
	{
		public static bool callbackWasFired;

		private bool waitingForAdCallback;

		private float callbackTimeoutTime;

		private const float CALLBACK_TIME_OUT = 5f;

		private AdsCounters type;

		private WatchXAdsPopUpStateStartParameter startParameter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		//protected override bool canShowBanner => false;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			startParameter = (parameter as WatchXAdsPopUpStateStartParameter);
			waitingForAdCallback = false;
			type = startParameter.type;
			if (startParameter.immediatelyAd)
			{
				base.connector.DisableWindowEnableOverlay(disable: true);
				OnUnlock();
				return;
			}
			base.connector.onUnlockButtonClicked = OnUnlock;
			base.connector.onReturnButtonClicked = OnReturn;
			base.connector.onRemoveAdsButtonClicked = OnRemoveAds;
			base.connector.voxelSprites.Clear();
			base.connector.voxelSprites = startParameter.voxelSpritesToUnlock;
			base.connector.popupDescription.defaultText = startParameter.description;
			base.connector.popupDescription.translationKey = startParameter.translationKey;
			base.connector.popupDescription.AddVisitor((string text) => text.Formatted(startParameter.numberOfAdsNeeded.ToString()));
			base.connector.popupDescription.ForceRefresh();
			base.connector.UpdateLockedBlocksSprites();
			startParameter.configWatchButton?.Invoke(base.connector.unlockButton.gameObject);
			startParameter.configCancelButton?.Invoke(base.connector.returnButton.gameObject);
			ConfigureRemoveAdsButton();
		}

		public override void UpdateState()
		{
			base.UpdateState();
			UpdateRemoveAdsComponents();
			if (waitingForAdCallback)
			{
				if (Time.realtimeSinceStartup > callbackTimeoutTime)
				{
					HideOverlayAndEndWaiting();
					Manager.Get<StateMachineManager>().PopState();
					if (Application.internetReachability != 0)
					{
						WatchedDesiredAd();
					}
					else
					{
						OnError();
					}
				}
			}
			else if (startParameter.updateAction != null)
			{
				startParameter.updateAction(base.connector);
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			if (TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.IsAdsFree)
			{
				Manager.Get<StateMachineManager>().PopState();
			}
		}

		/*private void ShowInterstitial()
		{
			if (type == AdsCounters.Clothes)
			{
				if (Manager.Get<ModelManager>().adsFree.IsInterstitialInsteadOfRewarded())
				{
					//AdWaterfall.get.ShowInterstitialAd("Title");
				}
				else
				{
					//AdWaterfall.get.ShowRewardedAd("Title", delegate
					{
					});
				}
			}
			else if (Manager.Get<ModelManager>().adsFree.IsInterstitialInsteadOfRewarded())
			{
				AdWaterfall.get.ShowInterstitialAd("Getblocks");
			}
			else
			{
				AdWaterfall.get.ShowRewardedAd("Getblocks", delegate
				{
				});
			}
			ShowOverlayAndStartWaiting();
		}

		private void ShowRewardedAd(string tag, Action<bool> callback)
		{
			ShowOverlayAndStartWaiting();
			AdWaterfall.get.ShowRewardedAd(tag, delegate(bool success)
			{
				if (Manager.Get<StateMachineManager>().IsCurrentStateA<WatchXAdsPopUpState>() && waitingForAdCallback)
				{
					HideOverlayAndEndWaiting();
					if (success)
					{
						WatchedDesiredAd();
						OnReturn();
					}
				}
			});
		}
		*/
		private void OnUnlock()
		{
			if (!Manager.Get<ModelManager>().commonAdSettings.IsRewardedWithInterstitial())
			{
				//ShowInterstitial();
				return;
			}
			UnityEngine.Debug.Log("Showing rewarded");
			Manager.Get<RewardedAdsManager>().ShowRewardedAd(startParameter.reason, delegate(bool success)
			{
				if (success)
				{
					IncrementAdsCounter();
					startParameter.onSuccess?.Invoke(obj: true);
					OnReturn();
				}
			});
		}

		private void OnRemoveAds()
		{
			TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.TryToUnlockAds();
			Manager.Get<StatsManager>().RemoveAdsClicked();
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		private void ShowOverlayAndStartWaiting()
		{
			base.connector.waitingOverlayGameObject.SetActive(value: true);
			waitingForAdCallback = true;
			callbackTimeoutTime = Time.realtimeSinceStartup + 5f;
		}

		private void HideOverlayAndEndWaiting()
		{
			base.connector.waitingOverlayGameObject.SetActive(value: false);
			waitingForAdCallback = false;
			base.connector.gameObject.SetActive(value: true);
		}

		private void WatchedDesiredAd()
		{
			IncrementAdsCounter();
			if (startParameter.onSuccess != null)
			{
				startParameter.onSuccess(obj: true);
			}
			Manager.Get<StatsManager>().AdShownWithReason(startParameter.reason);
			Manager.Get<StatsManager>().RewardedAdShown();
		}

		private void IncrementAdsCounter()
		{
			List<string> list = new List<string>();
			list.Add("numberOfWatchedRewardedAds");
			if (type == AdsCounters.Clothes)
			{
				list.Add("numberOfWatchedRewardedAdsClothes");
			}
			else if (type == AdsCounters.Blocks)
			{
				list.Add("numberOfWatchedRewardedAdsBlocks");
				list.Add("numberOfWatchedRewardedAds" + startParameter.blockCategory.ToString().ToLower());
			}
			else if (type == AdsCounters.Pets)
			{
				list.Add("numberOfWatchedRewardedAdsPets");
			}
			foreach (string item in list)
			{
				int @int = PlayerPrefs.GetInt(item, 0);
				@int++;
				PlayerPrefs.SetInt(item, @int);
			}
		}

		private void ConfigureRemoveAdsButton()
		{
			TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.UpdateAdsUnlock();
			bool flag = ShouldEnableRemoveAdsButton();
			base.connector.removeAdsButton.gameObject.SetActive(flag);
			base.connector.panelToResize.sizeDelta = new Vector2((!flag) ? 900f : 1000f, 350f);
			if (flag)
			{
				base.connector.pulsingEffect.StartEffect();
			}
		}

		private bool ShouldEnableRemoveAdsButton()
		{
			return (type == AdsCounters.Blocks || type == AdsCounters.Clothes || type == AdsCounters.Pets) && Manager.Get<ModelManager>().adsFree.IsAdsFreeButtonEnabled() && Manager.Get<ModelManager>().adsFree.IsRemoveAdsButtonInWatchXAdsEnabled() && !Manager.Get<ModelManager>().modulesContext.isAdsFree && startParameter.allowRemoveAdsButton;
		}

		private void UpdateRemoveAdsComponents()
		{
			if (ShouldEnableRemoveAdsButton() && TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.UpdateAdsUnlock())
			{
				OnReturn();
			}
		}

		private void OnError()
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "internet.connection.error.body";
					t.defaultText = "Please turn on the Internet connection or move to an area with good reception";
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
					});
					t.translationKey = "menu.ok";
					t.defaultText = "OK :(";
				}
			});
		}
	}
}
