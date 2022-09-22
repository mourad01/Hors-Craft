// DecompilerFi decompiler from Assembly-CSharp.dll class: CompositeLeaf
using System;
using System.Collections.Generic;
using UnityEngine;

public class CompositeLeaf<T> : AbstractComposite<T>
{
	public T value;

	public CompositeLeaf(T val)
	{
		value = val;
	}

	public override void Add(AbstractComposite<T> component)
	{
	}

	public override void Remove(AbstractComposite<T> component)
	{
	}

	public override AbstractComposite<T> GetChild(int i)
	{
		return this;
	}

	public override List<T> GetElements()
	{
		return value.AsList();
	}

	public override T GetFirst()
	{
		UnityEngine.Debug.LogError("GET FIRST: " + (value == null));
		return value;
	}

	public override void Do(Action<T> action)
	{
		action(value);
	}
}
