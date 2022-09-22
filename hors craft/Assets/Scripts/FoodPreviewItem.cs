// DecompilerFi decompiler from Assembly-CSharp.dll class: FoodPreviewItem
using UnityEngine;
using UnityEngine.UI;

public class FoodPreviewItem : MonoBehaviour
{
	public TranslateText prestigeRequired;

	public Text stock;

	public Image image;

	public GameObject lockedGO;

	public void InitUnlocked(Sprite icon, int count, int max)
	{
		lockedGO.SetActive(value: false);
		image.sprite = icon;
		stock.gameObject.SetActive(value: true);
		stock.text = count + "/" + max;
		prestigeRequired.gameObject.SetActive(value: false);
	}

	public void InitLocked(Sprite icon, int prestige)
	{
		lockedGO.SetActive(value: true);
		image.sprite = icon;
		stock.gameObject.SetActive(value: false);
		prestigeRequired.gameObject.SetActive(value: true);
		prestigeRequired.AddVisitor((string t) => t.Replace("{0}", prestige.ToString()));
	}
}
