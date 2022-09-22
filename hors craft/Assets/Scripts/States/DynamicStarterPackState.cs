// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DynamicStarterPackState
using Common.Managers;
using Common.Managers.States;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class DynamicStarterPackState : StarterPackState
	{
		private DynamicOfferPackManager offerPackManager;

		private StateMachineManager stateMachine;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = false;
				return autoAdsConfig;
			}
		}

		public override void StartState(StartParameter parameter)
		{
			stateMachine = Manager.Get<StateMachineManager>();
			offerPackManager = Manager.Get<DynamicOfferPackManager>();
			offerPackManager.OnStarterPackWillShow();
			base.StartState(parameter);
		}

		protected override void OnBuy(string buyId)
		{
			if (Application.isEditor)
			{
				OnBuySuccess();
			}
		}

		public override void UpdateState()
		{
			base.connector.UpdateClock();
		}

		public override void FinishState()
		{
			base.FinishState();
			offerPackManager.OnStarterPackWasShown();
			UpdateOfferPackButton();
		}

		protected override void OnBuySuccess()
		{
			offerPackManager.GrantCurrentPack();
			offerPackManager.OnStarterPackBuy();
			string itemId = Manager.Get<ConnectionInfoManager>().gameName + ".starterpack";
			Manager.Get<StatsManager>().ItemBought(itemId, 1);
			stateMachine.PopState();
			ShowSuccessPopup();
		}

		private void UpdateOfferPackButton()
		{
			if (offerPackManager.ShouldShowPackButton())
			{
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.OFFERPACK_ENABLED, new OfferPackContext
				{
					onOfferpackButton = OnOfferPackButton,
					sprite = offerPackManager.GetSpriteForButton()
				});
			}
			else
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.OFFERPACK_ENABLED);
			}
		}

		private void OnOfferPackButton()
		{
			offerPackManager.forceOpen = true;
			stateMachine.PushState<PauseState>();
			stateMachine.PushState<DynamicStarterPackState>();
		}

		private void ShowSuccessPopup()
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureTitle = delegate
				{
				},
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "offerpack.popup.success.message";
					t.defaultText = "Offerpack has been granted, thank you for the purchase";
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					t.translationKey = "offerpack.popup.success.button.ok";
					t.defaultText = "ok";
					b.gameObject.SetActive(value: true);
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					b.gameObject.SetActive(value: false);
				}
			});
		}
	}
}
