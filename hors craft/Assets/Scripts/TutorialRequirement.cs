// DecompilerFi decompiler from Assembly-CSharp.dll class: TutorialRequirement
using UnityEngine;

public class TutorialRequirement : ScriptableObject
{
	public Fact[] factsToSubscribeTo;

	public virtual void Init()
	{
	}

	public virtual bool IsFulfilled()
	{
		return true;
	}
}
