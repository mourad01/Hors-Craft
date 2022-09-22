// DecompilerFi decompiler from Assembly-CSharp.dll class: AddScriptsToGameObject
using System;
using UnityEngine;

public class AddScriptsToGameObject : CustomSearchBehaviourAbstract
{
	public override Action<GameObject, string> GetFunction()
	{
		return Action;
	}

	private void Action(GameObject gameObject, string path)
	{
	}
}
