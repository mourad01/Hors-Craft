// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestUIElement
using UnityEngine;
using UnityEngine.UI;

public class QuestUIElement : MonoBehaviour
{
	public TranslateText desription;

	public Image image;

	public const string TRANSLATION_KEY = "item.quest.name";

	public virtual void FillWithInfo(string description, Sprite sprite)
	{
		desription.translationKey = GenerateKey(description);
		desription.ForceRefresh();
		if (sprite != null)
		{
			ChangeImage(sprite);
		}
	}

	public virtual void ChangeImage(Sprite sprite)
	{
		image.sprite = sprite;
	}

	public void Changedescription(string newText)
	{
		desription.translationKey = newText;
		desription.ForceRefresh();
	}

	protected virtual string GenerateKey(string id)
	{
		return string.Format("{0}.{1}", "item.quest.name", id);
	}
}
