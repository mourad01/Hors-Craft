// DecompilerFi decompiler from Assembly-CSharp.dll class: States.XCraftUIState`1
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Common.Managers.States;
using Common.Managers.States.UI;
using Common.Waterfall;
using Gameplay;
using Gameplay.Audio;
using GameUI;
using UnityEngine;

namespace States
{
	public abstract class XCraftUIState<T> : UIConnectedState<T> where T : UIConnector
	{
		protected class AutoAdsConfig
		{
			public bool autoShowOnStart = true;

			public StatsManager.AdReason properAdReason;
		}

		public bool playSoundOnStart;

		public GameSound playSoundOnStartSound = GameSound.WINDOW_SLIDE_IN;

		private bool shownInterstitial;

		protected virtual bool hasBackground => true;

		protected virtual bool hasBackgroundOverlay => true;

		//protected virtual bool canShowBanner => Manager.Get<ModelManager>().globalBanner.BannerEnabled(AdsContext.StateToStringId(this));

		protected virtual AutoAdsConfig autoAdsConfig => new AutoAdsConfig();

		protected ModelManager modelManager
		{
			get;
			private set;
		}

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			shownInterstitial = false;
			ResetCommonUI();
			T connector = base.connector;
			connector.gameObject.SetActive(value: false);
			T connector2 = base.connector;
			connector2.gameObject.SetActive(value: true);
			if (playSoundOnStart)
			{
				PlayGUISound(playSoundOnStartSound);
			}
			modelManager = Manager.Get<ModelManager>();
			TryShowBanner();
			if (autoAdsConfig.autoShowOnStart)
			{
				//TryToShowAd();
			}
			SetFPSMode(GameMode.Interface);
		}

		protected void SetFPSMode(GameMode mode)
		{
			if (Manager.Get<StatsManager>().TryToGetReporter(out FPSStatReporter reporter))
			{
				reporter.SetGameMode(mode);
			}
		}

		/*ùprotected void TryToShowAd()
		{
			string text = AdsContext.StateToStringId(this);
			bool flag = (!(this is PauseState)) ? Manager.Get<ModelManager>().levelBasedAdRequirements.HasToShowAdForContext(text, 0, 0) : Manager.Get<ModelManager>().timeBasedAdRequirements.HasToShowAdForContext(text, PlayerPrefs.GetInt("timeSinceStartup"));
			if (flag && Manager.Get<ModelManager>().adScoringModule.CanShowAd())
			{
				string adTag = text.ToTitleCase();
				//AdWaterfall.get.ShowInterstitialAd(adTag);
				shownInterstitial = true;
				Manager.Get<StatsManager>().AdShownWithReason(autoAdsConfig.properAdReason);
			}
		}*/

		private void TryShowBanner()
		{
			//if (canShowBanner && modelManager.globalBanner.HasToShowGlobalBanner())
			{
				//AdWaterfall.get.ShowBanner("global.banner");
			}
		}

		public override void UpdateState()
		{
			base.UpdateState();
			//if (!canShowBanner || shownInterstitial || !modelManager.globalBanner.HasToShowGlobalBanner())
			{
				//AdWaterfall.get.HideBanner();
			}
			CheckPolicy(null);
		}

		public override void FreezeState()
		{
			base.FreezeState();
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			pushedPolicyPopup = false;
			ResetCommonUI();
			SetFPSMode(GameMode.Interface);
			TryShowBanner();
		}

		public override void FinishState()
		{
			base.FinishState();
		}

		private void ResetCommonUI()
		{
			CommonUIManager commonUIManager = Manager.Get<CommonUIManager>();
			commonUIManager.SetBackground(hasBackground, hasBackgroundOverlay);
		}

		protected void PlayGUISound(GameSound gameSound)
		{
			Sound sound = new Sound();
			sound.clip = Manager.Get<ClipsManager>().GetClipFor(gameSound);
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}
	}
}
