// DecompilerFi decompiler from Assembly-CSharp.dll class: NotInDevModeTutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialRequirements/NotInDevMode")]
public class NotInDevModeTutorialReq : TutorialRequirement
{
	public override bool IsFulfilled()
	{
		return !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.DEV_ENABLED);
	}
}
