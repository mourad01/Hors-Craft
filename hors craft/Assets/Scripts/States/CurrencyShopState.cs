// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CurrencyShopState
using Common.Managers;
using Common.Managers.States;
using System;

namespace States
{
	public class CurrencyShopState : XCraftUIState<CurrencyShopConnector>
	{
		public class DataEvent : EventArgs
		{
			private string _data;

			public string data => _data;

			public DataEvent(string data)
			{
				_data = data;
			}
		}

		private bool triedWatchVideoAdd;

		public event EventHandler<DataEvent> onPackageCliked;

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			CurrencyShopParameter currencyShopParameter = startParameter as CurrencyShopParameter;
			Action onReturn = null;
			if (currencyShopParameter != null)
			{
				onReturn = currencyShopParameter.onCancel;
			}
			base.connector.Init(Singleton<UltimateCraftModelDownloader>.get.GetIapPackages(), OnPackageTryToBuy, OnVideo, onReturn);
			OnPackageClikedDelegate();
			if (currencyShopParameter == null)
			{
				SetTitleText(noMoney: false);
			}
			else
			{
				SetTitleText(currencyShopParameter.noMoneyResponse);
			}
		}

		private void OnPackageClikedDelegate()
		{
			this.onPackageCliked = null;
		}

		private void SetTitleText(bool noMoney)
		{
			string translationKey = (!noMoney) ? "shop.soft.title" : "shop.soft.title.noMoney";
			string defaultText = (!noMoney) ? "SHOP" : "NOT ENOUGH MONEY";
			base.connector.title.GetComponent<TranslateText>().translationKey = translationKey;
			base.connector.title.GetComponent<TranslateText>().defaultText = defaultText;
			base.connector.title.GetComponent<TranslateText>().ForceRefresh();
		}

		private void OnPackageTryToBuy(string iap)
		{
			this.onPackageCliked(this, new DataEvent(iap));
		}

		private void OnVideo()
		{
			if (Singleton<PlayerData>.get.playerPurchases.CanWatchAdd())
			{
				triedWatchVideoAdd = true;
				Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
				{
					reason = StatsManager.AdReason.XCRAFT_CURRENCY,
					immediatelyAd = true,
					onSuccess = OnTryToViewVideo
				});
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			if (!WatchXAdsPopUpState.callbackWasFired && triedWatchVideoAdd)
			{
				Manager.Get<StateMachineManager>().PushState<GenericPopupState>(GenericPopupStateStartParameter.OnAdShowError(OnCancelVideo, OnVideoRetry));
			}
			triedWatchVideoAdd = false;
		}

		private void OnTryToViewVideo(bool success)
		{
			if (success)
			{
				Manager.Get<StateMachineManager>().PopStatesUntil<CurrencyShopState>();
				int currencyCount = Singleton<UltimateCraftModelDownloader>.get.GetIapPackages().Find((PackageData package) => package.isCostVideo).currencyCount;
				Singleton<PlayerData>.get.playerPurchases.ViewVideoForCurrency(currencyCount);
			}
			else
			{
				Manager.Get<StateMachineManager>().PushState<GenericPopupState>(GenericPopupStateStartParameter.OnInternetError(OnCancel, OnVideoRetry));
			}
		}

		private void OnCancel()
		{
			if (WorldsFragment.areWorldsFromPause)
			{
				Manager.Get<StateMachineManager>().PopStatesUntil<PauseState>();
			}
			else
			{
				Manager.Get<StateMachineManager>().PopStatesUntil<WorldShopState>();
			}
		}

		private void OnCancelVideo()
		{
			Manager.Get<StateMachineManager>().PopStatesUntil<CurrencyShopState>();
		}

		private void OnVideoRetry()
		{
			Manager.Get<StateMachineManager>().PopStatesUntil<CurrencyShopState>();
			OnVideo();
		}
	}
}
