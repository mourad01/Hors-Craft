// DecompilerFi decompiler from Assembly-CSharp.dll class: NotInTutorialTutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialRequirements/NotInTutorial")]
public class NotInTutorialTutorialReq : TutorialRequirement
{
	public override bool IsFulfilled()
	{
		return !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL);
	}
}
