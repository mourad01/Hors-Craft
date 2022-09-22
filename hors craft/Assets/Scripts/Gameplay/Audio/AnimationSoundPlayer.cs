// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Audio.AnimationSoundPlayer
using Common.Managers.Audio;
using UnityEngine;

namespace Gameplay.Audio
{
	public class AnimationSoundPlayer : MonoBehaviour
	{
		public Sound soundConfig;

		public void Play(AnimationEvent ev)
		{
			if (ev.objectReferenceParameter != null)
			{
				AudioClip audioClip = ev.objectReferenceParameter as AudioClip;
				if (audioClip != null)
				{
					soundConfig.clip = audioClip;
					soundConfig.Play();
				}
			}
		}
	}
}
