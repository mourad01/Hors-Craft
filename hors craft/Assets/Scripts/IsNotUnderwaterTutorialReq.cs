// DecompilerFi decompiler from Assembly-CSharp.dll class: IsNotUnderwaterTutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialRequirements/NotUnderWater")]
public class IsNotUnderwaterTutorialReq : TutorialRequirement
{
	public override bool IsFulfilled()
	{
		return !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.UNDERWATER);
	}
}
