// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.GameUI.SimpleRepeatButton
using Common.Managers;
using GameUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.GameUI
{
	[RequireComponent(typeof(Image))]
	public class SimpleRepeatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public delegate void OnFingerDown(int fingerId);

		public delegate void OnFingerUp(int fingerId);

		public delegate void OnFingerStay(int fingerId);

		[Header("ColorManager options")]
		public bool useColorManager;

		public Color tintColor = Color.gray;

		public ColorManager.ColorCategory colorCategory;

		[Header("Default options")]
		public Color baseColor = Color.white;

		public Color pressedColor = Color.gray;

		public float fadeDuration = 0.1f;

		private Image image;

		private Color lastTargetColor;

		private Color targetColor;

		private float fadeStartTime;

		public OnFingerDown onFingerDown;

		public OnFingerDown onFingerUp;

		public OnFingerStay onFingerStay;

		public bool pressed
		{
			get;
			private set;
		}

		public List<int> fingerIds
		{
			get;
			private set;
		}

		public int fingerId
		{
			get;
			private set;
		}

		public RectTransform rectTransform => (RectTransform)base.transform;

		private void Awake()
		{
			image = GetComponent<Image>();
			SetupColors();
			fingerId = -1;
			fingerIds = new List<int>();
		}

		public void OnPointerDown(PointerEventData data)
		{
			targetColor = pressedColor;
			pressed = true;
			fadeStartTime = Time.realtimeSinceStartup;
			fingerIds.Add(data.pointerId);
			if (onFingerDown != null)
			{
				onFingerDown(data.pointerId);
			}
			fingerId = data.pointerId;
		}

		private void Update()
		{
			if (pressed && onFingerStay != null)
			{
				onFingerStay(fingerId);
			}
		}

		public void OnPointerUp(PointerEventData data)
		{
			targetColor = baseColor;
			fadeStartTime = Time.realtimeSinceStartup;
			if (onFingerUp != null)
			{
				onFingerUp(data.pointerId);
			}
			fingerIds.Remove(data.pointerId);
			if (fingerIds.Count == 0)
			{
				pressed = false;
			}
			fingerId = -1;
		}

		private void OnGUI()
		{
			if (image.color != targetColor)
			{
				image.color = Color.Lerp(lastTargetColor, targetColor, (Time.realtimeSinceStartup - fadeStartTime) / Mathf.Max(0.1f, fadeDuration));
			}
		}

		private void OnDisable()
		{
			targetColor = baseColor;
			pressed = false;
			fadeStartTime = Time.realtimeSinceStartup;
			fingerIds = new List<int>();
			fingerId = -1;
		}

		private void SetupColors()
		{
			if (useColorManager)
			{
				baseColor = (lastTargetColor = (targetColor = Manager.Get<ColorManager>().GetColorForCategory(colorCategory)));
				pressedColor = baseColor * tintColor;
			}
			else
			{
				image.color = (lastTargetColor = (targetColor = baseColor));
			}
		}

		private void OnApplicationPause(bool pause)
		{
			fingerIds.Clear();
			pressed = false;
		}
	}
}
