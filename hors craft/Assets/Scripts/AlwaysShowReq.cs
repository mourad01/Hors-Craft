// DecompilerFi decompiler from Assembly-CSharp.dll class: AlwaysShowReq
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/always show")]
public class AlwaysShowReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		return true;
	}
}
