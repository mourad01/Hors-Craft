// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingPopupStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CraftingPopupStateConnector : UIConnector
	{
		public Transform parent;

		public Button returnButton;

		public Vector2 adjustedPositionlLeftBottom;

		public Vector2 adjustedPositionRightTop;

		public Action onReturnButton;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				onReturnButton();
			});
		}

		public void AdjustWindow()
		{
			RectTransform component = parent.GetComponent<RectTransform>();
			component.offsetMin = adjustedPositionlLeftBottom;
			component.offsetMax = adjustedPositionRightTop;
		}
	}
}
