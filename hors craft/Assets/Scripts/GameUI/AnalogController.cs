// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.AnalogController
using Common.Managers;
using Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class AnalogController : MonoBehaviour
	{
		public RectTransform gaugeTransform;

		public bool useIndicators;

		public Image leftIndicator;

		public Image rightIndicator;

		public Image upIndicator;

		public Image downIndicator;

		public float indicatorPressedAlpha = 0.8f;

		public float indicatorReleasedAlpha = 0.4f;

		private const float INDICATOR_TRESHOLD = 10f;

		private const float FINGER_TO_GAUGE_DIVIDER = 1.2f;

		private const float FINGER_TO_BASE_POSITION_FACTOR = 0.3f;

		private const float MAX_GAUGE_ANCHOR_OFFSET = 0.4f;

		private RectTransform analogTransform;

		private EaseType easeType;

		[HideInInspector]
		public Canvas canvas;

		public Vector2 center
		{
			get;
			private set;
		}

		private void Awake()
		{
			analogTransform = (RectTransform)base.transform;
			canvas = Manager.Get<CanvasManager>().canvas;
			SetFingerOffset(Vector2.zero);
		}

		public void SetFingerOffset(Vector2 offset)
		{
			SetGaugePosition(offset);
		}

		public void UpdateAnalogControllerPosition()
		{
			center = (Vector2)base.transform.position + analogTransform.rect.center * canvas.scaleFactor;
		}

		private void SetGaugePosition(Vector2 fingerOffset)
		{
			Vector2 a = Vector2.zero;
			a = new Vector2(fingerOffset.x / analogTransform.rect.width, fingerOffset.y / analogTransform.rect.height);
			a = a / 1.2f / canvas.scaleFactor;
			if (a.magnitude > 0.4f)
			{
				a = a.normalized * 0.4f;
			}
			a += new Vector2(0.5f, 0.5f);
			RectTransform rectTransform = gaugeTransform;
			Vector2 vector = a;
			gaugeTransform.anchorMax = vector;
			rectTransform.anchorMin = vector;
			if (useIndicators)
			{
				SetAlpha(leftIndicator, (!(fingerOffset.x < -10f)) ? indicatorReleasedAlpha : indicatorPressedAlpha);
				SetAlpha(rightIndicator, (!(fingerOffset.x > 10f)) ? indicatorReleasedAlpha : indicatorPressedAlpha);
				SetAlpha(downIndicator, (!(fingerOffset.y < -10f)) ? indicatorReleasedAlpha : indicatorPressedAlpha);
				SetAlpha(upIndicator, (!(fingerOffset.y > 10f)) ? indicatorReleasedAlpha : indicatorPressedAlpha);
			}
		}

		private void SetAlpha(Image img, float a)
		{
			Color color = img.color;
			if (color.a != a)
			{
				Color color2 = img.color;
				float r = color2.r;
				Color color3 = img.color;
				float g = color3.g;
				Color color4 = img.color;
				img.color = new Color(r, g, color4.b, a);
			}
		}

		public void SetGaugeCenterPositon()
		{
			SetGaugePosition(Vector2.zero);
		}
	}
}
