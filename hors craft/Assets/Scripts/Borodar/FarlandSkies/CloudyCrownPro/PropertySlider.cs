// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.PropertySlider
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	public class PropertySlider : MonoBehaviour
	{
		public enum Type
		{
			CloudsHeight,
			CloudsOffset,
			CloudsRotation,
			StarsExtinction,
			StarsTwinkling,
			SunAlpha,
			SunSize,
			SunFlareBrightness,
			MoonAlpha,
			MoonSize,
			MoonFlareBrightness,
			Exposure
		}

		public Type SliderType;

		private Slider _slider;

		protected void Awake()
		{
			_slider = GetComponent<Slider>();
		}

		protected void Start()
		{
			switch (SliderType)
			{
			case Type.CloudsHeight:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.CloudsHeight;
				break;
			case Type.CloudsOffset:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.CloudsOffset;
				break;
			case Type.CloudsRotation:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.CloudsRotationSpeed;
				break;
			case Type.StarsExtinction:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.StarsExtinction;
				break;
			case Type.StarsTwinkling:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.StarsTwinklingSpeed;
				break;
			case Type.SunAlpha:
			{
				Slider slider2 = _slider;
				Color sunTint = MonoBehaviourSingleton<SkyboxController>.get.SunTint;
				slider2.value = sunTint.a;
				break;
			}
			case Type.SunSize:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.SunSize;
				break;
			case Type.SunFlareBrightness:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.SunFlareBrightness;
				break;
			case Type.MoonAlpha:
			{
				Slider slider = _slider;
				Color moonTint = MonoBehaviourSingleton<SkyboxController>.get.MoonTint;
				slider.value = moonTint.a;
				break;
			}
			case Type.MoonSize:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.MoonSize;
				break;
			case Type.MoonFlareBrightness:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.MoonFlareBrightness;
				break;
			case Type.Exposure:
				_slider.value = MonoBehaviourSingleton<SkyboxController>.get.Exposure;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public void OnValueChanged(float value)
		{
			switch (SliderType)
			{
			case Type.CloudsHeight:
				MonoBehaviourSingleton<SkyboxController>.get.CloudsHeight = value;
				break;
			case Type.CloudsOffset:
				MonoBehaviourSingleton<SkyboxController>.get.CloudsOffset = value;
				break;
			case Type.CloudsRotation:
				MonoBehaviourSingleton<SkyboxController>.get.CloudsRotationSpeed = value;
				break;
			case Type.StarsExtinction:
				MonoBehaviourSingleton<SkyboxController>.get.StarsExtinction = value;
				break;
			case Type.StarsTwinkling:
				MonoBehaviourSingleton<SkyboxController>.get.StarsTwinklingSpeed = value;
				break;
			case Type.SunAlpha:
			{
				Color sunTint = MonoBehaviourSingleton<SkyboxController>.get.SunTint;
				sunTint.a = value;
				MonoBehaviourSingleton<SkyboxController>.get.SunTint = sunTint;
				break;
			}
			case Type.SunSize:
				MonoBehaviourSingleton<SkyboxController>.get.SunSize = value;
				break;
			case Type.SunFlareBrightness:
				MonoBehaviourSingleton<SkyboxController>.get.SunFlareBrightness = value;
				break;
			case Type.MoonAlpha:
			{
				Color moonTint = MonoBehaviourSingleton<SkyboxController>.get.MoonTint;
				moonTint.a = value;
				MonoBehaviourSingleton<SkyboxController>.get.MoonTint = moonTint;
				break;
			}
			case Type.MoonSize:
				MonoBehaviourSingleton<SkyboxController>.get.MoonSize = value;
				break;
			case Type.MoonFlareBrightness:
				MonoBehaviourSingleton<SkyboxController>.get.MoonFlareBrightness = value;
				break;
			case Type.Exposure:
				MonoBehaviourSingleton<SkyboxController>.get.Exposure = value;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
