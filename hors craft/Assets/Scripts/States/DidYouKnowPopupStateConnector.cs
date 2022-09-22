// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DidYouKnowPopupStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class DidYouKnowPopupStateConnector : UIConnector
	{
		public TranslateText featureDescriptionText;

		public TranslateText goToFeatureText;

		[Space]
		public Image playButtonImage;

		public Button playButton;

		public Button cancelButton;

		public Button nextHintButton;

		public Button goToButton;

		public Button okButton;

		public Action onCancel;

		public Action onNextHint;

		public Action onGoTo;

		public Action onPlay;

		public void Init(int didYouKnowIndex, bool nextHintEnabled, bool goToEnabled)
		{
			nextHintButton.gameObject.SetActive(nextHintEnabled);
			goToButton.gameObject.SetActive(goToEnabled);
			okButton.gameObject.SetActive(!nextHintEnabled && !goToEnabled);
			cancelButton.gameObject.SetActive(nextHintEnabled || goToEnabled);
			InitTexts(didYouKnowIndex);
			InitButtons();
		}

		private void InitTexts(int didYouKnowIndex)
		{
			string format = "didyouknow.{0}.featuredescription";
			featureDescriptionText.translationKey = string.Format(format, didYouKnowIndex);
			featureDescriptionText.ForceRefresh();
			string format2 = "didyouknow.{0}.gotofeature";
			goToFeatureText.translationKey = string.Format(format2, didYouKnowIndex);
			goToFeatureText.ForceRefresh();
		}

		private void InitButtons()
		{
			playButton.onClick.AddListener(delegate
			{
				if (onPlay != null)
				{
					onPlay();
				}
			});
			goToButton.onClick.AddListener(delegate
			{
				if (onGoTo != null)
				{
					onGoTo();
				}
			});
			nextHintButton.onClick.AddListener(delegate
			{
				if (onNextHint != null)
				{
					onNextHint();
				}
			});
			cancelButton.onClick.AddListener(delegate
			{
				if (onCancel != null)
				{
					onCancel();
				}
			});
			okButton.onClick.AddListener(delegate
			{
				if (onCancel != null)
				{
					onCancel();
				}
			});
		}
	}
}
