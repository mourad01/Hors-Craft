// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragRacingUIController
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DragMinigame
{
	public class DragRacingUIController : UIController
	{
		[SerializeField]
		private Text currentShiftText;

		[SerializeField]
		private Text timerText;

		[SerializeField]
		private GameObject aspectRatioChanger;

		[SerializeField]
		private CanvasGroup dashboardGroup;

		[SerializeField]
		private Image fadePanel;

		[SerializeField]
		private Animator shiftfRatingAnimator;

		[SerializeField]
		private TranslateText shiftTitleTranslateText;

		[SerializeField]
		private Button skipIntroButton;

		private Text shiftTitleText;

		private float endRatingXPosition;

		private bool isFading;

		private DragRacingGameManager dragRacingmanager;

		public override void Init()
		{
			base.Init();
			dragRacingmanager = (gameManager as DragRacingGameManager);
			shiftTitleText = shiftTitleTranslateText.GetComponent<Text>();
			skipIntroButton.onClick.AddListener(delegate
			{
				SkipIntro();
			});
		}

		private void SkipIntro()
		{
			dragRacingmanager.SkipIntro();
		}

		public void SetCurrentShiftText(int currentShift)
		{
			currentShiftText.text = currentShift.ToString();
		}

		public void UpdateTimer(float raceTime)
		{
			timerText.text = raceTime.ToString("0.00");
		}

		public void FadeOutIn(float fadeTime = 1f, bool startFadeFromFullAlpha = true)
		{
			if (!isFading)
			{
				StartCoroutine(FadeOutInCO(fadePanel, fadeTime, startFadeFromFullAlpha));
			}
		}

		public void FadeInDashBoard()
		{
			aspectRatioChanger.SetActive(value: false);
			dashboardGroup.alpha = 1f;
		}

		private IEnumerator FadeOutInCO(Image image, float time, bool startFadeFromFullAlpha)
		{
			image.gameObject.SetActive(value: true);
			if (!startFadeFromFullAlpha)
			{
				yield return StartCoroutine(FadeIn(image, time));
			}
			else
			{
				Color color = image.color;
				float r = color.r;
				Color color2 = image.color;
				float g = color2.g;
				Color color3 = image.color;
				image.color = new Color(r, g, color3.b, 1f);
			}
			float div = startFadeFromFullAlpha ? 1 : 2;
			yield return StartCoroutine(FadeOut(image, time / div));
			fadePanel.gameObject.SetActive(value: false);
			isFading = false;
		}

		private IEnumerator FadeIn(Image image, float time)
		{
			Color originalColor = image.color;
			while (true)
			{
				Color color = image.color;
				if (color.a < 1f)
				{
					Color color2 = image.color;
					float a = color2.a;
					image.color = new Color(a: a + Time.deltaTime / time, r: originalColor.r, g: originalColor.g, b: originalColor.b);
					yield return null;
					continue;
				}
				break;
			}
		}

		private IEnumerator FadeOut(Image image, float time)
		{
			Color originalColor = image.color;
			while (true)
			{
				Color color = image.color;
				if (color.a > 0f)
				{
					Color color2 = image.color;
					float a = color2.a;
					image.color = new Color(a: a - Time.deltaTime / time, r: originalColor.r, g: originalColor.g, b: originalColor.b);
					yield return null;
					continue;
				}
				break;
			}
		}

		private IEnumerator FadeIn(CanvasGroup group, float time)
		{
			while (group.alpha < 1f)
			{
				float alpha = group.alpha;
				group.alpha += Time.deltaTime / time;
				yield return null;
			}
		}

		public override void UpdateShiftUI(int currentTransfer)
		{
			currentShiftText.text = currentTransfer.ToString();
		}

		public override void ShowSwitchInfo(DragPlayerController.ShiftRating gearSwitch)
		{
			base.ShowSwitchInfo(gearSwitch);
			Color color = Color.green;
			switch (gearSwitch)
			{
			case DragPlayerController.ShiftRating.PERFECT:
				color = Color.green;
				break;
			case DragPlayerController.ShiftRating.GOOD:
				color = Color.yellow;
				break;
			case DragPlayerController.ShiftRating.BAD:
				color = Color.red;
				break;
			}
			SetText(shiftTitleTranslateText, shiftTitleText, "shift", "Shift", color);
			shiftfRatingAnimator.Play("ShiftInfoAnim", 0, 0f);
		}

		internal void DisableSkipIntro()
		{
			skipIntroButton.gameObject.SetActive(value: false);
		}

		public void ShowRecord()
		{
			SetText(shiftRatingTranslateText, shiftRatingText, "new", "NEW", Color.red);
			SetText(shiftTitleTranslateText, shiftTitleText, "record", "RECORD!", Color.red);
			shiftfRatingAnimator.Play("ShiftInfoAnim", 0, 0f);
		}
	}
}
