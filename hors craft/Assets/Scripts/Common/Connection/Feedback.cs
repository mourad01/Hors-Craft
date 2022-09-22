// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Connection.Feedback
using System.Collections;
using UnityEngine;

namespace Common.Connection
{
	public class Feedback
	{
		private string gamename;

		private string homeURL;

		public Feedback(string gamename, string homeURL)
		{
			this.gamename = gamename;
			this.homeURL = homeURL;
		}

		public IEnumerator SendFeedback(string feedback)
		{
			WWWForm form = FormFactory.CreateBasicWWWForm();
			form.AddField("mail_content", feedback);
			yield return new WWW(homeURL + "feedback/sendFeedback", form);
		}

		public IEnumerator SendFeedbackWithMail(string mail, string feedback)
		{
			WWWForm form = FormFactory.CreateBasicWWWForm();
			form.AddField("mail", mail);
			form.AddField("mail_content", feedback);
			UnityEngine.Debug.Log("sending feedback with mail: " + mail + " " + feedback);
			yield return new WWW(homeURL + "feedback/sendFeedbackWithMail", form);
		}
	}
}
