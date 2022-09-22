// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Audio.MixersManager
using Common.Managers;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Audio
{
	public class MixersManager : Manager
	{
		[HideInInspector]
		public AudioMixerGroup uiMixerGroup;

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

		public override void Init()
		{
			manager = Manager.Get<SoundsManager>();
			uiMixerGroup = Resources.Load<AudioMixer>("Audio/Mixers/Mixer").FindMatchingGroups("UI")[0];
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
