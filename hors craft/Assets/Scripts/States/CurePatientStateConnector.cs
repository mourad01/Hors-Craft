// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CurePatientStateConnector
using Common.Managers.States.UI;
using GameUI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CurePatientStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Button returnButton;

		public Button actionCureButton;

		public SimpleRepeatButton fightCureButton;

		public Slider miniGameSlider;

		public Slider progressSlider;

		public Animator miniGamePanel;

		public RectTransform miniGameMedArea;

		public RectTransform miniGameMedHandle;

		public RectTransform miniGameVirusArea;

		public RectTransform miniGameVirusHandle;

		public UIPulsingEffect pullPulse;

		public Image barBackgroud;

		private Image miniGameMedHandleImage;

		[Header("Tutorial Objects")]
		public Sprite MedNearSprite;

		public Sprite MedFarSprite;

		public OnClick onReturnButtonClicked;

		public OnClick onCureButtonClicked;

		public float addingSpeed;

		public float losingSpeed;

		public bool changeBarColor;

		public Color barNeutral;

		public Color barGood;

		public Color barBad;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
			actionCureButton.onClick.AddListener(delegate
			{
				if (onCureButtonClicked != null)
				{
					UnityEngine.Debug.LogError("actioncure clicked");
					onCureButtonClicked();
				}
			});
			miniGameMedHandleImage = miniGameMedHandle.GetComponent<Image>();
			if (barBackgroud != null && changeBarColor)
			{
				barBackgroud.color = barNeutral;
			}
		}

		public void SetMedAreaColor()
		{
			Vector3 localPosition = miniGameMedHandle.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = miniGameVirusHandle.localPosition;
			float num = Mathf.Abs(x - localPosition2.x);
			Vector2 sizeDelta = miniGameVirusHandle.sizeDelta;
			float num2 = sizeDelta.x / 2f;
			Vector2 sizeDelta2 = miniGameMedHandle.sizeDelta;
			float num3 = num2 + sizeDelta2.x / 2f;
			if (num < num3)
			{
				progressSlider.value += addingSpeed;
				MedNearVirusColor();
			}
			else
			{
				progressSlider.value -= losingSpeed;
				MedFarVirusColor();
			}
		}

		public void SetVirusWidth(float width)
		{
			RectTransform rectTransform = miniGameVirusArea;
			float x = width / 2f;
			Vector2 offsetMin = miniGameVirusArea.offsetMin;
			rectTransform.offsetMin = new Vector2(x, offsetMin.y);
			RectTransform rectTransform2 = miniGameVirusArea;
			float x2 = (0f - width) / 2f;
			Vector2 offsetMax = miniGameVirusArea.offsetMax;
			rectTransform2.offsetMax = new Vector2(x2, offsetMax.y);
			RectTransform rectTransform3 = miniGameVirusHandle;
			Vector2 sizeDelta = miniGameVirusHandle.sizeDelta;
			rectTransform3.sizeDelta = new Vector2(width, sizeDelta.y);
		}

		public void SetMedWidth(float medWidth, float virusIndex)
		{
			float num = medWidth - (virusIndex - 2f) * 20f;
			RectTransform rectTransform = miniGameMedArea;
			float x = num / 2f + 13f;
			Vector2 offsetMin = miniGameMedArea.offsetMin;
			rectTransform.offsetMin = new Vector2(x, offsetMin.y);
			RectTransform rectTransform2 = miniGameMedArea;
			float x2 = (0f - num) / 2f - 13f;
			Vector2 offsetMax = miniGameMedArea.offsetMax;
			rectTransform2.offsetMax = new Vector2(x2, offsetMax.y);
			RectTransform rectTransform3 = miniGameMedHandle;
			float x3 = num;
			Vector2 sizeDelta = miniGameMedHandle.sizeDelta;
			rectTransform3.sizeDelta = new Vector2(x3, sizeDelta.y);
		}

		public void MedNearVirusColor()
		{
			if (barBackgroud != null && changeBarColor)
			{
				barBackgroud.color = barGood;
			}
			miniGameMedHandleImage.sprite = MedNearSprite;
		}

		public void MedFarVirusColor()
		{
			if (barBackgroud != null && changeBarColor)
			{
				barBackgroud.color = barBad;
			}
			miniGameMedHandleImage.sprite = MedFarSprite;
		}

		public void SetVirusPivotPosition(float x)
		{
			RectTransform rectTransform = miniGameVirusHandle;
			Vector2 anchorMin = miniGameVirusHandle.anchorMin;
			rectTransform.anchorMin = new Vector2(x, anchorMin.y);
			RectTransform rectTransform2 = miniGameVirusHandle;
			Vector2 anchorMax = miniGameVirusHandle.anchorMax;
			rectTransform2.anchorMax = new Vector2(x, anchorMax.y);
		}
	}
}
