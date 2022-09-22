// DecompilerFi decompiler from Assembly-CSharp.dll class: States.GameOverStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class GameOverStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Text motivationalQuote;

		public Text motivationalQuoteAuthor;

		public GameObject motivationalPanel;

		public GameObject titlePanel;

		public Button showLeaderboardButton;

		public Button returnToMenuButton;

		public Button loadLastSaveButton;

		public Button restartGameButton;

		public OnClick onShowLeaderboardClicked;

		public OnClick onReturnToMenuClicked;

		public OnClick onRestartGameClicked;

		public OnClick onLoadLastSaveClicked;

		public TranslateText survived;

		private void Awake()
		{
			if (Manager.Contains<SocialPlatformManager>() && Manager.Get<SocialPlatformManager>().social.isLoggedIn)
			{
				showLeaderboardButton.gameObject.SetActive(value: true);
			}
			else
			{
				showLeaderboardButton.gameObject.SetActive(value: false);
			}
			showLeaderboardButton.onClick.AddListener(delegate
			{
				if (onShowLeaderboardClicked != null)
				{
					onShowLeaderboardClicked();
				}
			});
			returnToMenuButton.onClick.AddListener(delegate
			{
				if (onReturnToMenuClicked != null)
				{
					onReturnToMenuClicked();
				}
			});
			loadLastSaveButton.onClick.AddListener(delegate
			{
				if (onLoadLastSaveClicked != null)
				{
					onLoadLastSaveClicked();
				}
			});
			restartGameButton.onClick.AddListener(delegate
			{
				if (onRestartGameClicked != null)
				{
					onRestartGameClicked();
				}
			});
		}
	}
}
