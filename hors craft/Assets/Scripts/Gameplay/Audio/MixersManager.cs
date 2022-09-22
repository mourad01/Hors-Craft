// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Audio.MixersManager
using Common.Managers;
using Common.Managers.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Gameplay.Audio
{
	public class MixersManager : Manager
	{
		[HideInInspector]
		public AudioMixerGroup uiMixerGroup;

		[HideInInspector]
		public AudioMixerGroup musicMixerGroup;

		private SoundsManager manager;

		public bool mainMuted
		{
			get
			{
				return mainVolume != 0f;
			}
			set
			{
				if (value)
				{
					MuteMain();
				}
				else
				{
					UnmuteMain();
				}
			}
		}

		private float mainVolume
		{
			get
			{
				manager.mixer.GetFloat("mainVolume", out float value);
				return value;
			}
			set
			{
				manager.mixer.SetFloat("mainVolume", value);
			}
		}

		public static void Play(AudioClip clip)
		{
			if (!(clip == null))
			{
				Sound sound = new Sound();
				sound.clip = clip;
				sound.mixerGroup = Manager.Get<MixersManager>().uiMixerGroup;
				Sound sound2 = sound;
				sound2.Play();
			}
		}

		public override void Init()
		{
			manager = Manager.Get<SoundsManager>();
			uiMixerGroup = Resources.Load<AudioMixer>("Audio/Mixers/Mixer").FindMatchingGroups("UI")[0];
			musicMixerGroup = Resources.Load<AudioMixer>("Audio/Mixers/Mixer").FindMatchingGroups("Music")[0];
		}

		public void SwapMainMute()
		{
			if (mainMuted)
			{
				MuteMain();
			}
			else
			{
				UnmuteMain();
			}
		}

		private void MuteMain()
		{
			mainVolume = -80f;
		}

		private void UnmuteMain()
		{
			mainVolume = 0f;
		}
	}
}
