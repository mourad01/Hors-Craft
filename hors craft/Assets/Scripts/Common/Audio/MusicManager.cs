// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Audio.MusicManager
using Common.Managers;
using UnityEngine;

namespace Common.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicManager : Manager
	{
		public float maxVolume = 0.234f;

		public float volumeChangeDuration = 2.5f;

		private AudioSource source;

		private float volumeChangeStartTime;

		private float lastTargetVolume;

		private float targetVolume;

		public override void Init()
		{
			source = GetComponent<AudioSource>();
			float volume = 0f;
			source.volume = volume;
			targetVolume = (lastTargetVolume = volume);
		}

		public void Play()
		{
			lastTargetVolume = targetVolume;
			targetVolume = maxVolume;
			volumeChangeStartTime = Time.realtimeSinceStartup;
		}

		public void Stop()
		{
			lastTargetVolume = targetVolume;
			targetVolume = 0f;
			volumeChangeStartTime = Time.realtimeSinceStartup;
		}

		public bool IsPlaying()
		{
			return source.isPlaying;
		}

		private void Update()
		{
			if (source.volume != targetVolume)
			{
				float value = (Time.realtimeSinceStartup - volumeChangeStartTime) / volumeChangeDuration;
				value = Mathf.Clamp(value, 0f, 1f);
				source.volume = Mathf.Lerp(lastTargetVolume, targetVolume, value);
				if (source.volume > 0f && !source.isPlaying)
				{
					source.Play();
				}
				if (source.volume == 0f && source.isPlaying)
				{
					source.Stop();
				}
			}
		}
	}
}
