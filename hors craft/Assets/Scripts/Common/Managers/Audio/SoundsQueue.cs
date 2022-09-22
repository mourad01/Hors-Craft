// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.Audio.SoundsQueue
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers.Audio
{
	internal class SoundsQueue
	{
		private GameObject parent;

		private Stack<AudioSource> unusedSources = new Stack<AudioSource>();

		private List<AudioSource> playingSources = new List<AudioSource>();

		public void InitializeWith(GameObject parentGameObject)
		{
			parent = parentGameObject;
			AudioSource[] components = parent.GetComponents<AudioSource>();
			foreach (AudioSource source in components)
			{
				PushUnusedSource(source);
			}
		}

		public AudioSource RegisterNewSourceToPlay()
		{
			AudioSource audioSource = PopUnusedSource();
			playingSources.Add(audioSource);
			return audioSource;
		}

		public void UnregisterNonPlayingSources()
		{
			int num = 0;
			while (num < playingSources.Count)
			{
				if (!playingSources[num].isPlaying)
				{
					PushUnusedSource(playingSources[num]);
					playingSources.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}

		public bool IsPlayingClip(AudioClip clip)
		{
			foreach (AudioSource playingSource in playingSources)
			{
				if (playingSource.clip == clip)
				{
					return true;
				}
			}
			return false;
		}

		private void PushUnusedSource(AudioSource source)
		{
			source.enabled = false;
			unusedSources.Push(source);
		}

		private AudioSource PopUnusedSource()
		{
			AudioSource audioSource = (unusedSources.Count != 0) ? unusedSources.Pop() : parent.AddComponent<AudioSource>();
			audioSource.enabled = true;
			return audioSource;
		}
	}
}
