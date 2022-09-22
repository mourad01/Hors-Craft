// DecompilerFi decompiler from Assembly-CSharp.dll class: DefaultListMediator
using UnityEngine;
using UnityEngine.UI;

public class DefaultListMediator : ScrollableListMediator
{
	public ScrollRect scroll;

	public RectTransform content;

	public override ScrollableItemConnector CreateElement(ScrollableListElement element)
	{
		GameObject gameObject = Object.Instantiate(prefab, content, worldPositionStays: false);
		return gameObject.GetComponent<ScrollableItemConnector>();
	}

	public override void Show()
	{
		content.gameObject.SetActive(value: true);
		scroll.content = content;
		base.Show();
	}

	public override void Hide()
	{
		base.Hide();
		content.gameObject.SetActive(value: false);
	}

	public override void RefreshElement(ScrollableListElement element)
	{
	}
}
