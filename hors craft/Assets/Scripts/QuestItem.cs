// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestItem
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
	public Image questImage;

	public Text mainText;

	public Text progress;

	public void Init(string text, int current, int max, Sprite sprite)
	{
		mainText.text = text;
		progress.text = $"{current}/{max}";
		questImage.sprite = sprite;
	}
}
