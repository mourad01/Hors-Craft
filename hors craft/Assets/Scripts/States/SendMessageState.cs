// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SendMessageState
using Common.Managers;
using Common.Managers.States;

namespace States
{
	public class SendMessageState : XCraftUIState<SendMessageStateConnector>
	{
		private SendMessageStartParameter startParameter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			startParameter = (parameter as SendMessageStartParameter);
			base.connector.onSendButton = OnSend;
			base.connector.onReturnButton = OnReturn;
		}

		private void OnSend(string message)
		{
			Manager.Get<StateMachineManager>().PopState();
			startParameter.onMessageReceived(message);
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
