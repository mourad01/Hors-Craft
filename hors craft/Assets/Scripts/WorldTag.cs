// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldTag
using Common.Managers;
using System;
using UnityEngine;

[Serializable]
public class WorldTag
{
	public string value;

	public string translationKey;

	public int visible;

	public string color;

	public Color colorValue
	{
		get
		{
			Color gray = Color.gray;
			ColorUtility.TryParseHtmlString(color, out gray);
			return gray;
		}
	}

	public string text => Manager.Get<TranslationsManager>().GetText(translationKey, value);

	public bool isVisible => (visible != 0) ? true : false;

	public WorldTag()
	{
	}

	public WorldTag(string value, string translationKey, bool visible, Color color)
	{
		this.value = value;
		this.translationKey = translationKey;
		this.visible = (visible ? 1 : 0);
		this.color = ColorUtility.ToHtmlStringRGB(color);
	}
}
