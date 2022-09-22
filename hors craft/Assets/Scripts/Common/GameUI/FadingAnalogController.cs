// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.GameUI.FadingAnalogController
using UnityEngine;
using UnityEngine.UI;

namespace Common.GameUI
{
	public class FadingAnalogController : AnalogController
	{
		private const string ANALOG_TOUCHED = "analog.touched";

		public Image fadingBackground;

		public RectTransform gaugeRect;

		public float backgroundTransparency = 0.5f;

		public bool allowMove;

		private GameObject currentAnimation;

		public bool analogTouched
		{
			get
			{
				return PlayerPrefs.GetInt("analog.touched", 0) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("analog.touched", value ? 1 : 0);
			}
		}

		private void Awake()
		{
			SetFingerOffset(Vector2.zero);
			ChangeBackgroundColor((!analogTouched) ? 1f : 0f);
		}

		public override void OnTouchStart(Vector3 touchPos)
		{
			analogTouched = true;
			if (currentAnimation != null)
			{
				DestroyUtil.DestroyIfNotNull(currentAnimation);
			}
			currentAnimation = FloatAnimator.CreateNewAnimation(ChangeBackgroundColor, 0f, 1f, 0.2f, null);
			if (allowMove)
			{
				gaugeRect.transform.position = new Vector3(touchPos.x, touchPos.y, 0f);
				SetGaugeCenterPositon();
			}
		}

		public override void OnTouchEnd()
		{
			if (currentAnimation != null)
			{
				DestroyUtil.DestroyIfNotNull(currentAnimation);
			}
			currentAnimation = FloatAnimator.CreateNewAnimation(ChangeBackgroundColor, 1f, 0f, 0.2f, null);
		}

		public override void OnTouchContinue()
		{
			UpdateAnalogControllerPosition();
		}

		public override Rect GetGaugeRect()
		{
			return gaugeRect.rect;
		}

		public override Transform CenterTransform()
		{
			return gaugeRect.transform;
		}

		public void ChangeBackgroundColor(float value)
		{
			Color color = fadingBackground.color;
			Color color2 = gaugeTransform.GetComponent<Image>().color;
			color.a = Mathf.Lerp(0f, backgroundTransparency, value);
			color2.a = value;
			fadingBackground.color = color;
			gaugeTransform.GetComponent<Image>().color = color2;
		}
	}
}
