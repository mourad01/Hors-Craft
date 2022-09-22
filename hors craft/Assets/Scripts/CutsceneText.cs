// DecompilerFi decompiler from Assembly-CSharp.dll class: CutsceneText
using UnityEngine;
using UnityEngine.UI;

public class CutsceneText : MonoBehaviour
{
	public Image imageText;

	public Text plainText;

	public string translationKey;

	public string defaultText;

	private void Awake()
	{
		if (Application.systemLanguage == SystemLanguage.English)
		{
			imageText.gameObject.SetActive(value: true);
			plainText.gameObject.SetActive(value: false);
			return;
		}
		imageText.gameObject.SetActive(value: false);
		plainText.gameObject.SetActive(value: true);
		TranslateText component = plainText.GetComponent<TranslateText>();
		component.defaultText = defaultText;
		component.translationKey = translationKey;
		component.ForceRefresh();
	}
}
