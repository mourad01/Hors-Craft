// DecompilerFi decompiler from Assembly-CSharp.dll class: ScrollableListDataProvider
using System.Collections.Generic;
using UnityEngine;

public abstract class ScrollableListDataProvider : MonoBehaviour
{
	public abstract void Init(ScrollableListRawContent content);

	public abstract List<ScrollableListElement> PrepareData();
}
