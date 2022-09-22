// DecompilerFi decompiler from Assembly-CSharp.dll class: States.MultiPageTutorialStateConnector
using Common.Managers.States.UI;
using UnityEngine.UI;

namespace States
{
	public class MultiPageTutorialStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Image image;

		public Text text;

		public Button leftButton;

		public Button rightButton;

		public OnClick onLeftButtonClicked;

		public OnClick onRightButtonClicked;

		private void Awake()
		{
			leftButton.onClick.AddListener(delegate
			{
				onLeftButtonClicked();
			});
			rightButton.onClick.AddListener(delegate
			{
				onRightButtonClicked();
			});
		}
	}
}
