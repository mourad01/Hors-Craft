// DecompilerFi decompiler from Assembly-CSharp.dll class: States.IsometricObjectPlacementStateConnector
using Common.Managers.States.UI;
using GameUI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class IsometricObjectPlacementStateConnector : UIConnector
	{
		public AnalogController analog;

		public SimpleRepeatButton analogButton;

		public SimpleRepeatButton dragButton;

		public Button backButton;

		public Button acceptButton;

		public Button rotateButton;

		public GameObject errorMessage;

		public Action onBackButton;

		public Action onAcceptButton;

		public Action onRotateButton;

		private void Awake()
		{
			backButton.onClick.AddListener(delegate
			{
				onBackButton();
			});
			acceptButton.onClick.AddListener(delegate
			{
				onAcceptButton();
			});
			rotateButton.onClick.AddListener(delegate
			{
				onRotateButton();
			});
		}
	}
}
