// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ChangePetStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ChangePetStateConnector : UIConnector
	{
		public delegate void OnClick();

		public GameObject changePetContainer;

		public Button applyButton;

		public GameObject changePetPrefab;

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

		private void Start()
		{
			GetComponentInChildren<CustomizationFragment>().currencyTracker.SetActive(value: false);
		}
	}
}
