// DecompilerFi decompiler from Assembly-CSharp.dll class: AdUnlockHandler
using Common.Managers;
using Gameplay;
using States;
using TsgCommon;
using UnityEngine;

public class AdUnlockHandler : TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>
{
	public const string KEY_ADS_REMOVED = "ads.removed";

	public EPaymentsMethod paymentMethod = EPaymentsMethod.unlockForAds;

	public bool IsAdsFree
	{
		get
		{
			return Manager.Get<ModelManager>().modulesContext.isAdsFree;
		}
		private set
		{
			Manager.Get<ModelManager>().modulesContext.isAdsFree = value;
		}
	}

	public override void Init()
	{
		base.Init();
		UpdateAdsUnlock();
	}

	public bool UpdateAdsUnlock()
	{
		if (DebugAds())
		{
			return true;
		}
		bool flag = false;
		return IsAdsFree = (IsAdsFreeEnabledInPlayerPrefs() || flag);
	}

	private bool IsAdsFreeEnabledInPlayerPrefs()
	{
		return PlayerPrefs.GetInt("ads.removed", 0) == 1;
	}

	protected bool DebugAds()
	{
		if (PlayerPrefs.GetInt("debugAdsFree", 0) == 1)
		{
			IsAdsFree = true;
			return true;
		}
		return false;
	}

	public void SetNoAds(bool newState)
	{
		IsAdsFree = newState;
		PlayerPrefs.SetInt("ads.removed", newState ? 1 : 0);
		PlayerPrefs.Save();
	}

	public void TryToUnlockAds()
	{
		UnityEngine.Debug.Log("AdUnlockHandler | TryToUnlockAds");
		TrySwitchUnlockTypeToWatchAds();
		switch (paymentMethod)
		{
		case EPaymentsMethod.unlockForAds:
			if (Manager.Get<ModelManager>().adsFree.IsWatchAdsToRemoveAdsEnabled())
			{
				Manager.Get<StateMachineManager>().PushState<WatchAdsRemoveAdsState>();
			}
			else
			{
				OpenUrl();
			}
			break;
		case EPaymentsMethod.simpleIaps:
			RemoveForCash();
			break;
		}
	}

	protected void RemoveForCash()
	{
	}

	protected void OpenUrl()
	{
		Application.OpenURL("market://details?id=" + Manager.Get<ModelManager>().adsFree.GetAndroidAdsFreeAppURL());
	}

	private void TrySwitchUnlockTypeToWatchAds()
	{
		if (Manager.Contains<ModelManager>() && Manager.Get<ModelManager>().adsFree.IsForceUnlockByWatchAdsEnabled())
		{
			paymentMethod = EPaymentsMethod.unlockForAds;
			UnityEngine.Debug.Log("AdUnlockHandler | ForceUnlockByWatchAdsEnabled");
		}
	}
}
