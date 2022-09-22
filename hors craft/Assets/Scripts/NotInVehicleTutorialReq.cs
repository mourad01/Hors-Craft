// DecompilerFi decompiler from Assembly-CSharp.dll class: NotInVehicleTutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialRequirements/NotInVehicle")]
public class NotInVehicleTutorialReq : TutorialRequirement
{
	public override bool IsFulfilled()
	{
		return !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_VEHICLE);
	}
}
