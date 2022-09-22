// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.ButtonDragFixer
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI
{
	public class ButtonDragFixer : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
	{
		private Vector2 startPos;

		private float maxDistance = 75f;

		private bool dragHere;

		private Vector3 baseScale = new Vector3(1f, 1f, 1f);

		private Button button;

		private GameObject buttonGameObject;

		public void OnBeginDrag(PointerEventData data)
		{
			startPos = data.position;
			button = data.pointerCurrentRaycast.gameObject.GetComponent<Button>();
			if (button != null)
			{
				buttonGameObject = data.pointerCurrentRaycast.gameObject;
				Sound sound = new Sound();
				sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.BUTTON_CLICK);
				sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
				Sound sound2 = sound;
				sound2.Play();
			}
		}

		public void OnEndDrag(PointerEventData data)
		{
			if (button != null)
			{
				float num = Vector2.Distance(startPos, data.position);
				if (num < maxDistance)
				{
					button.onClick.Invoke();
				}
				buttonGameObject.transform.localScale = baseScale;
			}
		}
	}
}
