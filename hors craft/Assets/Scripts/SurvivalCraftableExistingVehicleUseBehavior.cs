// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalCraftableExistingVehicleUseBehavior
using System.Linq;
using UnityEngine;

public class SurvivalCraftableExistingVehicleUseBehavior : AbstractSurvivalCraftableUse
{
	private int existingHealth;

	public override bool CanBeUse(int id)
	{
		return true;
	}

	public override void OnFailed(int id)
	{
	}

	public override void OnSuccess(int id, GameObject usedObject)
	{
		if (existingHealth > 0)
		{
			usedObject.GetComponentInChildren<Health>().hp = existingHealth;
		}
	}

	public override void PrepareToUse(int id)
	{
		SurvivalCraftableExistingVehicleUseBehavior[] array = Object.FindObjectsOfType<SurvivalCraftableExistingVehicleUseBehavior>();
		VehicleController[] array2 = new VehicleController[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array[i].GetComponentInChildren<VehicleController>();
		}
		VehicleController vehicleController = array2.FirstOrDefault((VehicleController t) => t.gameObject.name.Equals(base.gameObject.name));
		if (vehicleController != null)
		{
			vehicleController.GetComponentInParent<CraftableDroper>().hasToDropCraftable = false;
			existingHealth = (int)vehicleController.GetComponentInParent<Health>().hp;
			UnityEngine.Object.Destroy(vehicleController.GetComponentInParent<Health>().gameObject);
			Singleton<PlayerData>.get.playerItems.AddCraftable(id, 1);
		}
	}
}
