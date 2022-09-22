// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.ColorManager
using Common.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
	public class ColorManager : Manager
	{
		public enum ColorCategory
		{
			MAIN_COLOR,
			SECONDARY_COLOR,
			HIGHLIGHT_COLOR,
			FONT_COLOR,
			THIRD_COLOR,
			SECONDARY_FONT_COLOR,
			FONT_COLOR_RED,
			FONT_COLOR_GREEN,
			FONT_COLOR_WHITE,
			DISABLED_COLOR,
			SC_COLOR,
			HEADER_FONT_COLOR,
			TERTIARY_COLOR
		}

		[Serializable]
		public struct CategoryToColor
		{
			public ColorCategory category;

			public Color color;
		}

		[Serializable]
		public class ColorsConstruct
		{
			public ColorRepeater repeater;

			public Color color;
		}

		[Serializable]
		public class PatternConstruct
		{
			public PatternRepeater repeater;

			public Material pattern;
		}

		[Serializable]
		public class NamedColor
		{
			public string name;

			public Color color;
		}

		[Serializable]
		public class NamedPattern
		{
			public string name;

			public Material pattern;
		}

		public CategoryToColor[] colors = new CategoryToColor[12]
		{
			new CategoryToColor
			{
				category = ColorCategory.MAIN_COLOR,
				color = new Color(0.458f, 0.812f, 0.855f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.SECONDARY_COLOR,
				color = new Color(0.761f, 0.635f, 0.537f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.HIGHLIGHT_COLOR,
				color = new Color(0.82f, 0.855f, 0.506f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.FONT_COLOR,
				color = new Color(0.134f, 0.134f, 0.134f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.THIRD_COLOR,
				color = new Color(0.71f, 0.71f, 0.71f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.SECONDARY_FONT_COLOR,
				color = new Color(0f, 0f, 0f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.FONT_COLOR_RED,
				color = new Color(0.9f, 0.134f, 0.134f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.FONT_COLOR_GREEN,
				color = new Color(0.134f, 0.9f, 0.134f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.FONT_COLOR_WHITE,
				color = new Color(1f, 1f, 1f, 1f)
			},
			new CategoryToColor
			{
				category = ColorCategory.DISABLED_COLOR,
				color = new Color(0.921f, 0.921f, 0.894f)
			},
			new CategoryToColor
			{
				category = ColorCategory.SC_COLOR,
				color = new Color(1f, 0.8f, 0.13f)
			},
			new CategoryToColor
			{
				category = ColorCategory.HEADER_FONT_COLOR,
				color = new Color(1f, 0.5f, 0.13f)
			}
		};

		public List<ColorsConstruct> colorsRepeaters = new List<ColorsConstruct>();

		public List<NamedColor> namedColors = new List<NamedColor>();

		public Dictionary<string, Color> _name2Color = new Dictionary<string, Color>();

		public List<PatternConstruct> patternRepeaters = new List<PatternConstruct>();

		public List<NamedPattern> namedPatterns = new List<NamedPattern>();

		public Dictionary<string, Material> _name2Pattern = new Dictionary<string, Material>();

		public override void Init()
		{
			_name2Color.Clear();
			_name2Pattern.Clear();
			foreach (ColorsConstruct colorsRepeater in colorsRepeaters)
			{
				colorsRepeater.repeater.Init(colorsRepeater.color);
				_name2Color.AddOrReplace(colorsRepeater.repeater.name, colorsRepeater.color);
			}
			IEnumerator enumerator2 = Enum.GetValues(typeof(ColorCategory)).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					ColorCategory cat = (ColorCategory)enumerator2.Current;
					_name2Color.AddOrReplace(cat.ToString(), GetColorForCategory(cat));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator2 as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			foreach (NamedColor namedColor in namedColors)
			{
				_name2Color.AddOrReplace(namedColor.name, namedColor.color);
			}
			foreach (PatternConstruct patternRepeater in patternRepeaters)
			{
				patternRepeater.repeater.Init(patternRepeater.pattern);
				_name2Pattern.AddOrReplace(patternRepeater.repeater.name, patternRepeater.pattern);
			}
			foreach (NamedPattern namedPattern in namedPatterns)
			{
				_name2Pattern.AddOrReplace(namedPattern.name, namedPattern.pattern);
			}
		}

		public Color GetColorForCategory(ColorCategory cat)
		{
			int i;
			for (i = 0; i < colors.Length && colors[i].category != cat; i++)
			{
			}
			return (i >= colors.Length) ? Color.black : colors[i].color;
		}

		public Color GetColorForCategory(ColorRepeater repeater)
		{
			return repeater.color;
		}

		public Material GetCPatternForCategory(PatternRepeater repeater)
		{
			return repeater.pattern;
		}

		public Color GetColorForName(string name)
		{
			if (_name2Color.ContainsKey(name))
			{
				return _name2Color[name];
			}
			return Color.white;
		}

		public Material GetMaterialForName(string name)
		{
			if (_name2Pattern.ContainsKey(name))
			{
				return _name2Pattern[name];
			}
			return null;
		}
	}
}
