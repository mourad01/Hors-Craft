// DecompilerFi decompiler from Assembly-CSharp.dll class: ChooseWorldReq
using Common.Managers;
using Common.Utils;
using Gameplay;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/ChooseWorld")]
public class ChooseWorldReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		return ShowChooseWorld();
	}

	public static bool ShowChooseWorld()
	{
		bool flag = PlayerSession.GetSessionNo() > 1;
		bool flag2 = Manager.Get<ModelManager>().worldsSettings.GetWorldsEnabled() && !Manager.Get<ModelManager>().worldsSettings.GetWorldsUltimateSelection();
		bool flag3 = !ChooseWorldState.haveShownChooseWorld;
		int count = Manager.Get<SavedWorldManager>().GetAllWorlds().Count;
		return flag && flag2 && flag3 && count > 1;
	}
}
