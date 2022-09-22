// DecompilerFi decompiler from Assembly-CSharp.dll class: FillScrollListFragment
using QuestSystems.Adventure;
using States;
using System.Collections.Generic;
using UnityEngine;

public class FillScrollListFragment : Fragment
{
	public RectTransform prefab;

	public RectTransform contentHolder;

	public virtual void Fill<T>(List<T> objects) where T : GenericObject
	{
		foreach (T @object in objects)
		{
			RectTransform rectTransform = InstantiateElement();
			QuestUIElement component = rectTransform.GetComponent<QuestUIElement>();
			if (component != null)
			{
				component.FillWithInfo("string", null);
			}
		}
	}

	public RectTransform InstantiateElement()
	{
		RectTransform rectTransform = Object.Instantiate(prefab);
		rectTransform.SetParent(contentHolder);
		rectTransform.gameObject.SetActive(value: true);
		rectTransform.localScale = Vector3.one;
		return rectTransform;
	}
}
