// DecompilerFi decompiler from Assembly-CSharp.dll class: FeatureElement
using UnityEngine;
using UnityEngine.UI;

public class FeatureElement : MonoBehaviour
{
	public Image icon;

	public TranslateText text;

	public void Init(string defaultText, string translationKey, Sprite icon = null)
	{
		this.icon.sprite = icon;
		text.defaultText = defaultText;
		text.translationKey = translationKey;
		text.ForceRefresh();
	}
}
