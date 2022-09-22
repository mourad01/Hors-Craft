// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WatchAdsRemoveAdsStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class WatchAdsRemoveAdsStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Button returnButton;

		public Button watchButton;

		public Text adsCounter;

		public GameObject waitingOverlayGameObject;

		public OnClick onReturnButtonClicked;

		public OnClick onWatchButtonClicked;

		private void Awake()
		{
			watchButton.onClick.AddListener(delegate
			{
				if (onWatchButtonClicked != null)
				{
					onWatchButtonClicked();
				}
			});
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
		}
	}
}
