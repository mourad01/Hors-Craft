// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingPauseStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine.UI;

namespace States
{
	public class CookingPauseStateConnector : UIConnector
	{
		public Button resumeButton;

		public Button leaveButton;

		public Action onResumeButton;

		public Action onLeaveButton;

		private void Awake()
		{
			resumeButton.onClick.AddListener(delegate
			{
				onResumeButton();
			});
			leaveButton.onClick.AddListener(delegate
			{
				onLeaveButton();
			});
		}
	}
}
