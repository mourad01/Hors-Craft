// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FishingReminderState
using Common.Managers;
using Common.Managers.States;

namespace States
{
	public class FishingReminderState : XCraftUIState<FishingReminderStateConnector>
	{
		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onReturnButtonClicked = OnReturn;
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
