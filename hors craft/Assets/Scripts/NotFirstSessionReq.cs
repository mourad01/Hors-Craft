// DecompilerFi decompiler from Assembly-CSharp.dll class: NotFirstSessionReq
using Common.Utils;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Initial Popups/Not First Session")]
public class NotFirstSessionReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		return PlayerSession.GetSessionNo() > 1;
	}
}
