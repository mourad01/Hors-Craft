// DecompilerFi decompiler from Assembly-CSharp.dll class: Composite
using System;
using System.Collections.Generic;

public class Composite<T> : AbstractComposite<T>
{
	public List<AbstractComposite<T>> components = new List<AbstractComposite<T>>();

	public Composite()
	{
	}

	public Composite(List<T> elements)
	{
		foreach (T element in elements)
		{
			Add(new CompositeLeaf<T>(element));
		}
	}

	public Composite(params T[] elements)
	{
		foreach (T val in elements)
		{
			Add(new CompositeLeaf<T>(val));
		}
	}

	public override void Add(AbstractComposite<T> component)
	{
		components.Add(component);
	}

	public override void Remove(AbstractComposite<T> component)
	{
		components.Remove(component);
	}

	public override AbstractComposite<T> GetChild(int i)
	{
		return (i >= components.Count) ? null : components[i];
	}

	public override List<T> GetElements()
	{
		List<T> list = new List<T>();
		foreach (AbstractComposite<T> component in components)
		{
			list.AddRange(component.GetElements());
		}
		return list;
	}

	public override T GetFirst()
	{
		if (components.Count == 0)
		{
			return default(T);
		}
		T result = default(T);
		for (int i = 0; i < components.Count; i++)
		{
			if (!result.Equals(default(T)))
			{
				break;
			}
			result = components[i].GetFirst();
		}
		return result;
	}

	public override void Do(Action<T> action)
	{
		foreach (AbstractComposite<T> component in components)
		{
			component.Do(action);
		}
	}
}
