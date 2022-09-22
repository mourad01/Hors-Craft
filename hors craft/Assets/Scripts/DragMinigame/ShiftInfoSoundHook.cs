// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.ShiftInfoSoundHook
using UnityEngine;

namespace DragMinigame
{
	public class ShiftInfoSoundHook : MonoBehaviour
	{
		[SerializeField]
		private AudioClip clip;

		[SerializeField]
		private float pitch;

		[SerializeField]
		private float volume;

		public void PlaySound()
		{
			DragRacingGameManager.dragRacingManagerInstance.Play2dSound(clip, pitch, 0.5f);
		}
	}
}
