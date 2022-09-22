// DecompilerFi decompiler from Assembly-CSharp.dll class: CookingChapter
using UnityEngine;

public class CookingChapter : MonoBehaviour
{
	public GameObject levelsHolder;

	public TranslateText text;

	public void SetName(int chapterNumber)
	{
		text.defaultText += chapterNumber;
		text.translationKey += chapterNumber;
		text.ForceRefresh();
	}
}
