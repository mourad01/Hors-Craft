// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.SoundController
using UnityEngine;

namespace DragMinigame
{
	public class SoundController : MonoBehaviour
	{
		[SerializeField]
		private AudioSource gearSound;

		[SerializeField]
		private AudioSource engineSound;

		[SerializeField]
		private AudioSource nitroSound;

		[SerializeField]
		private AudioSource slippageSound;

		[SerializeField]
		private AudioSource brakingSound;

		private float currentTacho;

		private float maxTacho = 8f;

		private float penalty;

		public void GearPlay(float penalty)
		{
			this.penalty = penalty;
			if (penalty < 1f)
			{
				gearSound.Play();
			}
			else
			{
				slippageSound.Play();
			}
		}

		public void SetEnginePItch(float tacho)
		{
			if (slippageSound.isPlaying)
			{
				tacho = 7.5f;
			}
			float target = 0.5f + tacho / 6f;
			float pitch = Mathf.MoveTowards(engineSound.pitch, target, Time.deltaTime);
			engineSound.pitch = pitch;
		}

		public void BrakingPlay()
		{
			brakingSound.Play();
		}

		public void StartEngine()
		{
			engineSound.Play();
		}

		public void StopEngine()
		{
			engineSound.Stop();
		}

		public void BrakingStop()
		{
			brakingSound.Stop();
		}

		public void NitroPlay()
		{
			nitroSound.Play();
		}

		public void NitroStop()
		{
			nitroSound.Stop();
		}
	}
}
