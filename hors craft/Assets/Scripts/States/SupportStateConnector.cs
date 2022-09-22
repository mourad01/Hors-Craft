// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SupportStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class SupportStateConnector : UIConnector
	{
		public delegate void OnSendButtonClick(string mail, string feedback);

		public delegate void OnClick();

		public InputField yourEmail;

		public InputField yourMessage;

		public Button returnButton;

		public Button sendButton;

		public OnSendButtonClick onSendButtonClicked;

		public OnClick onReturnButtonClicked;

		private void Awake()
		{
			sendButton.onClick.AddListener(delegate
			{
				if (onSendButtonClicked != null)
				{
					onSendButtonClicked(yourEmail.text, yourMessage.text);
				}
			});
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
		}

		public void SetNoEmailMode()
		{
			yourEmail.transform.parent.gameObject.SetActive(value: false);
		}

		public void InvalidEmailAnimation()
		{
			InvalidAnimation(yourEmail.gameObject, "InvalidMail");
		}

		public void InvalidFeedbackAnimation()
		{
			InvalidAnimation(yourMessage.gameObject, "InvalidMsg");
		}

		private void InvalidAnimation(GameObject obj, string trigger)
		{
			Animator componentInParent = obj.GetComponentInParent<Animator>();
			if (componentInParent != null)
			{
				componentInParent.SetTrigger(trigger);
			}
		}
	}
}
