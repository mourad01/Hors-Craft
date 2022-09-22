// DecompilerFi decompiler from Assembly-CSharp.dll class: AbstractSurvivalCraftableUse
using UnityEngine;

public abstract class AbstractSurvivalCraftableUse : MonoBehaviour
{
	public abstract bool CanBeUse(int id);

	public abstract void PrepareToUse(int id);

	public abstract void OnFailed(int id);

	public abstract void OnSuccess(int id, GameObject usedObject);
}
