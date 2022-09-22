// DecompilerFi decompiler from Assembly-CSharp.dll class: CookingLevel
using Common.Managers;
using GameUI;
using UnityEngine;
using UnityEngine.UI;

public class CookingLevel : MonoBehaviour
{
	public Button button;

	public TranslateText text;

	public GameObject[] stars;

	public GameObject lockObject;

	public int levelNumber;

	public int chapterNumber;

	public int numberOfStars;

	private int maxNumberOfStars = 3;

	private ColorManager.ColorCategory previousColorCategory;

	public void SetLevel(int level, int chapter)
	{
		levelNumber = level;
		chapterNumber = chapter;
		text.AddVisitor((string t) => t.Replace("{0}", level.ToString()));
	}

	public void SetStars(int gainedStars)
	{
		numberOfStars = gainedStars;
		for (int i = 1; i <= maxNumberOfStars; i++)
		{
			if (i <= gainedStars)
			{
				stars[i - 1].SetActive(value: true);
			}
			else
			{
				stars[i - 1].SetActive(value: false);
			}
		}
	}

	public void Unlock(bool unlock)
	{
		button.interactable = unlock;
		lockObject.SetActive(!unlock);
		ColorController component = button.GetComponent<ColorController>();
		component.category = ((!unlock) ? ColorManager.ColorCategory.DISABLED_COLOR : ColorManager.ColorCategory.HIGHLIGHT_COLOR);
		previousColorCategory = component.category;
		component.UpdateColor();
		Color color = Manager.Get<ColorManager>().GetColorForCategory(previousColorCategory) * 0.8f;
		color.a = 1f;
		button.GetComponent<Image>().color = color;
	}

	public void Choose(bool choose)
	{
		if (!choose)
		{
			Color color = Manager.Get<ColorManager>().GetColorForCategory(previousColorCategory) * 0.8f;
			color.a = 1f;
			button.GetComponent<Image>().color = color;
		}
		else
		{
			button.GetComponent<Image>().color = Manager.Get<ColorManager>().GetColorForCategory(previousColorCategory);
		}
	}
}
