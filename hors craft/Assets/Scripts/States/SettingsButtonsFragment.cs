// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SettingsButtonsFragment
using Common.Managers;
using Gameplay;
using System;
using TsgCommon;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class SettingsButtonsFragment : Fragment
	{
		public Button removeAdsButton;

		public Button restorePurchaseButton;

		public Button fbButton;

		public Button supportButton;

		public Button saveButton;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			InitUI();
			UpdateRemoveAdsComponents();
		}

		private void Update()
		{
			UpdateRemoveAdsComponents();
		}

		private void InitUI()
		{
			InitListener(removeAdsButton, OnRemoveAds);
			InitListener(restorePurchaseButton, OnRestorePurchase);
			InitListener(fbButton, OnFacebook);
			InitListener(supportButton, OnSupport);
			InitListener(saveButton, OnSaveWorld);
			saveButton.gameObject.SetActive(startParameter.pauseStartParameter.canSave);
			fbButton.gameObject.SetActive(Manager.Get<ModelManager>().fb.IsFBLinkEnabled());
		}

		private void InitListener(Button button, Action action)
		{
			button.onClick.AddListener(delegate
			{
				action();
			});
		}

		private void UpdateRemoveAdsComponents()
		{
			removeAdsButton.gameObject.SetActive(Manager.Get<ModelManager>().adsFree.IsAdsFreeButtonEnabled() && !Manager.Get<ModelManager>().modulesContext.isAdsFree);
			TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.UpdateAdsUnlock();
			restorePurchaseButton.gameObject.SetActive(SymbolsHelper.isIAPEnabled && SymbolsHelper.isIOS);
			if (PlayerPrefs.GetInt("debugAdsFree", 0) == 1 || Manager.Get<ModelManager>().worldsSettings.IsThisGameUltimate())
			{
				removeAdsButton.gameObject.SetActive(value: false);
			}
		}

		private void OnRemoveAds()
		{
			TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.TryToUnlockAds();
			Manager.Get<StatsManager>().RemoveAdsClicked();
		}

		private void OnRestorePurchase()
		{
		}

		private void OnFacebook()
		{
			Application.OpenURL(Manager.Get<ModelManager>().fb.GetFBURL());
		}

		private void OnSupport()
		{
			Manager.Get<StateMachineManager>().PushState<SupportState>(new SupportStateStartParameter
			{
				onReturn = delegate
				{
					Manager.Get<StateMachineManager>().PopState();
				}
			});
		}

		private void OnSaveWorld()
		{
			Engine.SaveWorldInstant();
			Manager.Get<GameCallbacksManager>().FrequentSave();
			Manager.Get<GameCallbacksManager>().InFrequentSave();
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureRightButton = delegate(Button button, TranslateText text)
				{
					text.translationKey = "rateus.ok";
					text.defaultText = "ok";
					button.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					b.gameObject.SetActive(value: false);
				},
				configureMessage = delegate(TranslateText text)
				{
					text.translationKey = "menu.savedworld";
					text.defaultText = "world saved";
				}
			});
		}
	}
}
