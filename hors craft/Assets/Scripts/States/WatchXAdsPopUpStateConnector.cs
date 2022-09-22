// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WatchXAdsPopUpStateConnector
using Common.Managers.States.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class WatchXAdsPopUpStateConnector : UIConnector
	{
		public delegate void OnClick();

		public TranslateText popupDescription;

		public Button returnButton;

		public Button unlockButton;

		public Button removeAdsButton;

		public GameObject waitingOverlayGameObject;

		public GameObject blocksToUnlockContainer;

		public GameObject blockToUnlockPrefab;

		public GameObject mainWindow;

		public RectTransform panelToResize;

		public UIPulsingEffect pulsingEffect;

		public OnClick onReturnButtonClicked;

		public OnClick onUnlockButtonClicked;

		public OnClick onRemoveAdsButtonClicked;

		[HideInInspector]
		public List<Sprite> voxelSprites = new List<Sprite>();

		private void Awake()
		{
			voxelSprites.Clear();
			unlockButton.onClick.AddListener(delegate
			{
				if (onUnlockButtonClicked != null)
				{
					onUnlockButtonClicked();
				}
			});
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
			removeAdsButton.onClick.AddListener(delegate
			{
				if (onRemoveAdsButtonClicked != null)
				{
					onRemoveAdsButtonClicked();
				}
			});
			UpdateLockedBlocksSprites();
		}

		public void UpdateLockedBlocksSprites()
		{
			if (voxelSprites != null)
			{
				foreach (Sprite voxelSprite in voxelSprites)
				{
					GameObject gameObject = Object.Instantiate(blockToUnlockPrefab);
					gameObject.transform.SetParent(blocksToUnlockContainer.transform, worldPositionStays: false);
					gameObject.transform.SetAsLastSibling();
					gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>()
						.sprite = voxelSprite;
						gameObject.transform.GetChild(1).gameObject.SetActive(value: false);
					}
				}
			}

			public void DisableWindowEnableOverlay(bool disable)
			{
				waitingOverlayGameObject.SetActive(disable);
				mainWindow.SetActive(!disable);
			}
		}
	}
