// DecompilerFi decompiler from Assembly-CSharp.dll class: AbstractComposite
using System;
using System.Collections.Generic;

public abstract class AbstractComposite<T>
{
	public T this[int i]
	{
		get
		{
			List<T> elements = GetElements();
			if (i < elements.Count)
			{
				return elements[i];
			}
			return default(T);
		}
	}

	public abstract void Add(AbstractComposite<T> element);

	public abstract void Remove(AbstractComposite<T> element);

	public abstract AbstractComposite<T> GetChild(int i);

	public abstract List<T> GetElements();

	public abstract T GetFirst();

	public abstract void Do(Action<T> action);
}
