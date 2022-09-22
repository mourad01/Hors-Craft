// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.FeedbackManager
using Common.Connection;

namespace Common.Managers
{
	public class FeedbackManager : Manager
	{
		private Feedback feedback;

		public override void Init()
		{
			ConnectionInfoManager connectionInfoManager = Manager.Get<ConnectionInfoManager>();
			feedback = new Feedback(connectionInfoManager.gameName, connectionInfoManager.homeURL);
		}

		public void SendFeedback(string content)
		{
			if (!string.IsNullOrEmpty(content))
			{
				StartCoroutine(feedback.SendFeedback(content));
			}
		}

		public void SendFeedbackWithMail(string mail, string content)
		{
			if (!string.IsNullOrEmpty(content))
			{
				if (string.IsNullOrEmpty(mail))
				{
					mail = " ";
				}
				StartCoroutine(feedback.SendFeedbackWithMail(mail, content));
			}
		}
	}
}
