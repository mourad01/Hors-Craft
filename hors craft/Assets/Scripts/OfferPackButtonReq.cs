// DecompilerFi decompiler from Assembly-CSharp.dll class: OfferPackButtonReq
using Common.Managers;
using GameUI;
using States;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Offerpack Button")]
public class OfferPackButtonReq : InitialPopupRequirements
{
	private OfferpackStarterTimerContext starterTimerContext;

	public override bool CanBeShown()
	{
		OfferPackManager offerPackManager = Manager.Get<OfferPackManager>();
		ShowButton(offerPackManager);
		if (offerPackManager is DynamicOfferPackManager)
		{
			return false;
		}
		if (offerPackManager.IsStarterOfferActive() || offerPackManager.offerEnabled)
		{
			if (starterTimerContext == null)
			{
				starterTimerContext = new OfferpackStarterTimerContext
				{
					color = GetStarerTimerColor(offerPackManager),
					time = GetTimer(offerPackManager),
					onClick = GetAction(offerPackManager)
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.OFFERPACK_ENABLED, starterTimerContext);
			}
		}
		else
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.OFFERPACK_ENABLED, starterTimerContext);
		}
		int @int = PlayerPrefs.GetInt("scaledTimeStartup", 0);
		return offerPackManager.ShouldShowStarterPack(@int) || offerPackManager.ShouldShowOfferPack(@int);
	}

	private void ShowButton(OfferPackManager offerManager)
	{
		if (offerManager.ShouldShowPackButton())
		{
			DynamicOfferPackManager dynamicOfferPackManager = offerManager as DynamicOfferPackManager;
			MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.OFFERPACK_ENABLED, new OfferPackContext
			{
				onOfferpackButton = OnOfferPackButton,
				sprite = dynamicOfferPackManager.GetSpriteForButton()
			});
		}
		else
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.OFFERPACK_ENABLED);
		}
	}

	private void OnOfferPackButton()
	{
		Manager.Get<DynamicOfferPackManager>().forceOpen = true;
		Manager.Get<StateMachineManager>().PushState<PauseState>();
		Manager.Get<StateMachineManager>().PushState<DynamicStarterPackState>();
	}

	private Color GetStarerTimerColor(OfferPackManager offerMan)
	{
		if (offerMan.IsStarterOfferActive())
		{
			return Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.SC_COLOR);
		}
		return Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.MAIN_COLOR);
	}

	private string GetTimer(OfferPackManager offerMan)
	{
		if (offerMan.IsStarterOfferActive())
		{
			return offerMan.GetTimeToEndStarterPack();
		}
		return offerMan.GetTimeToEndOfferPack();
	}

	private Action GetAction(OfferPackManager offerMan)
	{
		if (offerMan.IsStarterOfferActive())
		{
			return delegate
			{
				Manager.Get<StateMachineManager>().PushState<StarterPackState>();
			};
		}
		return delegate
		{
			Manager.Get<StateMachineManager>().PushState<OfferPackState>();
		};
	}
}
