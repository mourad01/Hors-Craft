// DecompilerFi decompiler from Assembly-CSharp.dll class: ScrollableItemWorker
using UnityEngine;

public abstract class ScrollableItemWorker : MonoBehaviour, IWorker<ScrollableListElement>
{
	public abstract void Work(ScrollableListElement element);
}
