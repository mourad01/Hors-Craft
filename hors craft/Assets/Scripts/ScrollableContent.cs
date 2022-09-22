// DecompilerFi decompiler from Assembly-CSharp.dll class: ScrollableContent
using System.Collections.Generic;
using UnityEngine;

public class ScrollableContent : MonoBehaviour
{
	public ScrollableListConstructor constructor;

	public ScrollableListDataProvider adapter;

	public List<ScrollableItemWorker> workers;

	public void InjectScrollableContent(ScrollableListMediator mediator)
	{
		ScrollableListRawContent content = constructor.ConstructList();
		adapter.Init(content);
		mediator.SetAdapter(adapter);
		mediator.AddWorkers(workers);
	}
}
