// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SupportState
using Common.Managers;
using Common.Managers.States;
using Gameplay;

namespace States
{
	public class SupportState : XCraftUIState<SupportStateConnector>
	{
		private SupportStateStartParameter startParameter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			startParameter = (parameter as SupportStateStartParameter);
			base.connector.onSendButtonClicked = OnSend;
			base.connector.onReturnButtonClicked = OnReturn;
			if (startParameter != null && startParameter.noEmailMode)
			{
				base.connector.SetNoEmailMode();
			}
		}

		private bool CanSend()
		{
			if (startParameter.noEmailMode && CheckFeedback(base.connector.yourMessage.text))
			{
				return true;
			}
			if (!CheckEmail(base.connector.yourEmail.text))
			{
				NoEmailAnimation();
				return false;
			}
			if (!CheckFeedback(base.connector.yourMessage.text))
			{
				NoFeedbackAnimation();
				return false;
			}
			return true;
		}

		private bool CheckFeedback(string msg)
		{
			return !string.IsNullOrEmpty(msg);
		}

		private void NoEmailAnimation()
		{
			base.connector.InvalidEmailAnimation();
		}

		private void NoFeedbackAnimation()
		{
			base.connector.InvalidFeedbackAnimation();
		}

		private bool CheckEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return false;
			}
			return email.Contains("@");
		}

		private void OnSend(string mail, string feedback)
		{
			if (CanSend())
			{
				Manager.Get<FeedbackManager>().SendFeedbackWithMail(mail, feedback);
				Manager.Get<StateMachineManager>().PopState();
				if (startParameter != null && startParameter.noEmailMode)
				{
					Manager.Get<ModelManager>().timeBasedRateUs.RatedAlready();
				}
			}
		}

		private void OnReturn()
		{
			if (startParameter != null && startParameter.onReturn != null)
			{
				startParameter.onReturn();
			}
		}
	}
}
