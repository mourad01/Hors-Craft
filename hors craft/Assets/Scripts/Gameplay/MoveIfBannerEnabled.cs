// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.MoveIfBannerEnabled
using Common.Managers;
using System;
using UnityEngine;

//namespace Gameplay
//{
	/*public class MoveIfBannerEnabled : MonoBehaviour
	{
		public enum Direction
		{
			UP,
			DOWN
		}

		public enum Mode
		{
			MOVE,
			FLATTEN
		}

		public Direction direction = Direction.DOWN;

		[SerializeField]
		private Mode mode;

		private float startingYOffset;

		private static bool bannerDisabled;

		private static bool editorForceEnable;

		private static bool editorBannerForceEnable;

		private HeyzapBannerSize heyzap = new HeyzapBannerSize();

		private AdMobBannerSize admob = new AdMobBannerSize();

		private AddApptrBannerSize addApptr = new AddApptrBannerSize();

		private AppodealBannerSize appodeal = new AppodealBannerSize();

		private FanBannerSize fan = new FanBannerSize();

		private const float checkInterval = 0.5f;

		private float nextCheckTime;

		private float lastBannerHeight;

		public static void BannerDisabled()
		{
			bannerDisabled = true;
		}

		private void Awake()
		{
			SetYOffset();
		}

		private void Update()
		{
			if (Time.realtimeSinceStartup > nextCheckTime)
			{
				nextCheckTime = Time.realtimeSinceStartup + 0.5f;
				AdjustPositionAccordingToBannerHeight();
			}
		}

		private void SetYOffset()
		{
			if (mode == Mode.MOVE)
			{
				Vector2 anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
				startingYOffset = anchoredPosition.y;
			}
			else if (mode == Mode.FLATTEN)
			{
				Vector2 offsetMax = GetComponent<RectTransform>().offsetMax;
				startingYOffset = offsetMax.y;
			}
		}

		private void AdjustPositionAccordingToBannerHeight()
		{
			bool flag = false;
			if (CheckIfBannersAreForceHidden())
			{
				if (lastBannerHeight != 0f)
				{
					lastBannerHeight = 0f;
					flag = true;
				}
			}
			else
			{
				float bannerHeight = GetBannerHeight();
				if (bannerHeight > 0f)
				{
					if (lastBannerHeight != bannerHeight / Manager.Get<CanvasManager>().canvas.scaleFactor)
					{
						lastBannerHeight = bannerHeight / Manager.Get<CanvasManager>().canvas.scaleFactor;
						flag = true;
					}
				}
				else if (lastBannerHeight > 0f)
				{
					lastBannerHeight = 0f;
					flag = true;
				}
			}
			if (flag)
			{
				ResetBannerHeight(lastBannerHeight);
			}
		}

		private void ResetBannerHeight(float bannerHeight)
		{
			lastBannerHeight = bannerHeight;
			RectTransform rectTransform = base.transform as RectTransform;
			float num = (direction != Direction.DOWN) ? 1f : (-1f);
			if (mode == Mode.MOVE)
			{
				RectTransform rectTransform2 = rectTransform;
				Vector2 anchoredPosition = rectTransform.anchoredPosition;
				rectTransform2.anchoredPosition = new Vector2(anchoredPosition.x, startingYOffset + num * lastBannerHeight);
			}
			else if (mode == Mode.FLATTEN)
			{
				RectTransform rectTransform3 = rectTransform;
				Vector2 offsetMax = rectTransform.offsetMax;
				rectTransform3.offsetMax = new Vector2(offsetMax.x, startingYOffset + num * lastBannerHeight);
			}
		}

		public void ChangeMode(Mode mode)
		{
			ResetBannerHeight(0f);
			this.mode = mode;
			SetYOffset();
		}

		private bool CheckIfBannersAreForceHidden()
		{
			return Manager.Get<AbstractModelManager>().modulesContext.isAdsFree || bannerDisabled;
		}

		private float GetFakeEditorBannerHeight()
		{
			int second = DateTime.Now.Second;
			if (editorForceEnable)
			{
				return (!editorBannerForceEnable) ? ((float)Screen.height * 0.15f) : 0f;
			}
			return (second >= 30) ? ((float)Screen.height * 0.15f) : 0f;
		}

		private float GetBannerHeight()
		{
			return GetAlwaysEnabledBannerSize();
		}

		private float GetAlwaysEnabledBannerSize()
		{
			return Mathf.Max(heyzap.GetBannerSize(), admob.GetBannerSize(), addApptr.GetBannerSize(), appodeal.GetBannerSize(), fan.GetBannerSize());
		}

		private float GetApproximatedBannerSize()
		{
			float num = 0f;
			if (heyzap.IsBannerShown())
			{
				num = Mathf.Max(num, heyzap.GetBannerSize());
			}
			if (admob.IsBannerShown())
			{
				num = Mathf.Max(num, admob.GetBannerSize());
			}
			if (addApptr.IsBannerShown())
			{
				num = Mathf.Max(num, addApptr.GetBannerSize());
			}
			if (appodeal.IsBannerShown())
			{
				num = Mathf.Max(num, appodeal.GetBannerSize());
			}
			if (fan.IsBannerShown())
			{
				num = Mathf.Max(num, fan.GetBannerSize());
			}
			return num;
		}
	}
}*/
