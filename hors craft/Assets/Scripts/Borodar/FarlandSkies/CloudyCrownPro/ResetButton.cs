// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.ResetButton
using UnityEngine;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	public class ResetButton : MonoBehaviour
	{
		private static class DefaultValue
		{
			public static Color TopColor;

			public static Color BottomColor;

			public static Color StarsTint;

			public static float StarsExtinction;

			public static float StarsTwinklingSpeed;

			public static Color SunTint;

			public static float SunSize;

			public static bool SunFlare;

			public static float SunFlareBrightness;

			public static Color MoonTint;

			public static float MoonSize;

			public static bool MoonFlare;

			public static float MoonFlareBrightness;

			public static float CloudsHeight;

			public static float CloudsOffset;

			public static float CloudsRotationSpeed;

			public static float Exposure;

			public static bool AdjustFog;
		}

		[Header("Sky")]
		public Image TopColorImage;

		public Image BottomColorImage;

		[Header("Clouds")]
		public Dropdown CloudsDropdown;

		public Slider CloudsHeightSlider;

		public Slider CloudsOffsetSlider;

		public Slider CloudsRotationSlider;

		[Header("Stars")]
		public Dropdown StarsDropdown;

		public Image StarsTintImage;

		public Slider ExtinctionSlider;

		public Slider TwinklingSpeedSlider;

		[Header("Sun")]
		public Dropdown SunDropdown;

		public Image SunTintImage;

		public Slider SunAlphaSlider;

		public Slider SunSizeSlider;

		public Toggle SunFlareToggle;

		public Slider SunFlareBrightnessSlider;

		[Header("Moon")]
		public Dropdown MoonDropdown;

		public Image MoonTintImage;

		public Slider MoonAlphaSlider;

		public Slider MoonSizeSlider;

		public Toggle MoonFlareToggle;

		public Slider MoonFlareBrightnessSlider;

		[Header("General")]
		public Slider ExoposureSlider;

		public Toggle AdjustFogToggle;

		public void Start()
		{
			DefaultValue.TopColor = MonoBehaviourSingleton<SkyboxController>.get.TopColor;
			DefaultValue.BottomColor = MonoBehaviourSingleton<SkyboxController>.get.BottomColor;
			DefaultValue.StarsTint = MonoBehaviourSingleton<SkyboxController>.get.StarsTint;
			DefaultValue.StarsExtinction = MonoBehaviourSingleton<SkyboxController>.get.StarsExtinction;
			DefaultValue.StarsTwinklingSpeed = MonoBehaviourSingleton<SkyboxController>.get.StarsTwinklingSpeed;
			DefaultValue.SunTint = MonoBehaviourSingleton<SkyboxController>.get.SunTint;
			DefaultValue.SunSize = MonoBehaviourSingleton<SkyboxController>.get.SunSize;
			DefaultValue.SunFlare = MonoBehaviourSingleton<SkyboxController>.get.SunFlare;
			DefaultValue.SunFlareBrightness = MonoBehaviourSingleton<SkyboxController>.get.SunFlareBrightness;
			DefaultValue.MoonTint = MonoBehaviourSingleton<SkyboxController>.get.MoonTint;
			DefaultValue.MoonSize = MonoBehaviourSingleton<SkyboxController>.get.MoonSize;
			DefaultValue.MoonFlare = MonoBehaviourSingleton<SkyboxController>.get.MoonFlare;
			DefaultValue.MoonFlareBrightness = MonoBehaviourSingleton<SkyboxController>.get.MoonFlareBrightness;
			DefaultValue.CloudsHeight = MonoBehaviourSingleton<SkyboxController>.get.CloudsHeight;
			DefaultValue.CloudsOffset = MonoBehaviourSingleton<SkyboxController>.get.CloudsOffset;
			DefaultValue.CloudsRotationSpeed = MonoBehaviourSingleton<SkyboxController>.get.CloudsRotationSpeed;
			DefaultValue.Exposure = MonoBehaviourSingleton<SkyboxController>.get.Exposure;
			DefaultValue.AdjustFog = MonoBehaviourSingleton<SkyboxController>.get.AdjustFogColor;
		}

		public void OnClick()
		{
			MonoBehaviourSingleton<SkyboxController>.get.TopColor = DefaultValue.TopColor;
			TopColorImage.color = DefaultValue.TopColor;
			MonoBehaviourSingleton<SkyboxController>.get.BottomColor = DefaultValue.BottomColor;
			BottomColorImage.color = DefaultValue.BottomColor;
			StarsDropdown.value = 0;
			MonoBehaviourSingleton<SkyboxController>.get.StarsTint = DefaultValue.StarsTint;
			StarsTintImage.color = DefaultValue.StarsTint;
			MonoBehaviourSingleton<SkyboxController>.get.StarsExtinction = DefaultValue.StarsExtinction;
			ExtinctionSlider.value = DefaultValue.StarsExtinction;
			MonoBehaviourSingleton<SkyboxController>.get.StarsTwinklingSpeed = DefaultValue.StarsTwinklingSpeed;
			TwinklingSpeedSlider.value = DefaultValue.StarsTwinklingSpeed;
			SunDropdown.value = 0;
			MonoBehaviourSingleton<SkyboxController>.get.SunTint = DefaultValue.SunTint;
			SunTintImage.color = DefaultValue.SunTint;
			SunAlphaSlider.value = DefaultValue.SunTint.a;
			MonoBehaviourSingleton<SkyboxController>.get.SunSize = DefaultValue.SunSize;
			SunSizeSlider.value = DefaultValue.SunSize;
			MonoBehaviourSingleton<SkyboxController>.get.SunFlare = DefaultValue.SunFlare;
			SunFlareToggle.isOn = DefaultValue.SunFlare;
			MonoBehaviourSingleton<SkyboxController>.get.SunFlareBrightness = DefaultValue.SunFlareBrightness;
			SunFlareBrightnessSlider.value = DefaultValue.SunFlareBrightness;
			MoonDropdown.value = 0;
			MonoBehaviourSingleton<SkyboxController>.get.MoonTint = DefaultValue.MoonTint;
			MoonTintImage.color = DefaultValue.MoonTint;
			MoonAlphaSlider.value = DefaultValue.MoonTint.a;
			MonoBehaviourSingleton<SkyboxController>.get.MoonSize = DefaultValue.MoonSize;
			MoonSizeSlider.value = DefaultValue.MoonSize;
			MonoBehaviourSingleton<SkyboxController>.get.MoonFlare = DefaultValue.MoonFlare;
			MoonFlareToggle.isOn = DefaultValue.MoonFlare;
			MonoBehaviourSingleton<SkyboxController>.get.MoonFlareBrightness = DefaultValue.MoonFlareBrightness;
			MoonFlareBrightnessSlider.value = DefaultValue.MoonFlareBrightness;
			CloudsDropdown.value = 0;
			MonoBehaviourSingleton<SkyboxController>.get.CloudsHeight = DefaultValue.CloudsHeight;
			CloudsHeightSlider.value = DefaultValue.CloudsHeight;
			MonoBehaviourSingleton<SkyboxController>.get.CloudsOffset = DefaultValue.CloudsOffset;
			CloudsOffsetSlider.value = DefaultValue.CloudsOffset;
			MonoBehaviourSingleton<SkyboxController>.get.CloudsRotationSpeed = DefaultValue.CloudsRotationSpeed;
			CloudsRotationSlider.value = DefaultValue.CloudsRotationSpeed;
			MonoBehaviourSingleton<SkyboxController>.get.Exposure = DefaultValue.Exposure;
			ExoposureSlider.value = DefaultValue.Exposure;
			MonoBehaviourSingleton<SkyboxController>.get.AdjustFogColor = DefaultValue.AdjustFog;
			AdjustFogToggle.isOn = DefaultValue.AdjustFog;
		}
	}
}
