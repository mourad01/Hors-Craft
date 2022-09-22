// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ResetWorldPopUpState
using Common.Managers;
using Common.Managers.States;

namespace States
{
	public class ResetWorldPopUpState : XCraftUIState<ResetWorldPopUpStateConnector>
	{
		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			ResetWorldPopUpStateStartParameter resetWorldPopUpStateStartParameter = parameter as ResetWorldPopUpStateStartParameter;
			base.connector.onResetButtonClicked = OnReset;
			base.connector.onReturnButtonClicked = OnReturn;
			if (resetWorldPopUpStateStartParameter != null && resetWorldPopUpStateStartParameter.instantTestWorldReset)
			{
				OnReset();
			}
		}

		private void OnReset()
		{
			SavedWorldManager.ResetCurrentWorld(shouldSave: true);
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
