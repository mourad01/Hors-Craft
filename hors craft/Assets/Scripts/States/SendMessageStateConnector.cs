// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SendMessageStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine.UI;

namespace States
{
	public class SendMessageStateConnector : UIConnector
	{
		public InputField message;

		public Button sendButton;

		public Button returnButton;

		public Action<string> onSendButton;

		public Action onReturnButton;

		private void Awake()
		{
			sendButton.onClick.AddListener(delegate
			{
				onSendButton(message.text);
			});
			returnButton.onClick.AddListener(delegate
			{
				onReturnButton();
			});
		}
	}
}
