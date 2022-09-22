// DecompilerFi decompiler from Assembly-CSharp.dll class: ScriptableCutscene
using UnityEngine;

public abstract class ScriptableCutscene : ScriptableObject
{
	public abstract void Show();

	public abstract bool EndCondition();

	public abstract void OnEnd();
}
