// DecompilerFi decompiler from Assembly-CSharp.dll class: ChangeOrganicBehaviour
using System;
using UnityEngine;

public class ChangeOrganicBehaviour : CustomSearchBehaviourAbstract
{
	public Mesh orginalMesh;

	public Mesh newMesh;

	public override Action<GameObject, string> GetFunction()
	{
		return Action;
	}

	private void Action(GameObject gameObject, string path)
	{
	}
}
