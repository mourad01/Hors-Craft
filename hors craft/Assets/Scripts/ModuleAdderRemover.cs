// DecompilerFi decompiler from Assembly-CSharp.dll class: ModuleAdderRemover
using System;
using System.Collections.Generic;
using UnityEngine;

public class ModuleAdderRemover : CustomSearchBehaviourAbstract
{
	public List<string> gameplaySubstates = new List<string>();

	public List<string> modulesToAdd = new List<string>();

	public List<string> modulesToRemove = new List<string>();

	public override Action<GameObject, string> GetFunction()
	{
		return Action;
	}

	private void Action(GameObject gameObject, string path)
	{
	}
}
