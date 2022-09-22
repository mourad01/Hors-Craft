// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Audio.AnimatorSoundPlayer
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using UnityEngine;

namespace Gameplay.Audio
{
	public class AnimatorSoundPlayer : StateMachineBehaviour
	{
		public bool playSoundOnEnter;

		public GameSound soundOnEnter;

		public bool playSoundOnExit;

		public GameSound soundOnExit;

		public Sound soundConfig;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (playSoundOnEnter)
			{
				Play(soundOnEnter, animator.transform);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (playSoundOnExit)
			{
				Play(soundOnExit, animator.transform);
			}
		}

		private void Play(GameSound sound, Transform parent)
		{
			if (soundConfig != null)
			{
				soundConfig.clip = Manager.Get<ClipsManager>().GetClipFor(sound);
				soundConfig.Play(parent);
			}
		}
	}
}
