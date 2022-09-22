// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalVehicle
using Common.Managers;
using Gameplay;
using System;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class SurvivalVehicle : MonoBehaviour
{
	public bool killPlayerOnDeath;

	private Health _health;

	private Health health => _health ?? (_health = GetComponentInParent<Health>());

	private void Awake()
	{
		VehicleController componentInChildren = GetComponentInChildren<VehicleController>();
		if (componentInChildren != null)
		{
			VehicleController vehicleController = componentInChildren;
			vehicleController.OnVehicleActivated = (Action<GameObject>)Delegate.Combine(vehicleController.OnVehicleActivated, new Action<GameObject>(UpdateGameplay));
		}
	}

	private void UpdateGameplay(GameObject player)
	{
		if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.SURVIVAL_MODE_ENABLED))
		{
			player.GetComponent<ArmedPlayer>().OnVehicleEnter(GetComponentInChildren<VehicleController>());
		}
	}

	private void Update()
	{
		if ((bool)health && health.IsDead())
		{
			Die();
		}
		else
		{
			UpdateFallingUnderMap();
		}
	}

	private void UpdateFallingUnderMap()
	{
		Vector3 position = base.transform.position;
		if (position.y < -64f)
		{
			CraftableList craftableListInstance = Manager.Get<CraftingManager>().GetCraftableListInstance();
			Craftable craftable = craftableListInstance.craftableList.FirstOrDefault((Craftable c) => c.customCraftableObject != null && base.gameObject.name.Contains(c.customCraftableObject.name));
			if (craftable != null)
			{
				PlayerPrefs.SetFloat("tank" + craftable.id, health.hp);
				Singleton<PlayerData>.get.playerItems.AddCraftable(craftable.id, 1);
			}
			Die();
		}
	}

	private void Die()
	{
		CameraEventsSender componentInChildren = GetComponentInChildren<CameraEventsSender>();
		if (componentInChildren != null)
		{
			componentInChildren.GetHoverAction<VehicleHoverAction>().OnVehicleUse();
		}
		UnityEngine.Object.Destroy(health.gameObject);
		Health componentInChildren2 = PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<Health>();
		if (killPlayerOnDeath && componentInChildren2 != null)
		{
			componentInChildren2.hp -= componentInChildren2.maxHp;
		}
	}

	private void AddToCraftableIfNotAlreadyThere()
	{
		CraftableList craftableListInstance = Manager.Get<CraftingManager>().GetCraftableListInstance();
		Craftable craftable = craftableListInstance.craftableList.FirstOrDefault((Craftable c) => c.customCraftableObject != null && base.gameObject.name.Contains(c.customCraftableObject.name));
		if (craftable != null)
		{
			int value = 0;
			if (!Singleton<PlayerData>.get.playerItems.TryGetCraftableValue(craftable.id, out value) || value == 0)
			{
				Singleton<PlayerData>.get.playerItems.AddCraftable(craftable.id, 1);
			}
		}
	}
}
