// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RhythmicMinigame.RhythmController
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.RhythmicMinigame
{
	public class RhythmController
	{
		public delegate void OnBeat();

		public delegate void OnBeatsPassed(float beats);

		private List<OnBeat> onBeatListeners = new List<OnBeat>();

		private List<OnBeat> onBeatListenersToRemove = new List<OnBeat>();

		private Dictionary<OnBeatsPassed, float> beatsPassedListeners = new Dictionary<OnBeatsPassed, float>();

		private List<OnBeatsPassed> beatsPassedListenersToRemove = new List<OnBeatsPassed>();

		private bool started;

		private float beatLength;

		private int beatsPassedRound;

		private AudioSource musicSource;

		private int musicPassedCount;

		private float nextBeatLength;

		private AudioClip nextClip;

		private bool newTempoClipConfig;

		public bool paused
		{
			get;
			private set;
		}

		public RhythmController()
		{
			beatLength = 1f;
			musicPassedCount = 0;
			started = false;
			PrepareMusicSource();
		}

		public void AddOnBeatListener(OnBeat onBeat)
		{
			onBeatListeners.Add(onBeat);
		}

		public void RemoveOnBeatListener(OnBeat onBeat)
		{
			onBeatListenersToRemove.Add(onBeat);
		}

		public void AddBeatsPassedListener(OnBeatsPassed onBeatsPassed)
		{
			beatsPassedListeners.Add(onBeatsPassed, CalculateBeatsPassed());
		}

		public void RemoveBeatsPassedListener(OnBeatsPassed onBeat)
		{
			beatsPassedListenersToRemove.Add(onBeat);
		}

		private float CalculateBeatsPassed()
		{
			return (float)(8 * musicPassedCount) + musicSource.time / beatLength;
		}

		private void PrepareMusicSource()
		{
			musicSource = new GameObject("rhythmMusic").AddComponent<AudioSource>();
			musicSource.playOnAwake = false;
		}

		public void Dispose()
		{
			UnityEngine.Object.Destroy(musicSource.gameObject);
		}

		public void SetTempoAndClip(float tempo, AudioClip clip)
		{
			nextBeatLength = 1f / Mathf.Max(0.01f, tempo);
			nextClip = clip;
			newTempoClipConfig = true;
		}

		private void ApplyNewTempoClipConfig()
		{
			beatLength = nextBeatLength;
			musicSource.clip = nextClip;
			newTempoClipConfig = false;
		}

		public void Play()
		{
			if (!started)
			{
				Start();
			}
			if (paused)
			{
				paused = false;
				musicSource.UnPause();
			}
		}

		private void Start()
		{
			if (newTempoClipConfig)
			{
				ApplyNewTempoClipConfig();
			}
			musicSource.Play();
			started = true;
		}

		public void Pause()
		{
			if (!paused)
			{
				paused = true;
				musicSource.Pause();
			}
		}

		public void Update()
		{
			if (paused || !started)
			{
				return;
			}
			if (!musicSource.isPlaying)
			{
				musicPassedCount++;
				if (newTempoClipConfig)
				{
					ApplyNewTempoClipConfig();
				}
				musicSource.Play();
			}
			float num = CalculateBeatsPassed();
			while (num > (float)beatsPassedRound)
			{
				beatsPassedRound++;
				foreach (OnBeat onBeatListener in onBeatListeners)
				{
					onBeatListener();
				}
			}
			foreach (KeyValuePair<OnBeatsPassed, float> beatsPassedListener in beatsPassedListeners)
			{
				float beats = num - beatsPassedListener.Value;
				beatsPassedListener.Key(beats);
			}
			if (onBeatListenersToRemove.Count > 0)
			{
				foreach (OnBeat item in onBeatListenersToRemove)
				{
					onBeatListeners.Remove(item);
				}
				onBeatListenersToRemove.Clear();
			}
			if (beatsPassedListenersToRemove.Count > 0)
			{
				foreach (OnBeatsPassed item2 in beatsPassedListenersToRemove)
				{
					beatsPassedListeners.Remove(item2);
				}
				beatsPassedListenersToRemove.Clear();
			}
		}
	}
}
