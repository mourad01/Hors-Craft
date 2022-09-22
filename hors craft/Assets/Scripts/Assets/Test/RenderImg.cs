// DecompilerFi decompiler from Assembly-CSharp.dll class: Assets.Test.RenderImg
using UnityEngine;

namespace Assets.Test
{
	internal class RenderImg : MonoBehaviour
	{
		public Material mat;

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!(mat == null))
			{
				Graphics.Blit(source, destination, mat);
			}
		}
	}
}
