// DecompilerFi decompiler from Assembly-CSharp.dll class: IsUnderwaterTutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialRequirements/IsUnderWater")]
public class IsUnderwaterTutorialReq : TutorialRequirement
{
	public override bool IsFulfilled()
	{
		return MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.UNDERWATER);
	}
}
