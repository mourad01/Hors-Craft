// DecompilerFi decompiler from Assembly-CSharp.dll class: States.OfferPackState
using Common.Managers;
using Common.Managers.States;
using UnityEngine;

namespace States
{
	public class OfferPackState : XCraftUIState<OfferPackStateConnector>
	{
		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			OfferPackDefinition currentOffer = Manager.Get<OfferPackManager>().GetCurrentOffer();
			base.connector.Init(currentOffer, delegate
			{
				OnBuy("offerpack");
			});
		}

		protected virtual void OnBuy(string buyId)
		{
			if (Application.isEditor)
			{
				OnBuySuccess();
			}
		}

		public override void FinishState()
		{
			Manager.Get<OfferPackManager>().OnOfferPackShown();
			base.FinishState();
		}

		protected virtual void OnBuySuccess()
		{
			Manager.Get<OfferPackManager>().GrantCurrentPack();
			string text = Manager.Get<ConnectionInfoManager>().gameName + ".offerpack";
			Manager.Get<StatsManager>().ItemBought(text, 1);
			UnityEngine.Debug.Log("Buy success: " + text);
		}
	}
}
