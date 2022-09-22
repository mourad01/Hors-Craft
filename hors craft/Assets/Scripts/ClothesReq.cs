// DecompilerFi decompiler from Assembly-CSharp.dll class: ClothesReq
using Common.Managers;
using Gameplay;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Clothes")]
public class ClothesReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		ModelManager modelManager = Manager.Get<ModelManager>();
		return modelManager.clothesSetting.GetClothesEnabled() && modelManager.clothesSetting.GetClothesPopupEnabled();
	}
}
