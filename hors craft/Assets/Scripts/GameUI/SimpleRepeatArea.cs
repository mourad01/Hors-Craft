// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.SimpleRepeatArea
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI
{
	public class SimpleRepeatArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
	{
		public Color baseColor = Color.white;

		public Color pressedColor = Color.gray;

		public float fadeDuration = 0.1f;

		private Image image;

		private Color lastTargetColor;

		private Color targetColor;

		private float fadeStartTime;

		public Action onDoubleClick;

		private float lastClickTime;

		private const float doubleClickInterval = 0.2f;

		public RectTransform rectTransform => (RectTransform)base.transform;

		public bool pressed
		{
			get;
			private set;
		}

		public int fingerId
		{
			get;
			private set;
		}

		private void Awake()
		{
			image = base.gameObject.GetComponentInChildren<Image>();
			image.color = (lastTargetColor = (targetColor = baseColor));
			fingerId = -1;
		}

		public void OnPointerEnter(PointerEventData data)
		{
			fingerId = data.pointerId;
		}

		public void OnPointerExit(PointerEventData data)
		{
			fingerId = -1;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (Time.realtimeSinceStartup < lastClickTime + 0.2f && onDoubleClick != null)
			{
				onDoubleClick();
			}
			lastClickTime = Time.realtimeSinceStartup;
		}

		private void OnGUI()
		{
			if (fingerId >= 0 && !pressed)
			{
				StartPress();
			}
			if (fingerId >= 0)
			{
				UpdatePress();
			}
			if (fingerId < 0 && pressed)
			{
				EndPress();
			}
			if (image.color != targetColor)
			{
				image.color = Color.Lerp(lastTargetColor, targetColor, (Time.realtimeSinceStartup - fadeStartTime) / Mathf.Max(0.1f, fadeDuration));
			}
		}

		private void StartPress()
		{
			pressed = true;
			targetColor = pressedColor;
			fadeStartTime = Time.realtimeSinceStartup;
		}

		private void UpdatePress()
		{
			Touch[] touches = Input.touches;
			int num = 0;
			while (true)
			{
				if (num < touches.Length)
				{
					Touch touch = touches[num];
					if (touch.fingerId == fingerId && (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			fingerId = -1;
		}

		private void EndPress()
		{
			pressed = false;
			targetColor = baseColor;
			fadeStartTime = Time.realtimeSinceStartup;
		}
	}
}
