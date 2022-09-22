// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.ColorRepeater
using UnityEngine;

namespace GameUI
{
	[CreateAssetMenu(menuName = "ScriptableObjects/ColorRepeater")]
	public class ColorRepeater : ScriptableObject
	{
		protected Color _color = Color.white;

		public Color color
		{
			get
			{
				return _color;
			}
			protected set
			{
				_color = value;
			}
		}

		public void Init(Color color)
		{
			this.color = color;
		}
	}
}
