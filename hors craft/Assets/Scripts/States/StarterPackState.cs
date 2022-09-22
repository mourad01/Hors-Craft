// DecompilerFi decompiler from Assembly-CSharp.dll class: States.StarterPackState
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using UnityEngine;

namespace States
{
	public class StarterPackState : XCraftUIState<StarterPackStateConnector>
	{
		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			OfferPackDefinition starterPack = Manager.Get<OfferPackManager>().GetStarterPack();
			base.connector.Init(starterPack, Manager.Get<OfferPackManager>().GetStarterPackGender(), delegate
			{
				OnBuy("starterpack");
			});
		}

		public override void FinishState()
		{
			base.connector.UnloadBackground();
			Manager.Get<OfferPackManager>().OnStarterShow();
			base.FinishState();
		}

		protected virtual void OnBuy(string buyId)
		{
			if (Application.isEditor)
			{
				OnBuySuccess();
			}
		}

		protected virtual void OnBuySuccess()
		{
			OfferPackDefinition activeStarterPack = Manager.Get<ModelManager>().offerPackSettings.GetActiveStarterPack();
			Manager.Get<OfferPackManager>().GrantPack(activeStarterPack);
			Manager.Get<OfferPackManager>().OnStarterPackBuy();
			string text = Manager.Get<ConnectionInfoManager>().gameName + ".starterpack_" + Manager.Get<OfferPackManager>().GetStarterPackGender();
			Manager.Get<StatsManager>().ItemBought(text, 1);
			UnityEngine.Debug.Log("Buy success: " + text);
		}
	}
}
