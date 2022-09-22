// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.UnityEffectsWrapperBase
using UnityStandardAssets.ImageEffects;

namespace Common.ImageEffects
{
	public class UnityEffectsWrapperBase : EffectBase
	{
		public PostEffectsBase effect;

		private void OnEnable()
		{
			effect.enabled = true;
		}

		private void OnDisable()
		{
			effect.enabled = false;
		}
	}
}
