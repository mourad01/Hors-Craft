// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.PropertyToggle
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	public class PropertyToggle : MonoBehaviour
	{
		public enum Type
		{
			SunFlare,
			MoonFlare,
			AdjustFog
		}

		public Type ToggleType;

		private Toggle _toggle;

		protected void Awake()
		{
			_toggle = GetComponent<Toggle>();
		}

		protected void Start()
		{
			switch (ToggleType)
			{
			case Type.SunFlare:
				_toggle.isOn = MonoBehaviourSingleton<SkyboxController>.get.SunFlare;
				break;
			case Type.MoonFlare:
				_toggle.isOn = MonoBehaviourSingleton<SkyboxController>.get.MoonFlare;
				break;
			case Type.AdjustFog:
				_toggle.isOn = MonoBehaviourSingleton<SkyboxController>.get.AdjustFogColor;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public void OnValueChanged(bool value)
		{
			switch (ToggleType)
			{
			case Type.SunFlare:
				MonoBehaviourSingleton<SkyboxController>.get.SunFlare = value;
				break;
			case Type.MoonFlare:
				MonoBehaviourSingleton<SkyboxController>.get.MoonFlare = value;
				break;
			case Type.AdjustFog:
				MonoBehaviourSingleton<SkyboxController>.get.AdjustFogColor = value;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
