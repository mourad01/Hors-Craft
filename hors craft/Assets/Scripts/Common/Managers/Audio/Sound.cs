// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.Audio.Sound
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Managers.Audio
{
	[Serializable]
	public class Sound
	{
		public enum EVolumeFrom
		{
			POSITION,
			MANUAL
		}

		public enum EPanStereoFrom
		{
			POSITION,
			MANUAL
		}

		public bool editorFolded = true;

		public AudioClip clip;

		public AudioMixerGroup mixerGroup;

		public EVolumeFrom volumeFrom = EVolumeFrom.MANUAL;

		public float volume = 1f;

		public float maxDistance = 150f;

		public EPanStereoFrom panStereoFrom = EPanStereoFrom.MANUAL;

		public float panStereo = 0.5f;

		public float pitch = 1f;

		public float randomizePitch;

		private const float PAN_STEREO_FROM_POSITION_FACTOR = 0.5f;

		public void Play(Transform parent = null)
		{
			if (parent != null)
			{
				if (volumeFrom == EVolumeFrom.POSITION)
				{
					CalculateVolumeFromDistanceLinear(parent);
				}
				if (panStereoFrom == EPanStereoFrom.POSITION)
				{
					CalculatePanStereoFromPosition(parent);
				}
			}
			Manager.Get<SoundsManager>().PlaySound(this);
		}

		public void ApplyConfigToSource(AudioSource source)
		{
			source.clip = clip;
			source.panStereo = panStereo;
			source.outputAudioMixerGroup = mixerGroup;
			source.volume = volume;
			if (randomizePitch != 0f)
			{
				source.pitch = pitch - randomizePitch / 2f + UnityEngine.Random.Range(0f, randomizePitch);
			}
			else
			{
				source.pitch = pitch;
			}
		}

		private void CalculatePanStereoFromPosition(Transform transform)
		{
			if (Camera.main == null)
			{
				UnityEngine.Debug.LogWarning("There's no main camera - we can't calculate pan stereo from worldPosition");
				return;
			}
			Vector3 vector = Camera.main.WorldToScreenPoint(transform.position);
			float x = vector.x;
			float num = (x / (float)Screen.width - 0.5f) * 2f;
			panStereo = num * 0.5f;
		}

		private void CalculateVolumeFromDistanceLinear(Transform transform)
		{
			if (Camera.main == null)
			{
				UnityEngine.Debug.LogWarning("There's no main camera - we can't calculate volume from worldPosition");
				return;
			}
			Vector3 position = Camera.main.transform.position;
			float num = Vector3.Distance(transform.position, position);
			volume = Mathf.Clamp((maxDistance - num) / maxDistance, 0f, 1f);
		}
	}
}
