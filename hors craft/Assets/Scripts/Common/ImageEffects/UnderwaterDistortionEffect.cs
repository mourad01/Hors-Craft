// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.UnderwaterDistortionEffect
using UnityEngine;

namespace Common.ImageEffects
{
	public class UnderwaterDistortionEffect : ImageEffectBase
	{
		private float underwaterEffectLerp;

		public float underwaterEffectLerpSpeed = 1f;

		public float underwaterEffectMin = 0.1f;

		public float underwaterEffectMax = 0.25f;

		private float lerpDirection = 1f;

		private void OnEnable()
		{
			underwaterEffectLerp = 0f;
		}

		private void Update()
		{
			if (underwaterEffectLerp < 1f || underwaterEffectLerp > 0f)
			{
				underwaterEffectLerp += Time.deltaTime * underwaterEffectLerpSpeed * lerpDirection;
				underwaterEffectLerp = Mathf.Clamp01(underwaterEffectLerp);
			}
			material.SetFloat("_Strength", Mathf.Lerp(underwaterEffectMin, underwaterEffectMax, underwaterEffectLerp));
		}

		public void LerpStrength(float lerpDir)
		{
			lerpDirection = lerpDir;
		}
	}
}
