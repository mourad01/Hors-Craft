// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingAfterStageStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CookingAfterStageStateConnector : UIConnector
	{
		public Text level;

		public Text todaysGoal;

		public GameObject goalCompletedGO;

		public GameObject goalFailedGO;

		public Text earnings;

		public Text goalBonusGold;

		public Text goalBonusPrestige;

		public GameObject[] stars;

		public Button okButton;

		public Action onOkButton;

		private int maxNumberOfStars = 3;

		private void Awake()
		{
			okButton.onClick.AddListener(delegate
			{
				onOkButton();
			});
		}

		public void SetStars(int gainedStars)
		{
			for (int i = 1; i <= maxNumberOfStars; i++)
			{
				if (i <= gainedStars)
				{
					stars[i - 1].SetActive(value: true);
				}
				else
				{
					stars[i - 1].SetActive(value: false);
				}
			}
		}
	}
}
