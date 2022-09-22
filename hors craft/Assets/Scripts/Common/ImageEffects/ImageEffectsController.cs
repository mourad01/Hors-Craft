// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectsController
using System.Collections.Generic;
using System.Linq;

namespace Common.ImageEffects
{
	public class ImageEffectsController : MonoBehaviourSingleton<ImageEffectsController>
	{
		private List<EffectBase> _effects;

		private List<EffectBase> effects
		{
			get
			{
				if (_effects == null || _effects.Count == 0)
				{
					Setup();
				}
				return _effects;
			}
			set
			{
				_effects = value;
			}
		}

		private void Awake()
		{
			Setup();
		}

		public EffectBase GetEffectByTag(string tag)
		{
			return effects.FirstOrDefault((EffectBase e) => e.effectTag.Equals(tag));
		}

		public T GetEffect<T>() where T : EffectBase
		{
			foreach (EffectBase effect in effects)
			{
				if (effect is T)
				{
					return (T)effect;
				}
			}
			return (T)null;
		}

		private void Setup()
		{
			effects = GetComponentsInChildren<EffectBase>().ToList();
			foreach (EffectBase effect in effects)
			{
				if (!effect.requirements.MeetRequirement())
				{
					effect.enabled = false;
				}
			}
		}

		public void DisableAll()
		{
			foreach (EffectBase effect in effects)
			{
				effect.enabled = false;
			}
		}

		public void TryEnableAll()
		{
			Setup();
		}
	}
}
