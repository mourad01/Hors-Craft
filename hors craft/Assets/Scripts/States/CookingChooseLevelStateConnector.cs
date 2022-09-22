// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingChooseLevelStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CookingChooseLevelStateConnector : UIConnector
	{
		public GameObject ChaptersList;

		public GameObject[] buttonIcons;

		public GameObject[] additionalButtonIcons;

		public Button startButton;

		public Button rankingButton;

		public Button upgradesButton;

		public Button leaveButton;

		public Action onRestockButton;

		public Action onStartButton;

		public Action onRankingButton;

		public Action onUpgradesButton;

		public Action onLeaveButton;

		private void Awake()
		{
			startButton.onClick.AddListener(delegate
			{
				onStartButton();
			});
			rankingButton.onClick.AddListener(delegate
			{
				onRankingButton();
			});
			upgradesButton.onClick.AddListener(delegate
			{
				onUpgradesButton();
			});
			leaveButton.onClick.AddListener(delegate
			{
				onLeaveButton();
			});
		}
	}
}
