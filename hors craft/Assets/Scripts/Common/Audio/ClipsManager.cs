// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Audio.ClipsManager
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Audio
{
	public class ClipsManager : Manager
	{
		[HideInInspector]
		public Dictionary<string, AudioClip> pairs = new Dictionary<string, AudioClip>();

		public bool loadAllClipsOnInit = true;

		public override void Init()
		{
			if (loadAllClipsOnInit)
			{
				LoadAudioClips();
			}
		}

		private void LoadAudioClips()
		{
			AudioClip[] array = Resources.LoadAll<AudioClip>("Audio/");
			if (array == null || array.Length == 0)
			{
				UnityEngine.Debug.LogWarning("No audio clips in Resources/Audio");
			}
			AudioClip[] array2 = array;
			foreach (AudioClip audioClip in array2)
			{
				pairs.Add(audioClip.name, audioClip);
			}
		}

		public AudioClip GetClipFor(Enum clipNameEnum)
		{
			string clipName = clipNameEnum.ToString().ToLower();
			return GetClipFor(clipName);
		}

		public AudioClip GetClipFor(string clipName)
		{
			AudioClip audioClip = null;
			if (pairs.ContainsKey(clipName))
			{
				audioClip = pairs[clipName];
			}
			else if (!loadAllClipsOnInit)
			{
				audioClip = AddAudioClip(clipName);
			}
			if (audioClip == null)
			{
				UnityEngine.Debug.LogWarning("Can't find sound " + clipName);
			}
			return audioClip;
		}

		private AudioClip AddAudioClip(string name)
		{
			AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + name);
			if (audioClip != null)
			{
				pairs.Add(name, audioClip);
			}
			return audioClip;
		}
	}
}
