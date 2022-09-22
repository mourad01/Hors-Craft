// DecompilerFi decompiler from Assembly-CSharp.dll class: MPCEtutorialReq
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/MCPE Turorial Req")]
public class MPCEtutorialReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL))
		{
			return false;
		}
		return true;
	}
}
