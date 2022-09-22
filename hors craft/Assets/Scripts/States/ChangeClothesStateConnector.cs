// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ChangeClothesStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ChangeClothesStateConnector : UIConnector
	{
		public delegate void OnClick();

		public GameObject changeClothesContainer;

		public Button applyButton;

		public GameObject changeClothesPrefab;

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
			bool active = Manager.Get<ModelManager>().clothesSetting.GetUnlockType() == ItemsUnlockModel.SoftCurrency;
			GetComponentInChildren<CustomizationFragment>().currencyTracker.SetActive(active);
		}
	}
}
