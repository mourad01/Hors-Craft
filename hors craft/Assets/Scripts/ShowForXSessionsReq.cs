// DecompilerFi decompiler from Assembly-CSharp.dll class: ShowForXSessionsReq
using Common.Utils;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/ShowForXSessionsReq")]
public class ShowForXSessionsReq : InitialPopupRequirements
{
	public int times = 1;

	public override bool CanBeShown()
	{
		int sessionNo = PlayerSession.GetSessionNo();
		return sessionNo <= times;
	}
}
