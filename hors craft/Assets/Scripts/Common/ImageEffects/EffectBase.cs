// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.EffectBase
using System;
using UnityEngine;

namespace Common.ImageEffects
{
	public abstract class EffectBase : MonoBehaviour
	{
		[Serializable]
		public class ImageEffectsRequirements
		{
			public ImageEffectsRequirement[] requirements;

			public bool MeetRequirement()
			{
				ImageEffectsRequirement[] array = requirements;
				foreach (ImageEffectsRequirement imageEffectsRequirement in array)
				{
					if (!imageEffectsRequirement.MeetRequirement())
					{
						UnityEngine.Debug.LogError("Do not meet: " + imageEffectsRequirement.GetType().Name);
						return false;
					}
				}
				return true;
			}
		}

		public string effectTag;

		public ImageEffectsRequirements requirements = new ImageEffectsRequirements();

		public void TryEnable()
		{
			if (!base.enabled)
			{
				base.enabled = requirements.MeetRequirement();
			}
		}
	}
}
