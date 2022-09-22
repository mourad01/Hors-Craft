// DecompilerFi decompiler from Assembly-CSharp.dll class: CustomSearchBehaviourAbstract
using System;
using UnityEngine;

[Serializable]
public abstract class CustomSearchBehaviourAbstract : MonoBehaviour
{
	public abstract Action<GameObject, string> GetFunction();

	public virtual Action GetLastAction()
	{
		return delegate
		{
		};
	}
}
