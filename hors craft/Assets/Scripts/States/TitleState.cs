// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TitleState
using Common.Crosspromo;
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Common.Waterfall;
using Gameplay;
using GameUI;
using System;
using TsgCommon;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class TitleState : XCraftUIState<TitleStateConnector>
	{
		private Sprite titleScreenResource;

		private Sprite logoResource;

		public GameObject animatedTitleScreen;

		public bool spawnAnimatedTitleScreenAsRoot;

		public Sprite titleScreen;

		public Sprite logo;

		public Color playButtonColor;

		public Color removeAdsButtonColor;

		public Color sogButtonColor;

		public Color restorePurchcaseColor;

		public Color playFontColor = new Color(0.134f, 0.134f, 0.134f, 1f);

		public Color otherButtonsFontColor = new Color(0.134f, 0.134f, 0.134f, 1f);

		public Color iconsColor = new Color(0.134f, 0.134f, 0.134f, 1f);

		public Color iconsInsideColor = new Color(0.134f, 0.134f, 0.134f, 1f);

		public GameObject seeOtherGamesLayoutCustomButton;

		public GameObject removeAdsLayoutCustomButton;

		public GameObject RestorePurchasesCustomButton;

		public GameObject FacebookLikeCustomButton;

		public GameObject playLayoutCustomButton;

		public Font privacyPolicyTextFont;

		public Action customOnStartAction;

		private TitleStateCommonConnector commonConnector;

		protected override bool hasBackground => true;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = true;
				autoAdsConfig.properAdReason = StatsManager.AdReason.XCRAFT_TITLE;
				return autoAdsConfig;
			}
		}

		public void SetColorsToDefault()
		{
			iconsColor = playButtonColor;
			iconsInsideColor = otherButtonsFontColor;
		}

		public void OnDevMode()
		{
		}

		public override void StartState(StartParameter parameter)
		{
			TimeScaleHelper.value = 0f;
			UpdateIsAdsFree();
			base.StartState(parameter);
			if (Manager.Get<ModelManager>().configSettings.ForceInstantStart())
			{
				OnStart();
				return;
			}
			if (SymbolsHelper.isAndroid)
			{
				titleScreenResource = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Titlescreen");
				logoResource = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Logo");
			}
			else
			{
				titleScreenResource = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Titlescreen_iOS");
				logoResource = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Logo_iOS");
				if (titleScreenResource == null)
				{
					titleScreenResource = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Titlescreen");
				}
				if (logoResource == null)
				{
					logoResource = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Logo");
				}
			}
			logo = logoResource;
			titleScreen = titleScreenResource;
			SetupCustomUI();
			InitListeners();
			InitRestorePurchases();
			UpdateAdsFreeGameObject();
			if (!CheckMultiCrosspromo())
			{
				Manager.Get<CrosspromoManager>().TryToShowIconAt(Manager.Get<CanvasManager>().canvas, this, new Vector2(1f, 0.6f));
			}
			MonoBehaviourSingleton<ChatBotWebData>.get.Initialize(null, null);
			commonConnector = (base.connector as TitleStateCommonConnector);
			if (commonConnector != null)
			{
				if (logo != null)
				{
					commonConnector.logoImage.sprite = logo;
				}
				SetupBackground();
				commonConnector.startButton.GetComponent<Image>().color = playButtonColor;
				commonConnector.removeAdsButton.GetComponent<Image>().color = removeAdsButtonColor;
				commonConnector.seeOtherGamesButton.GetComponent<Image>().color = sogButtonColor;
				commonConnector.restorePurchasesButton.GetComponent<Image>().color = iconsColor;
				commonConnector.facebookButton.GetComponent<Image>().color = iconsColor;
				commonConnector.privacyPolicy.color = sogButtonColor;
				commonConnector.startButton.GetComponentInChildren<Text>().color = playFontColor;
				commonConnector.removeAdsButton.GetComponentInChildren<Text>().color = otherButtonsFontColor;
				commonConnector.seeOtherGamesButton.GetComponentInChildren<Text>().color = otherButtonsFontColor;
				commonConnector.restorePurchasesButton.transform.GetChild(0).GetComponent<Image>().color = iconsInsideColor;
				commonConnector.facebookButton.transform.GetChild(0).GetComponent<Image>().color = iconsInsideColor;
				if (privacyPolicyTextFont != null)
				{
					commonConnector.privacyPolicy.GetComponentInChildren<Text>().font = privacyPolicyTextFont;
				}
			}
			if (Manager.Contains<DailyChestManager>())
			{
				Manager.Get<DailyChestManager>().SpawnIfNeeded(new Vector2(0.4f, 0.185f), hideAfterOpen: false);
			}
			StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.TITLE_SHOWN);
		}

		private bool CheckMultiCrosspromo()
		{
			int multiCrosspromoAmount = Manager.Get<ModelManager>().crosspromoSettings.GetMultiCrosspromoAmount();
			if (multiCrosspromoAmount > 0)
			{
				string[] tags = Manager.Get<ModelManager>().crosspromoSettings.GetMultiCrosspromoCampainTags().ToArray();
				Manager.Get<CrosspromoManager>().TryToShowIconsAt(this, base.connector.crosspromoPivots, tags, multiCrosspromoAmount);
				return true;
			}
			return false;
		}

		private void SetupCustomUI()
		{
			if (seeOtherGamesLayoutCustomButton != null)
			{
				UnityEngine.Object.Destroy(base.connector.seeOtherGamesGO.transform.GetChild(0).gameObject);
				GameObject gameObject = UnityEngine.Object.Instantiate(seeOtherGamesLayoutCustomButton, base.connector.seeOtherGamesGO.transform, worldPositionStays: false);
				base.connector.seeOtherGamesButton = gameObject.GetComponentInChildren<Button>();
			}
			if (removeAdsLayoutCustomButton != null)
			{
				UnityEngine.Object.Destroy(base.connector.adsFreeGameObject.transform.GetChild(0).gameObject);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(removeAdsLayoutCustomButton, base.connector.adsFreeGameObject.transform, worldPositionStays: false);
				base.connector.removeAdsButton = gameObject2.GetComponentInChildren<Button>();
			}
			if (RestorePurchasesCustomButton != null)
			{
				UnityEngine.Object.Destroy(base.connector.restorePurchasesButton.transform.GetChild(0).gameObject);
				GameObject gameObject3 = UnityEngine.Object.Instantiate(RestorePurchasesCustomButton, base.connector.restorePurchasesButton.transform, worldPositionStays: false);
				base.connector.restorePurchasesButton = gameObject3.GetComponentInChildren<Button>();
			}
			if (FacebookLikeCustomButton != null)
			{
				UnityEngine.Object.Destroy(base.connector.facebookGameObject.transform.GetChild(0).gameObject);
				GameObject gameObject4 = UnityEngine.Object.Instantiate(FacebookLikeCustomButton, base.connector.facebookGameObject.transform, worldPositionStays: false);
				base.connector.facebookButton = gameObject4.GetComponentInChildren<Button>();
			}
			if (playLayoutCustomButton != null)
			{
				UnityEngine.Object.Destroy(base.connector.startButtonGameObject.transform.GetChild(0).gameObject);
				GameObject gameObject5 = UnityEngine.Object.Instantiate(playLayoutCustomButton, base.connector.startButtonGameObject.transform, worldPositionStays: false);
				base.connector.startButton = gameObject5.GetComponentInChildren<Button>();
			}
			base.connector.InitButtons();
		}

		private void SetupBackground()
		{
			if (animatedTitleScreen != null)
			{
				commonConnector.titleImage.gameObject.SetActive(value: false);
				if (spawnAnimatedTitleScreenAsRoot)
				{
					commonConnector.animatedTitle.transform.parent = null;
					commonConnector.animatedTitle.transform.localScale = Vector3.one;
					commonConnector.animatedTitle.transform.rotation = Quaternion.identity;
					commonConnector.animatedTitle.transform.position = Vector3.zero;
				}
				commonConnector.animatedTitle.injectPrefab = animatedTitleScreen;
				DisableBackgroudOverlay();
			}
			else
			{
				if (commonConnector.animatedTitle != null)
				{
					commonConnector.animatedTitle.gameObject.SetActive(value: false);
				}
				commonConnector.titleImage.sprite = titleScreen;
			}
		}

		private void DisableBackgroudOverlay()
		{
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
		}

		private void CleanupBackground()
		{
			if (!(commonConnector == null))
			{
				UnityEngine.Object.Destroy(commonConnector.animatedTitle.gameObject);
			}
		}

		private void UpdateIsAdsFree()
		{
			TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.UpdateAdsUnlock();
		}

		private void InitListeners()
		{
			base.connector.onStartButtonClicked = OnStart;
			base.connector.onSeeOtherGamesButtonClicked = OnSeeOtherGames;
			base.connector.onRemoveAdsButtonClicked = OnRemoveAds;
			base.connector.restorePurchasesGameObject.SetActive(value: false);
			base.connector.onAdTestsButtonClicked = delegate
			{
			};
			base.connector.seeOtherGamesGO.SetActive(Manager.Get<ModelManager>().sog.IsSOGEnabled());
			base.connector.facebookGameObject.SetActive(Manager.Get<ModelManager>().fb.IsFBLinkTitleEnabled());
			base.connector.onFacebookButtonClicked = delegate
			{
				Application.OpenURL(Manager.Get<ModelManager>().fb.GetFBTitleURL());
			};
		}

		private void InitRestorePurchases()
		{
		}

		private void UpdateAdsFreeGameObject()
		{
			bool active = Manager.Get<ModelManager>().adsFree.IsAdsFreeButtonEnabled() && !Manager.Get<ModelManager>().modulesContext.isAdsFree;
			base.connector.adsFreeGameObject.SetActive(active);
		}

		public override void UpdateState()
		{
			base.UpdateState();
			UpdateAdsFreeGameObject();
			if (base.connector.adsFreeGameObject.activeSelf)
			{
				UpdateIsAdsFree();
			}
			else
			{
				//AdWaterfall.get.HideBanner();
			}
			bool flag = PlayerPrefs.GetInt("whatsnew.popup.showed." + Application.version, 0) == 1;
			if (Manager.Get<ModelManager>().whatsNewSettings.GetNewFeaturesCount() > 0 && !flag)
			{
				Manager.Get<StateMachineManager>().PushState<WhatsNewPopUpState>();
			}
		}

		public override void FinishState()
		{
			base.FinishState();
			CleanupBackground();
			titleScreen = null;
			logo = null;
			commonConnector = null;
			logoResource = null;
			titleScreenResource = null;
			Resources.UnloadAsset(titleScreenResource);
			Resources.UnloadAsset(logoResource);
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			if (animatedTitleScreen != null)
			{
				DisableBackgroudOverlay();
			}
		}

		private void OnStart()
		{
			StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.TITLE_PLAY_CLICKED);
			if (customOnStartAction != null)
			{
				customOnStartAction();
			}
			else
			{
				Manager.Get<StateMachineManager>().SetState<LoadLevelState>();
			}
		}

		private void OnSeeOtherGames()
		{
			Application.OpenURL(Manager.Get<ModelManager>().sog.GetSOGURL());
			Manager.Get<StatsManager>().SeeOtherGamesClicked();
		}

		private void OnRemoveAds()
		{
			TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.TryToUnlockAds();
			Manager.Get<StatsManager>().RemoveAdsClicked();
			if (TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.IsAdsFree)
			{
				Manager.Get<StatsManager>().AdsRemoved();
			}
		}

		private void OnRestorePurchases()
		{
		}
	}
}
