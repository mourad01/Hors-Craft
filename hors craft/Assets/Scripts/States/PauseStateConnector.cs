// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PauseStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class PauseStateConnector : UIConnector
	{
		public delegate void OnClick();

		public GameObject MenuButtons;

		public Button returnButton;

		public Button wardrobeButton;

		public Transform fragmentParent;

		public OnClick onReturnButtonClicked;

		public OnClick onWardrobeButtonClicked;

		public GameObject notificationPrefab;

		private SpawnerPool notificationPool;

		[HideInInspector]
		public bool showWardrobeButton;

		[HideInInspector]
		public int wardrobeMenuButtonPosition;

		[HideInInspector]
		public bool openWardrobeFirst;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
			notificationPool = new SpawnerPool("pauseNotifications", notificationPrefab);
		}

		public void ShowWardrobe()
		{
			if (wardrobeButton == null)
			{
				return;
			}
			wardrobeButton.gameObject.transform.parent.gameObject.SetActive(value: false);
			if (showWardrobeButton)
			{
				wardrobeButton.gameObject.transform.parent.gameObject.SetActive(value: true);
				wardrobeButton.gameObject.transform.parent.SetSiblingIndex(wardrobeMenuButtonPosition);
				wardrobeButton.onClick.AddListener(delegate
				{
					if (onWardrobeButtonClicked != null)
					{
						onWardrobeButtonClicked();
					}
				});
				if (openWardrobeFirst && onWardrobeButtonClicked != null)
				{
					onWardrobeButtonClicked();
				}
			}
		}

		public void ClearNotifications()
		{
			notificationPool.DespawnAll();
		}

		public void AddNotificationAt(string name)
		{
		}
	}
}
