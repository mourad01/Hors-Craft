// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PatientCuredPopupStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class PatientCuredPopupStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Button okButton;

		public Button rewardButton;

		public Text prestigeEarnedText;

		public Text moneyEarnedText;

		public GameObject waitingOverlayGameObject;

		public OnClick onOkButtonClicked;

		public OnClick onRewardButtonClicked;

		private void Awake()
		{
			okButton.onClick.AddListener(delegate
			{
				if (onOkButtonClicked != null)
				{
					onOkButtonClicked();
				}
			});
			rewardButton.onClick.AddListener(delegate
			{
				if (onRewardButtonClicked != null)
				{
					onRewardButtonClicked();
				}
			});
		}
	}
}
