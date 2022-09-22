// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.GameUI.CommonButtonController
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.GameUI
{
	public class CommonButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public bool scaleDownInsteadOfUp;

		public bool useAudio = true;

		private float scaleChangeOnDown = 0.1f;

		private Vector3 baseScale;

		private Button button;

		private SimpleRepeatButton simpleRepeatButton;

		private Sound sound;

		private void Awake()
		{
			baseScale = base.transform.localScale;
			button = GetComponent<Button>();
			simpleRepeatButton = GetComponent<SimpleRepeatButton>();
		}

		public void OnPointerDown(PointerEventData data)
		{
			if ((!(button != null) || button.IsInteractable()) && (!(simpleRepeatButton != null) || simpleRepeatButton.isActiveAndEnabled))
			{
				Click();
			}
		}

		private void Click()
		{
			base.transform.localScale = baseScale * (1f + scaleChangeOnDown * (float)((!scaleDownInsteadOfUp) ? 1 : (-1)));
			if (useAudio)
			{
				PlaySound();
			}
		}

		public void OnPointerUp(PointerEventData data)
		{
			base.transform.localScale = baseScale;
		}

		private void SetClickSound()
		{
			if (Manager.Contains<ClipsManager>() && Manager.Contains<MixersManager>())
			{
				sound = new Sound
				{
					clip = Manager.Get<ClipsManager>().GetClipFor("button_click"),
					mixerGroup = Manager.Get<MixersManager>().uiMixerGroup
				};
			}
		}

		private void PlaySound()
		{
			if (sound == null)
			{
				SetClickSound();
			}
			if (sound != null)
			{
				sound.Play();
			}
		}
	}
}
