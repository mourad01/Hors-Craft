// DecompilerFi decompiler from Assembly-CSharp.dll class: TabRarityContainer
using States;
using UnityEngine;

public class TabRarityContainer : MonoBehaviour
{
	public BlocksFragment.BlocksRarityCategoryTabController[] tabs;

	[SerializeField]
	private RectTransform frontTab;

	[SerializeField]
	private RectTransform backTab;

	public void SetBarOffset(float offset)
	{
		RectTransform rectTransform = frontTab;
		Vector2 offsetMax = frontTab.offsetMax;
		float x = offsetMax.x;
		Vector2 offsetMax2 = frontTab.offsetMax;
		rectTransform.offsetMax = new Vector2(x, offsetMax2.y + offset);
		RectTransform rectTransform2 = backTab;
		Vector2 offsetMax3 = backTab.offsetMax;
		float x2 = offsetMax3.x;
		Vector2 offsetMax4 = backTab.offsetMax;
		rectTransform2.offsetMax = new Vector2(x2, offsetMax4.y + offset);
	}
}
