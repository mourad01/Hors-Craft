// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingPauseState
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Cooking;

namespace States
{
	public class CookingPauseState : XCraftUIState<CookingPauseStateConnector>
	{
		private WorkController workController;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onResumeButton = OnResume;
			base.connector.onLeaveButton = OnLeave;
			workController = Manager.Get<CookingManager>().workController;
			TimeScaleHelper.value = 0f;
			base.connector.transform.SetAsLastSibling();
		}

		public override void FinishState()
		{
			TimeScaleHelper.value = 1f;
			base.FinishState();
		}

		private void OnResume()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		private void OnLeave()
		{
			workController.LeaveWave();
			Manager.Get<StateMachineManager>().PopStatesUntil<CookingGameplayState>();
			Manager.Get<StateMachineManager>().SetState<CookingChooseLevelState>();
		}
	}
}
