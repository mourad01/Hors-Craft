// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.ColorController
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class ColorController : MonoBehaviour
	{
		[Header("Color")]
		public ColorRepeater repeater;

		public ColorManager.ColorCategory category;

		public string colorName;

		[Header("Pattern")]
		public PatternRepeater patternRepeater;

		public string patternName;

		private void Awake()
		{
			UpdateColor();
		}

		public void UpdateColor()
		{
			Image component = GetComponent<Image>();
			Text component2 = GetComponent<Text>();
			string name = (!string.IsNullOrEmpty(colorName)) ? colorName : ((!(repeater != null)) ? category.ToString() : repeater.name);
			Color colorForName = Manager.Get<ColorManager>().GetColorForName(name);
			if (component != null)
			{
				Image image = component;
				float r = colorForName.r;
				float g = colorForName.g;
				float b = colorForName.b;
				Color color = component.color;
				image.color = new Color(r, g, b, color.a);
			}
			else if (component2 != null)
			{
				Text text = component2;
				float r2 = colorForName.r;
				float g2 = colorForName.g;
				float b2 = colorForName.b;
				Color color2 = component2.color;
				text.color = new Color(r2, g2, b2, color2.a);
			}
			UpdatePattern();
		}

		private void UpdatePattern()
		{
			Image component = GetComponent<Image>();
			string name = (!string.IsNullOrEmpty(patternName)) ? patternName : ((!(patternRepeater != null)) ? category.ToString() : patternRepeater.name);
			Material materialForName = Manager.Get<ColorManager>().GetMaterialForName(name);
			if (materialForName != null)
			{
				component.material = materialForName;
			}
		}
	}
}
