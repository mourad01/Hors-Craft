// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.ColorButton
using Borodar.FarlandSkies.Core.Demo;
using System;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	public class ColorButton : BaseColorButton
	{
		public enum ColorType
		{
			Top,
			Bottom,
			StarsTint,
			SunTint,
			MoonTint
		}

		public ColorType SkyColorType;

		protected void Start()
		{
			switch (SkyColorType)
			{
			case ColorType.Top:
				ColorImage.color = MonoBehaviourSingleton<SkyboxController>.get.TopColor;
				break;
			case ColorType.Bottom:
				ColorImage.color = MonoBehaviourSingleton<SkyboxController>.get.BottomColor;
				break;
			case ColorType.StarsTint:
				ColorImage.color = MonoBehaviourSingleton<SkyboxController>.get.StarsTint;
				break;
			case ColorType.SunTint:
				ColorImage.color = MonoBehaviourSingleton<SkyboxController>.get.SunTint;
				break;
			case ColorType.MoonTint:
				ColorImage.color = MonoBehaviourSingleton<SkyboxController>.get.MoonTint;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public override void ChangeColor(Color color)
		{
			base.ChangeColor(color);
			switch (SkyColorType)
			{
			case ColorType.Top:
				MonoBehaviourSingleton<SkyboxController>.get.TopColor = color;
				break;
			case ColorType.Bottom:
				MonoBehaviourSingleton<SkyboxController>.get.BottomColor = color;
				break;
			case ColorType.StarsTint:
				MonoBehaviourSingleton<SkyboxController>.get.StarsTint = color;
				break;
			case ColorType.SunTint:
			{
				Color sunTint = MonoBehaviourSingleton<SkyboxController>.get.SunTint;
				color.a = sunTint.a;
				MonoBehaviourSingleton<SkyboxController>.get.SunTint = color;
				break;
			}
			case ColorType.MoonTint:
			{
				Color moonTint = MonoBehaviourSingleton<SkyboxController>.get.MoonTint;
				color.a = moonTint.a;
				MonoBehaviourSingleton<SkyboxController>.get.MoonTint = color;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
