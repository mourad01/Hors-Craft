// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.Timer
using Common.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking
{
	public class Timer : MonoBehaviour
	{
		public enum Colors
		{
			RED = 0,
			ORANGE = 1,
			GREEN = 2,
			NONE = 999
		}

		public Image clockImageLayer1;

		public Image clockImageLayer2;

		public Sprite greenClock;

		public Sprite orangeClock;

		public Sprite redClock;

		public Transform clockHand;

		public Action onFinish;

		[HideInInspector]
		public bool destroyOnFinish = true;

		[HideInInspector]
		public bool pause;

		private WorkController _workController;

		private float duration;

		public float timeLeft
		{
			get;
			private set;
		}

		public float progress => (duration - timeLeft) / duration;

		public bool isDone => timeLeft < 0f;

		private WorkController workController
		{
			get
			{
				if (_workController == null)
				{
					_workController = Manager.Get<CookingManager>().workController;
				}
				return _workController;
			}
		}

		public void Init(float time, Colors bgColor, Colors frontColor)
		{
			duration = time;
			timeLeft = time;
			clockImageLayer1.enabled = (bgColor != Colors.NONE);
			clockImageLayer1.sprite = GetSprite(bgColor);
			clockImageLayer2.sprite = GetSprite(frontColor);
		}

		private void Update()
		{
			if (pause)
			{
				return;
			}
			if (isDone)
			{
				if (onFinish != null)
				{
					onFinish();
					onFinish = null;
				}
				if (destroyOnFinish)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			else
			{
				clockImageLayer2.fillAmount = 1f - progress;
				clockHand.rotation = Quaternion.Euler(0f, 0f, -360f * progress);
				timeLeft -= Time.deltaTime;
			}
		}

		private Sprite GetSprite(Colors color)
		{
			switch (color)
			{
			default:
				return null;
			case Colors.GREEN:
				return greenClock;
			case Colors.RED:
				return redClock;
			case Colors.ORANGE:
				return orangeClock;
			}
		}
	}
}
