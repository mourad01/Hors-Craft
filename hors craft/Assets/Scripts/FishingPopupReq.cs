// DecompilerFi decompiler from Assembly-CSharp.dll class: FishingPopupReq
using Common.Managers;
using Gameplay;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/FishingPopup")]
public class FishingPopupReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		if (Manager.Get<ModelManager>().fishingSettings.IsFishingEnabled())
		{
			int @int = PlayerPrefs.GetInt("timeSinceStartup", 0);
			if (Manager.Get<ModelManager>().fishingSettings.HasToShowFishingReminder(@int))
			{
				return true;
			}
		}
		return false;
	}
}
