// DecompilerFi decompiler from Assembly-CSharp.dll class: ToolkitEnabledTutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialRequirements/ToolkitEnabled")]
public class ToolkitEnabledTutorialReq : TutorialRequirement
{
	public override bool IsFulfilled()
	{
		return MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TOOLKIT_ENABLED);
	}
}
