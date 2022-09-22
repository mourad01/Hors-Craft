// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SoundsManager
using Common.Managers.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Managers
{
	[RequireComponent(typeof(AudioSource))]
	public class SoundsManager : Manager
	{
		public AudioMixer mixer;

		public AudioMixerGroup masterMixerGroup;

		private SoundsQueue soundsQueue = new SoundsQueue();

		private const string KEY_SOUND_ENABLED = "settings.sounds.enabled";

		public bool soundEnabled
		{
			get
			{
				return AudioListener.volume > 0f;
			}
			set
			{
				AudioListener.volume = (value ? 1 : 0);
				PlayerPrefs.SetInt("settings.sounds.enabled", value ? 1 : 0);
				PlayerPrefs.Save();
			}
		}

		public override void Init()
		{
			if (mixer == null || masterMixerGroup == null)
			{
				UnityEngine.Debug.LogError("No mixer or master mixer group set in SoundsManagerBehaviour!");
			}
			soundsQueue.InitializeWith(base.gameObject);
			soundEnabled = (PlayerPrefs.GetInt("settings.sounds.enabled", 1) == 1);
		}

		public void PlaySound(Sound config)
		{
			if (!(config.clip == null))
			{
				AudioSource audioSource = soundsQueue.RegisterNewSourceToPlay();
				config.ApplyConfigToSource(audioSource);
				audioSource.Play();
			}
		}

		private void FixedUpdate()
		{
			soundsQueue.UnregisterNonPlayingSources();
		}

		public bool IsPlayingClip(AudioClip clip)
		{
			return soundsQueue.IsPlayingClip(clip);
		}
	}
}
