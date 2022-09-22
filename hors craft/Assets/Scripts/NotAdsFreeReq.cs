// DecompilerFi decompiler from Assembly-CSharp.dll class: NotAdsFreeReq
using Common.Managers;
using Gameplay;
using TsgCommon;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Not Ads Free")]
public class NotAdsFreeReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		return !TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.IsAdsFree || Manager.Get<ModelManager>().blocksUnlocking.IsRarityBlocksNoAdsEnabled();
	}
}
