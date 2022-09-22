// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AfterTutorialPopUpState
using Common.Managers;
using Common.Managers.States;

namespace States
{
	public class AfterTutorialPopUpState : XCraftUIState<AfterTutorialPopUpStateConnector>
	{
		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onReturnButtonClicked = OnReturn;
			Manager.Get<FishingManager>().isTutorialPopupFinished = true;
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_TUTORIAL);
		}
	}
}
