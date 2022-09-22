// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TitleStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class TitleStateConnector : UIConnector
	{
		public delegate void OnClicked();

		public Button startButton;

		public GameObject startButtonGameObject;

		public Button seeOtherGamesButton;

		public Button wwwButton;

		public Button removeAdsButton;

		public Button restorePurchasesButton;

		public GameObject restorePurchasesGameObject;

		public Button facebookButton;

		public GameObject facebookGameObject;

		public GameObject adsFreeGameObject;

		public GameObject seeOtherGamesGO;

		public RectTransform[] crosspromoPivots;

		public OnClicked onStartButtonClicked;

		public OnClicked onSeeOtherGamesButtonClicked;

		public OnClicked onAdTestsButtonClicked;

		public OnClicked onRemoveAdsButtonClicked;

		public OnClicked onRestorePurchasesButtonClicked;

		public OnClicked onFacebookButtonClicked;

		protected virtual void Awake()
		{
		}

		public void InitButtons()
		{
			startButton.onClick.AddListener(delegate
			{
				if (onStartButtonClicked != null)
				{
					onStartButtonClicked();
				}
			});
			seeOtherGamesButton.onClick.AddListener(delegate
			{
				if (onSeeOtherGamesButtonClicked != null)
				{
					onSeeOtherGamesButtonClicked();
				}
			});
			removeAdsButton.onClick.AddListener(delegate
			{
				if (onRemoveAdsButtonClicked != null)
				{
					onRemoveAdsButtonClicked();
				}
			});
			restorePurchasesButton.onClick.AddListener(delegate
			{
				if (onRestorePurchasesButtonClicked != null)
				{
					onRestorePurchasesButtonClicked();
				}
			});
			facebookButton.onClick.AddListener(delegate
			{
				if (onFacebookButtonClicked != null)
				{
					onFacebookButtonClicked();
				}
			});
		}
	}
}
