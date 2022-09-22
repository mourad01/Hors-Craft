// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AfterTutorialPopUpStateConnector
using Common.Managers.States.UI;
using UnityEngine.UI;

namespace States
{
	public class AfterTutorialPopUpStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Button returnButton;

		public OnClick onReturnButtonClicked;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
		}
	}
}
