// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.SimpleRepeatButton
using Common.Managers;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI
{
	[RequireComponent(typeof(Image))]
	public class SimpleRepeatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public delegate void OnFingerDown(int fingerId);

		public delegate void OnFingerUp(int fingerId);

		[Header("ColorManager options")]
		public bool useColorController;

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

		public RectTransform rectTransform => (RectTransform)base.transform;

		public int fingerId
		{
			get;
			private set;
		}

		private void Awake()
		{
			image = GetComponent<Image>();
			SetupColors();
			fingerId = -1;
			fingerIds = new List<int>();
		}

		public void OnPointerDown(PointerEventData data)
		{
			if (Engine.EditMode)
			{
				if (Input.GetMouseButtonDown(2))
				{
					targetColor = pressedColor;
					pressed = true;
					fadeStartTime = Time.realtimeSinceStartup;
					fingerIds.Add(data.pointerId);
					fingerId = data.pointerId;
				}
			}
			else
			{
				targetColor = pressedColor;
				pressed = true;
				fadeStartTime = Time.realtimeSinceStartup;
				fingerIds.Add(data.pointerId);
				fingerId = data.pointerId;
			}
			if (onFingerDown != null)
			{
				onFingerDown(fingerId);
			}
		}

		public void OnPointerUp(PointerEventData data)
		{
			targetColor = baseColor;
			pressed = false;
			fadeStartTime = Time.realtimeSinceStartup;
			if (onFingerUp != null)
			{
				onFingerUp(fingerId);
			}
			fingerId = -1;
			fingerIds.Remove(data.pointerId);
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
			fingerId = -1;
			fingerIds = new List<int>();
		}

		private void SetupColors()
		{
			if (useColorController)
			{
				baseColor = (lastTargetColor = (targetColor = Manager.Get<ColorManager>().GetColorForCategory(colorCategory)));
				pressedColor = baseColor * tintColor;
			}
			else
			{
				image.color = (lastTargetColor = (targetColor = baseColor));
			}
		}
	}
}
