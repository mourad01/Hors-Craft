// DecompilerFi decompiler from Assembly-CSharp.dll class: IsMCPESteeringTutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialRequirements/IsMCPESteering")]
public class IsMCPESteeringTutorialReq : TutorialRequirement
{
	public override bool IsFulfilled()
	{
		return MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING);
	}
}
