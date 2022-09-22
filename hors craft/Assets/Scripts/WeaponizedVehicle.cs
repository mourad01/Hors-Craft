// DecompilerFi decompiler from Assembly-CSharp.dll class: WeaponizedVehicle
using Gameplay;
using System;
using System.Linq;
using UnityEngine;

public class WeaponizedVehicle : MonoBehaviour, IWeaponProvider
{
	public TankWeapon[] weapons
	{
		get;
		set;
	}

	private void Awake()
	{
		weapons = (from a in base.gameObject.GetComponentsInChildren<TankWeapon>()
			orderby (!(a is TankCannon)) ? 1 : 0
			select a).ToArray();
		UnityEngine.Debug.Log(weapons.Length);
		VehicleController componentInChildren = GetComponentInChildren<VehicleController>();
		if (componentInChildren != null)
		{
			VehicleController vehicleController = componentInChildren;
			vehicleController.OnVehicleUnactivated = (Action)Delegate.Combine(vehicleController.OnVehicleUnactivated, new Action(DeactivateWeapons));
		}
	}

	private void DeactivateWeapons()
	{
		TankWeapon[] weapons = this.weapons;
		foreach (TankWeapon tankWeapon in weapons)
		{
			tankWeapon.active = false;
		}
		if (this.weapons.Length > 0 && this.weapons[0].crosshairToMove != null)
		{
			this.weapons[0].crosshairToMove.transform.position = Vector3.zero;
		}
	}

	public Weapon[] GetWeapons()
	{
		return weapons;
	}

	public WeaponEquipment ConstructWeaponEquipment(ArmedPlayer player)
	{
		return new TankWeaponEquipment((from w in GetWeapons()
			select w.gameObject).ToArray(), player);
	}
}
