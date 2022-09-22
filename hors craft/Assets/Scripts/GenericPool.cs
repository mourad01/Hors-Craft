// DecompilerFi decompiler from Assembly-CSharp.dll class: GenericPool
using System;
using System.Collections.Generic;

public class GenericPool<T> where T : IPoolAble
{
	private Stack<T> pool;

	private Func<T> itemConstructor;

	private int currentSize;

	public GenericPool(int startSize = 521)
	{
		InitPool(startSize);
	}

	public GenericPool(Func<T> itemConstructor, int startSize = 521)
	{
		this.itemConstructor = itemConstructor;
		InitPool(startSize);
	}

	public T Take()
	{
		T result = (pool.Count <= 0) ? NewObject() : pool.Pop();
		result.Activate();
		return result;
	}

	public void Release(T item)
	{
		item.Deactivate();
		if (!pool.Contains(item))
		{
			pool.Push(item);
		}
	}

	public void Dispose()
	{
		if (pool != null)
		{
			while (pool.Count > 0)
			{
				pool.Pop();
			}
			pool = null;
			itemConstructor = null;
		}
	}

	private void InitPool(int startSize)
	{
		currentSize = startSize;
		pool = new Stack<T>(startSize);
		for (int i = 0; i < startSize; i++)
		{
			T item = NewObject();
			item.Deactivate();
			pool.Push(item);
		}
	}

	private T NewObject()
	{
		if (itemConstructor != null)
		{
			return itemConstructor();
		}
		return default(T);
	}

	public override string ToString()
	{
		return $"Pool type {typeof(T).FullName}, has {(pool ?? new Stack<T>()).Count}/{currentSize} elements";
	}
}
