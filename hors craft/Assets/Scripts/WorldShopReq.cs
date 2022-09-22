// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldShopReq
using States;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/World Shop")]
public class WorldShopReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		return !WorldShopState.stateVisitedAtThisSession;
	}
}
