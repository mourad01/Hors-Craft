// DecompilerFi decompiler from Assembly-CSharp.dll class: AddGameplayModuleToAllGameplays
using States;
using System;
using UnityEngine;

public class AddGameplayModuleToAllGameplays : CustomSearchBehaviourAbstract
{
	public GameplayState.Substates substateType;

	public string module;

	public override Action<GameObject, string> GetFunction()
	{
		return Action;
	}

	private void Action(GameObject gameObject, string path)
	{
	}
}
