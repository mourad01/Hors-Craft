// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectBase
using UnityEngine;

namespace Common.ImageEffects
{
	public class ImageEffectBase : EffectBase
	{
		public Material material;

		protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (material == null)
			{
				base.enabled = false;
			}
			else
			{
				Graphics.Blit(source, destination, material);
			}
		}
	}
}
