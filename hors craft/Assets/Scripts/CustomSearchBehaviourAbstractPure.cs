// DecompilerFi decompiler from Assembly-CSharp.dll class: CustomSearchBehaviourAbstractPure
using System;
using UnityEngine;

[Serializable]
public abstract class CustomSearchBehaviourAbstractPure
{
	public abstract Action<GameObject, string> GetFunction();

	public virtual Action GetLastAction()
	{
		return delegate
		{
		};
	}
}
