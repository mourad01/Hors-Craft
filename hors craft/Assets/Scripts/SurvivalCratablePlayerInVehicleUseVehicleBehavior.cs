// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalCratablePlayerInVehicleUseVehicleBehavior
using Common.Managers;
using UnityEngine;

public class SurvivalCratablePlayerInVehicleUseVehicleBehavior : AbstractSurvivalCraftableUse
{
	public override bool CanBeUse(int id)
	{
		GameObject gameObject = PlayerGraphic.GetControlledPlayerInstance().gameObject;
		if (gameObject.GetComponentInParent<VehicleController>() == null)
		{
			return gameObject.GetComponentInParent<VehicleController>() == null;
		}
		string key = "has." + GetComponent<SurvivalCraftable>().Name + ".already";
		string fallback = "Leave your current " + GetComponent<SurvivalCraftable>().Name + " first!";
		string text = Manager.Get<TranslationsManager>().GetText(key, fallback).ToUpper();
		Manager.Get<ToastManager>().ShowToast(text);
		return gameObject.GetComponentInParent<VehicleController>() == null;
	}

	public override void OnFailed(int id)
	{
	}

	public override void OnSuccess(int id, GameObject usedObject)
	{
	}

	public override void PrepareToUse(int id)
	{
	}
}
