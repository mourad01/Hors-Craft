// DecompilerFi decompiler from Assembly-CSharp.dll class: ScrollableListMediator
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScrollableListMediator : MonoBehaviour
{
	private class NumeratorWorker : IWorker<ScrollableListElement>
	{
		private int number;

		public void Work(ScrollableListElement element)
		{
			element.position = number++;
		}
	}

	public GameObject prefab;

	public List<ScrollableItemWorker> workers;

	public float coroutineSpawnDelay = 0.1f;

	protected ScrollableListDataProvider provider;

	private List<ScrollableListElement> elements = new List<ScrollableListElement>();

	private NumeratorWorker numerator = new NumeratorWorker();

	private Coroutine spawnCoroutine;

	public virtual void SetAdapter(ScrollableListDataProvider provider)
	{
		this.provider = provider;
	}

	public virtual void AddWorkers(List<ScrollableItemWorker> workers)
	{
		this.workers.AddRange(workers);
	}

	public virtual void CreateContent(bool useCoroutine = true)
	{
		numerator = new NumeratorWorker();
		elements = provider.PrepareData();
		int? num = (elements != null) ? new int?(elements.Count) : null;
		UnityEngine.Debug.Log($"Found {(num.HasValue ? num.Value : 0)} elements");
		SpawnElements(elements, useCoroutine);
	}

	private void SpawnElements(List<ScrollableListElement> elements, bool useCoroutine = true)
	{
		if (useCoroutine)
		{
			spawnCoroutine = StartCoroutine(SpawnElements(elements));
		}
		else
		{
			foreach (ScrollableListElement element in elements)
			{
				SpawnElement(element);
			}
		}
	}

	private void SpawnElement(ScrollableListElement element)
	{
		ScrollableItemConnector connector = CreateElement(element);
		element.connector = connector;
		numerator.Work(element);
		workers.ForEach(delegate(ScrollableItemWorker w)
		{
			w.Work(element);
		});
	}

	public abstract ScrollableItemConnector CreateElement(ScrollableListElement element);

	public abstract void RefreshElement(ScrollableListElement element);

	protected virtual void Update()
	{
		int i;
		for (i = 0; i < elements.Count; i++)
		{
			if (elements[i].isDirty)
			{
				RefreshElement(elements[i]);
				workers.ForEach(delegate(ScrollableItemWorker w)
				{
					w.Work(elements[i]);
				});
				elements[i].isDirty = false;
			}
		}
	}

	public virtual void Show()
	{
		List<ScrollableListElement> list = new List<ScrollableListElement>();
		int i;
		for (i = 0; i < elements.Count; i++)
		{
			if (elements[i].connector != null)
			{
				elements[i].connector.gameObject.SetActive(value: true);
				workers.ForEach(delegate(ScrollableItemWorker w)
				{
					w.Work(elements[i]);
				});
			}
			else
			{
				list.Add(elements[i]);
			}
		}
		if (spawnCoroutine == null)
		{
			SpawnElements(list, useCoroutine: true);
		}
	}

	public virtual void Hide()
	{
		for (int i = 0; i < elements.Count; i++)
		{
			elements[i].connector?.gameObject.SetActive(value: false);
		}
		StopAllCoroutines();
		spawnCoroutine = null;
	}

	public virtual void Destroy()
	{
		StopAllCoroutines();
		spawnCoroutine = null;
		for (int i = 0; i < elements.Count; i++)
		{
			if (elements[i].connector != null)
			{
				UnityEngine.Object.Destroy(elements[i].connector.gameObject);
			}
		}
		elements = new List<ScrollableListElement>();
	}

	private IEnumerator SpawnElements(List<ScrollableListElement> elements)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			SpawnElement(elements[i]);
			yield return new WaitForSecondsRealtime(coroutineSpawnDelay);
		}
		spawnCoroutine = null;
	}
}
