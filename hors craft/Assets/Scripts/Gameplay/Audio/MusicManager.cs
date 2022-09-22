// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Audio.MusicManager
using Common.Managers;
using States;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicManager : Manager
	{
		public ClipsList availableClips;

		public float maxPitchChange;

		public float minPauseBetweenClips;

		public float maxPauseBetweenClips;

		private AudioClip currentClip;

		private bool shouldPlay;

		private float startPauseTime;

		private float pauseBetweenClips;

		private bool pausing;

		private AudioSource source;

		private float defaultPitch;

		private StateMachineManager _stateManager;

		private StateMachineManager stateManager
		{
			get
			{
				if (_stateManager == null)
				{
					_stateManager = Manager.Get<StateMachineManager>();
				}
				return _stateManager;
			}
		}

		public override void Init()
		{
			source = GetComponent<AudioSource>();
			defaultPitch = source.pitch;
		}

		public void SetStatus(bool activate)
		{
			shouldPlay = activate;
		}

		private void PlayNextClip()
		{
			pausing = false;
			List<ClipsList.ClipWithWeight> list = new List<ClipsList.ClipWithWeight>();
			int num = 0;
			foreach (ClipsList.ClipWithWeight audioClip in availableClips.audioClips)
			{
				if (audioClip.clip != currentClip || availableClips.audioClips.Count == 1)
				{
					list.Add(audioClip);
					num += audioClip.weight;
				}
			}
			int num2 = Random.Range(1, num + 1);
			foreach (ClipsList.ClipWithWeight item in list)
			{
				num2 -= item.weight;
				if (num2 <= 0)
				{
					currentClip = item.clip;
					break;
				}
			}
			source.clip = currentClip;
			source.pitch = defaultPitch + Random.Range(0f - maxPitchChange, maxPitchChange);
			source.Play();
		}

		private bool CanPlay()
		{
			if (stateManager.currentState is GameplayState || stateManager.IsStateInStack(typeof(IsometricObjectPlacementState)) || stateManager.IsStateInStack(typeof(CraftingPopupState)) || stateManager.IsStateInStack(typeof(BlocksPopupState)) || stateManager.IsStateInStack(typeof(BlueprintAnimationState)) || ((stateManager.currentState is WatchXAdsPopUpState || stateManager.currentState is GenericPopupState) && stateManager.IsStateInStack(typeof(GameplayState)) && !stateManager.IsStateInStack(typeof(PauseState))))
			{
				return true;
			}
			return false;
		}

		private void Update()
		{
			if (!shouldPlay || availableClips == null)
			{
				return;
			}
			if (!CanPlay() && source.isPlaying)
			{
				source.Stop();
			}
			else if (CanPlay())
			{
				if (!source.isPlaying)
				{
					if (!pausing)
					{
						startPauseTime = Time.realtimeSinceStartup;
						pausing = true;
						pauseBetweenClips = Random.Range(minPauseBetweenClips, maxPauseBetweenClips);
					}
					else if (Time.realtimeSinceStartup - startPauseTime > pauseBetweenClips)
					{
						PlayNextClip();
					}
				}
			}
			else
			{
				pausing = false;
			}
		}
	}
}
