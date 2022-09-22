// DecompilerFi decompiler from Assembly-CSharp.dll class: OnGameplayStartedTutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialRequirements/OnGameplayStarted")]
public class OnGameplayStartedTutorialReq : TutorialRequirement
{
	public override bool IsFulfilled()
	{
		return MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.GAMEPLAY_STARTED);
	}
}
