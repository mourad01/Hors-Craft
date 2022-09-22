// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlocksPopupStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class BlocksPopupStateConnector : UIConnector
	{
		public Transform parent;

		public Button returnButton;

		public Vector2 adjustedPositionlLeftBottomOldWindow;

		public Vector2 adjustedPositionRightTopOldWindow;

		public Vector2 adjustedPositionlLeftBottomNewWindow;

		public Vector2 adjustedPositionRightTopNewWindow;

		public Action onReturnButton;

		public float categoriesBarVecticalOffset;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				onReturnButton();
			});
		}

		public void AdjustWindow()
		{
			bool enableRarityBlocks = Singleton<BlocksController>.get.enableRarityBlocks;
			RectTransform component = parent.GetComponent<RectTransform>();
			component.offsetMin = ((!enableRarityBlocks) ? adjustedPositionlLeftBottomOldWindow : adjustedPositionlLeftBottomNewWindow);
			component.offsetMax = ((!enableRarityBlocks) ? adjustedPositionRightTopOldWindow : adjustedPositionRightTopNewWindow);
			if (enableRarityBlocks)
			{
				GetComponentInChildren<TabRarityContainer>().SetBarOffset(categoriesBarVecticalOffset);
			}
		}
	}
}
