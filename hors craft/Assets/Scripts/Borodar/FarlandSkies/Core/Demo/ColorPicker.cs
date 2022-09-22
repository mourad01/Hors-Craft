// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.Core.Demo.ColorPicker
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.Core.Demo
{
	public class ColorPicker : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerClickHandler, IEventSystemHandler
	{
		private RectTransform _rectTransform;

		private Image _image;

		public BaseColorButton ColorButton
		{
			get;
			set;
		}

		public void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_image = GetComponent<Image>();
		}

		public void OnDrag(PointerEventData eventData)
		{
			OnPickColor(eventData);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			base.gameObject.SetActive(value: false);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			OnPickColor(eventData);
			base.gameObject.SetActive(value: false);
		}

		private void OnPickColor(PointerEventData eventData)
		{
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
			{
				int x = (int)(_rectTransform.rect.width + localPoint.x);
				int y = (int)(_rectTransform.rect.height + localPoint.y);
				Color pixel = _image.sprite.texture.GetPixel(x, y);
				ColorButton.ChangeColor(pixel);
			}
		}
	}
}
