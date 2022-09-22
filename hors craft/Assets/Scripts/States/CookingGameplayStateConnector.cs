// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CookingGameplayStateConnector
using Common.Managers.States.UI;
using GameUI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CookingGameplayStateConnector : UIConnector
	{
		public Text clientsLeft;

		public Text timeLeft;

		public Text currentEarnings;

		public Text goalEarnings;

		public Slider specialProgressSlider;

		public GameObject ActivateOnSliderFill;

		public AudioSource audioSource;

		public Image borderSprite;

		public SimpleRepeatButton inputButton;

		public Button pauseButton;

		public Action onPauseButton;

		private void Awake()
		{
			pauseButton.onClick.AddListener(delegate
			{
				onPauseButton();
			});
		}
	}
}
