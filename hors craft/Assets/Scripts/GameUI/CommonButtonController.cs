// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.CommonButtonController
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI
{
	public class CommonButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public bool scaleDownInsteadOfUp;

		public bool useAudio = true;

		private float scaleChangeOnDown = 0.1f;

		private Vector3 baseScale;

		private Button button;

		private Button.ButtonClickedEvent onClick;

		private int sem;

		private void Awake()
		{
			baseScale = base.transform.localScale;
			button = GetComponent<Button>();
		}

		public void OnPointerDown(PointerEventData data)
		{
			if (button == null || button.IsInteractable())
			{
				sem++;
				if (button != null && button.onClick != null && sem == 1)
				{
					onClick = button.onClick;
					button.onClick = new Button.ButtonClickedEvent();
				}
				base.transform.localScale = baseScale * (1f + scaleChangeOnDown * (float)((!scaleDownInsteadOfUp) ? 1 : (-1)));
				if (useAudio)
				{
					Sound sound = new Sound();
					sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.BUTTON_CLICK);
					sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
					Sound sound2 = sound;
					sound2.Play();
				}
			}
		}

		public void OnPointerUp(PointerEventData data)
		{
			sem--;
			if (onClick != null && sem == 0)
			{
				button.onClick = onClick;
				onClick = null;
			}
			base.transform.localScale = baseScale;
		}

		private void OnDisable()
		{
			ResetMy();
		}

		private void OnEnable()
		{
			ResetMy();
		}

		private void ResetMy()
		{
			if (onClick != null && sem != 0)
			{
				button.onClick = onClick;
				onClick = null;
			}
			sem = 0;
		}
	}
}
