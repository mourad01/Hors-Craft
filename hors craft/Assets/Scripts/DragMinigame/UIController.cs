// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.UIController
using UnityEngine;
using UnityEngine.UI;

namespace DragMinigame
{
	public class UIController : MonoBehaviour
	{
		[SerializeField]
		protected Text countdownText;

		[SerializeField]
		protected TranslateText shiftRatingTranslateText;

		[SerializeField]
		protected Button exitButton;

		[SerializeField]
		private Text tutorialText;

		[SerializeField]
		private RectTransform tutorialWindowTransform;

		protected Text shiftRatingText;

		protected DragGameManager gameManager;

		private RectTransform tutorialTextTransform;

		public virtual void Init()
		{
			gameManager = GetComponent<DragGameManager>();
			shiftRatingText = shiftRatingTranslateText.GetComponent<Text>();
			tutorialTextTransform = tutorialText.GetComponent<RectTransform>();
			exitButton.onClick.AddListener(delegate
			{
				gameManager.ExitGame();
			});
		}

		public void StartCountDown()
		{
			countdownText.gameObject.SetActive(value: true);
		}

		public void UpdateCountDown(int time)
		{
			countdownText.text = time.ToString();
		}

		public void StopCountDown()
		{
			countdownText.gameObject.SetActive(value: false);
		}

		public virtual void ShowSwitchInfo(DragPlayerController.ShiftRating gearSwitch)
		{
			switch (gearSwitch)
			{
			case DragPlayerController.ShiftRating.PERFECT:
				SetText(shiftRatingTranslateText, shiftRatingText, "perfect", "Perfect!", Color.green);
				break;
			case DragPlayerController.ShiftRating.GOOD:
				SetText(shiftRatingTranslateText, shiftRatingText, "good", "Good", Color.yellow);
				break;
			case DragPlayerController.ShiftRating.BAD:
				SetText(shiftRatingTranslateText, shiftRatingText, "bad", "Bad", Color.red);
				break;
			}
		}

		protected virtual void SetText(TranslateText translateText, Text textComponent, string key, string defaultText, Color color)
		{
			translateText.translationKey = key;
			translateText.defaultText = defaultText;
			translateText.ForceRefresh();
			textComponent.color = color;
		}

		public void SetTutorialUI(TutorialController.TutorialStep step)
		{
			tutorialWindowTransform.gameObject.SetActive(value: true);
			tutorialWindowTransform.position = step.anchor.position;
			Vector3 localScale = tutorialWindowTransform.localScale;
			localScale.x = ((!step.flipX) ? localScale.x : (localScale.x * -1f));
			tutorialWindowTransform.localScale = localScale;
			Vector3 localScale2 = tutorialTextTransform.localScale;
			localScale2.x = ((!step.flipX) ? localScale2.x : (localScale2.x * -1f));
			tutorialTextTransform.localScale = localScale2;
		}

		public void SetTutorialText(string key, string defaultText)
		{
			tutorialText.text = defaultText;
		}

		public virtual void UpdateShiftUI(int currentShift)
		{
		}

		public void HideTutorial()
		{
			tutorialWindowTransform.gameObject.SetActive(value: false);
		}
	}
}
