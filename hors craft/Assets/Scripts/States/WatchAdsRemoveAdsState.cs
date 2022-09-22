// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WatchAdsRemoveAdsState
using Common.Managers;
using Common.Managers.States;
using Common.Waterfall;
using Gameplay;
using TsgCommon;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class WatchAdsRemoveAdsState : XCraftUIState<WatchAdsRemoveAdsStateConnector>
	{
		private const float CALLBACK_TIME_OUT = 5f;

		private const string adsLeftKey = "ads.adsleft";

		private int debugAdsLeftToWatch = 1;

		private bool waitingForAdCallback;

		private float callbackTimeoutTime;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		//protected override bool canShowBanner => false;

		private int adsLeftToWatch
		{
			get
			{
				if (PlayerPrefs.GetInt("debugOneAdForTen", 0) == 1)
				{
					return debugAdsLeftToWatch;
				}
				return PlayerPrefs.GetInt("ads.adsleft", Manager.Get<ModelManager>().adsFree.GetAdsToWatchToRemoveAds());
			}
			set
			{
				if (PlayerPrefs.GetInt("debugOneAdForTen", 0) == 1)
				{
					debugAdsLeftToWatch = value;
				}
				PlayerPrefs.SetInt("ads.adsleft", value);
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onWatchButtonClicked = OnWatch;
			base.connector.onReturnButtonClicked = OnReturn;
			UpdateAdsCounter();
			waitingForAdCallback = false;
		}

		private void OnWatch()
		{
			if (Manager.Get<ModelManager>().adsFree.IsInterstitialInsteadOfRewarded() && !Manager.Get<ModelManager>().commonAdSettings.IsRewardedWithInterstitial())
			{
				//AdWaterfall.get.ShowInterstitialAd("Title");
				ShowOverlayAndStartWaiting();
			}
			else
			{
				UnityEngine.Debug.Log("Showing rewarded remove ads");
				Manager.Get<RewardedAdsManager>().ShowRewardedAd(StatsManager.AdReason.XCRAFT_REMOVEADS, delegate(bool success)
				{
					if (success)
					{
						int @int = PlayerPrefs.GetInt("numberOfWatchedRewardedAds");
						@int++;
						PlayerPrefs.SetInt("numberOfWatchedRewardedAds", @int);
						adsLeftToWatch--;
						if (adsLeftToWatch <= 0)
						{
							AdsRemoved();
						}
						else
						{
							UpdateAdsCounter();
						}
					}
				});
			}
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (waitingForAdCallback && Time.realtimeSinceStartup > callbackTimeoutTime)
			{
				HideOverlayAndEndWaiting();
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

		private void RewardedAdShowCallback(bool ok)
		{
			if (Manager.Get<StateMachineManager>().IsCurrentStateA<WatchAdsRemoveAdsState>() && waitingForAdCallback)
			{
				HideOverlayAndEndWaiting();
				if (ok)
				{
					WatchedDesiredAd();
				}
			}
		}

		private void UpdateAdsCounter()
		{
			TranslateText component = base.connector.adsCounter.GetComponent<TranslateText>();
			component.AddVisitor(delegate(string text)
			{
				text = text.Replace("{0}", adsLeftToWatch.ToString());
				return text;
			});
		}

		private void WatchedDesiredAd()
		{
			TrackAdStats();
			adsLeftToWatch--;
			if (adsLeftToWatch <= 0)
			{
				AdsRemoved();
			}
			else
			{
				UpdateAdsCounter();
			}
		}

		private void TrackAdStats()
		{
			Manager.Get<StatsManager>().RewardedAdShown();
			int @int = PlayerPrefs.GetInt("numberOfWatchedRewardedAds");
			@int++;
			PlayerPrefs.SetInt("numberOfWatchedRewardedAds", @int);
			Manager.Get<StatsManager>().AdShownWithReason(StatsManager.AdReason.XCRAFT_REMOVEADS);
		}

		private void AdsRemoved()
		{
			TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.SetNoAds(newState: true);
			Manager.Get<StateMachineManager>().PopState();
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
