// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.Core.Demo.BaseColorButton
using UnityEngine;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.Core.Demo
{
	public abstract class BaseColorButton : MonoBehaviour
	{
		public ColorPicker ColorPicker;

		protected Image ColorImage;

		protected void Awake()
		{
			ColorImage = GetComponent<Image>();
		}

		public void OnClick()
		{
			ColorPicker.ColorButton = this;
			ColorPicker.gameObject.SetActive(value: true);
		}

		public virtual void ChangeColor(Color color)
		{
			ColorImage.color = color;
		}
	}
}
