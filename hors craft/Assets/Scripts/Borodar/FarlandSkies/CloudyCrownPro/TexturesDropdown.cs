// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.TexturesDropdown
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	public class TexturesDropdown : MonoBehaviour
	{
		public enum TextureType
		{
			Stars,
			Sun,
			Moon,
			Clouds
		}

		[SerializeField]
		protected TextureType Type;

		[SerializeField]
		protected Texture[] Textures;

		private Dropdown _dropdown;

		protected void Awake()
		{
			_dropdown = GetComponent<Dropdown>();
		}

		public void OnValueChanged()
		{
			int value = _dropdown.value;
			switch (Type)
			{
			case TextureType.Stars:
				MonoBehaviourSingleton<SkyboxController>.get.StarsCubemap = (Textures[value] as Cubemap);
				break;
			case TextureType.Sun:
				MonoBehaviourSingleton<SkyboxController>.get.SunTexture = (Textures[value] as Texture2D);
				break;
			case TextureType.Moon:
				MonoBehaviourSingleton<SkyboxController>.get.MoonTexture = (Textures[value] as Texture2D);
				break;
			case TextureType.Clouds:
				MonoBehaviourSingleton<SkyboxController>.get.CloudsCubemap = (Textures[value] as Cubemap);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
