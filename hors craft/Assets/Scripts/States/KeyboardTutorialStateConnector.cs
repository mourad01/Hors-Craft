// DecompilerFi decompiler from Assembly-CSharp.dll class: States.KeyboardTutorialStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class KeyboardTutorialStateConnector : UIConnector
	{
		public delegate void OnClick();

		public GameObject fragmentPrefab;

		public Transform fragmentParent;

		public Button applyButton;

		public OnClick onApplyButtonClicked;

		private void Awake()
		{
			applyButton.onClick.AddListener(delegate
			{
				if (onApplyButtonClicked != null)
				{
					onApplyButtonClicked();
				}
			});
		}
	}
}
