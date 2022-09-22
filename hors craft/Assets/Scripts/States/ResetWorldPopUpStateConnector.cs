// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ResetWorldPopUpStateConnector
using Common.Managers.States.UI;
using UnityEngine.UI;

namespace States
{
	public class ResetWorldPopUpStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Button returnButton;

		public Button resetButton;

		public OnClick onResetButtonClicked;

		public OnClick onReturnButtonClicked;

		private void Awake()
		{
			resetButton.onClick.AddListener(delegate
			{
				if (onResetButtonClicked != null)
				{
					onResetButtonClicked();
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
	}
}
