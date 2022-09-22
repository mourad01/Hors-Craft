// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.PatternRepeater
using UnityEngine;

namespace GameUI
{
	[CreateAssetMenu(menuName = "ScriptableObjects/PatternRepeater")]
	public class PatternRepeater : ScriptableObject
	{
		protected Material _pattern;

		public Material pattern
		{
			get
			{
				return _pattern;
			}
			protected set
			{
				_pattern = value;
			}
		}

		public void Init(Material pattern)
		{
			this.pattern = pattern;
		}
	}
}
