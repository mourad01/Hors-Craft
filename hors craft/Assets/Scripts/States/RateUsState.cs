// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RateUsState
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using UnityEngine;

namespace States
{
	public class RateUsState : XCraftUIState<RateUsStateConnector>
	{
		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onApplyButtonClicked = OnApply;
			base.connector.onReturnButtonClicked = OnReturn;
		}

		private void OnApply(int starNo)
		{
			if (starNo <= 4)
			{
				Manager.Get<StateMachineManager>().SetState<SupportState>(new SupportStateStartParameter
				{
					onReturn = delegate
					{
						Manager.Get<StateMachineManager>().SetState<RateUsState>();
					},
					noEmailMode = true
				});
				return;
			}
			Application.OpenURL("market://details?id=" + Application.identifier);
			Manager.Get<ModelManager>().timeBasedRateUs.RatedAlready();
			Manager.Get<StateMachineManager>().PopState();
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
