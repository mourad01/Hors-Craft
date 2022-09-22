// DecompilerFi decompiler from Assembly-CSharp.dll class: RewardedAdsManager
using Common.Managers;
using Common.Waterfall;
using GameUI;
using States;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardedAdsManager : Manager
{
	private float callbackTimeoutTime;

	private bool waitingForAdCallback;

	private bool tryWaitingForCallback;

	private const float CALLBACK_TIME_OUT = 5f;

	private Action<bool> onSuccess;

	private Action onFail;

	private StatsManager.AdReason adReason;

	public void ShowRewardedAd(StatsManager.AdReason adReason, Action<bool> onSuccess, Action onFail = null, string adTag = "Rewarded")
	{
		this.onSuccess = onSuccess;
		this.onFail = onFail;
		this.adReason = adReason;
		ShowOverlayAndStartWaiting();
		//AdWaterfall.get.ShowRewardedAd(adTag, delegate(bool success)
		
			if (waitingForAdCallback)
			{
				HideOverlayAndEndWaiting();
				//if (success)
				{
					//UnityEngine.Debug.LogError("Watched From Callback");
					//WatchedDesiredAd();
				}
			}
		
	}

	private void Update()
	{
		if (!waitingForAdCallback || !(Time.realtimeSinceStartup > callbackTimeoutTime))
		{
			return;
		}
		if (tryWaitingForCallback)
		{
			tryWaitingForCallback = false;
			return;
		}
		HideOverlayAndEndWaiting();
		if (Application.internetReachability != 0)
		{
			UnityEngine.Debug.LogError("Watched From Timeout");
			WatchedDesiredAd();
			return;
		}
		if (onFail != null)
		{
			onFail();
		}
		OnError();
	}

	private void WatchedDesiredAd()
	{
		onSuccess(obj: true);
		Manager.Get<StatsManager>().AdShownWithReason(adReason);
		Manager.Get<StatsManager>().RewardedAdShown();
	}

	private void ShowOverlayAndStartWaiting()
	{
		callbackTimeoutTime = Time.realtimeSinceStartup + 5f;
		Manager.Get<CommonUIManager>().ShowBlackBackgroundOverlayForAds(show: true);
		waitingForAdCallback = true;
		tryWaitingForCallback = true;
	}

	private void HideOverlayAndEndWaiting()
	{
		waitingForAdCallback = false;
		StartCoroutine(HideBackground());
	}

	private IEnumerator HideBackground()
	{
		yield return new WaitForEndOfFrame();
		Manager.Get<CommonUIManager>().ShowBlackBackgroundOverlayForAds(show: false);
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

	public override void Init()
	{
	}
}
