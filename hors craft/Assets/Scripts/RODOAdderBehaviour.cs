// DecompilerFi decompiler from Assembly-CSharp.dll class: RODOAdderBehaviour
using System;
using UnityEngine;

public class RODOAdderBehaviour : CustomSearchBehaviourAbstract
{
	public string PolicyPopupStatePath;

	public string StartingPolicyShowStatePath;

	public override Action<GameObject, string> GetFunction()
	{
		return Action;
	}

	private void Action(GameObject gameObject, string path)
	{
	}
}
