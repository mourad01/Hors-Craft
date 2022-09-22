// DecompilerFi decompiler from Assembly-CSharp.dll class: ChangeMaterial
using System;
using UnityEngine;

public class ChangeMaterial : CustomSearchBehaviourAbstract
{
	public Material material;

	public override Action<GameObject, string> GetFunction()
	{
		return Action;
	}

	private void Action(GameObject gameObject, string path)
	{
	}
}
