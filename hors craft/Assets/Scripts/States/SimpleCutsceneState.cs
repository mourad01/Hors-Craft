// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SimpleCutsceneState
using Common.Managers.States;
using Common.Utils;
using System;
using UnityEngine;

namespace States
{
	public class SimpleCutsceneState : XCraftUIState<SimpleCutsceneStateConnector>
	{
		private bool fadingFirstPart;

		private bool fadingSecondPart;

		private bool fadingOut;

		private float timer;

		private float duration = 2f;

		private float deltaTime;

		private float previousTime;

		private Sprite sprite;

		private Action doAfter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = false;
				return autoAdsConfig;
			}
		}

		//protected override bool canShowBanner => false;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			previousTime = Time.realtimeSinceStartup;
		}

		public void FadeFor(Sprite sprite, Action doAfter)
		{
			fadingFirstPart = true;
			timer = 0f;
			duration = 1f;
			this.sprite = sprite;
			this.doAfter = doAfter;
		}

		public void FadeOut(Action doAfter, float duration = 2f)
		{
			fadingOut = true;
			timer = 0f;
			this.doAfter = doAfter;
			this.duration = duration;
			base.connector.bg.gameObject.SetActive(value: false);
			EnableTargetImage();
		}

		public override void UpdateState()
		{
			base.UpdateState();
			deltaTime = Time.realtimeSinceStartup - previousTime;
			previousTime = Time.realtimeSinceStartup;
			if (fadingFirstPart)
			{
				UpdateFadeInBg();
			}
			if (fadingSecondPart)
			{
				UpdateBlendBGAndSpriteFading();
			}
			if (fadingOut)
			{
				UpdateFadingOut();
			}
		}

		private void UpdateFadeInBg()
		{
			if (timer < duration)
			{
				base.connector.bg.gameObject.SetActive(value: true);
				base.connector.targetImage.gameObject.SetActive(value: false);
				float value = timer / duration;
				base.connector.bg.color = new Color(0f, 0f, 0f, Easing.Ease(EaseType.InCubic, 0f, 1f, value));
				timer += deltaTime;
			}
			else
			{
				timer = 0f;
				duration = 2f;
				fadingSecondPart = true;
				fadingFirstPart = false;
				EnableTargetImage(sprite);
				base.connector.targetImage.color = new Color(1f, 1f, 1f, 0f);
			}
		}

		private void UpdateBlendBGAndSpriteFading()
		{
			if (timer < duration)
			{
				float value = timer / duration;
				base.connector.bg.color = new Color(0f, 0f, 0f, Easing.Ease(EaseType.InCubic, 1f, 0f, value));
				base.connector.targetImage.color = new Color(1f, 1f, 1f, Easing.Ease(EaseType.InCubic, 0f, 1f, value));
				timer += deltaTime;
				return;
			}
			fadingSecondPart = false;
			base.connector.bg.gameObject.SetActive(value: false);
			base.connector.targetImage.gameObject.SetActive(value: false);
			if (doAfter != null)
			{
				doAfter();
			}
		}

		private void UpdateFadingOut()
		{
			if (timer < duration)
			{
				float value = timer / duration;
				base.connector.targetImage.color = new Color(1f, 1f, 1f, Easing.Ease(EaseType.InCubic, 1f, 0f, value));
				timer += deltaTime;
				return;
			}
			fadingOut = false;
			base.connector.bg.gameObject.SetActive(value: false);
			base.connector.targetImage.gameObject.SetActive(value: false);
			if (doAfter != null)
			{
				doAfter();
			}
		}

		private void EnableTargetImage(Sprite sprite = null)
		{
			if (sprite != null)
			{
				base.connector.targetImage.sprite = sprite;
			}
			base.connector.targetImage.gameObject.SetActive(value: true);
			float num = (float)Screen.width / (float)Screen.height;
			float num2 = 1.77777779f;
			float x = num2 / num;
			base.connector.targetImage.transform.localScale = new Vector3(x, 1f, 1f);
		}
	}
}
