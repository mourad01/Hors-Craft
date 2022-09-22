// DecompilerFi decompiler from Assembly-CSharp.dll class: TankMark
using com.ootii.Cameras;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankMark : Tank
{
	private TankWeapon[] allWeapons;

	private TankWeapon[] cannonWeapons;

	private Transform cameraTransform => CameraController.instance.MainCamera.transform;

	protected override void Awake()
	{
		base.Awake();
		WeaponizedVehicle componentInParent = GetComponentInParent<WeaponizedVehicle>();
		allWeapons = componentInParent.weapons;
		cannonWeapons = (from w in allWeapons
			where w is TankCannon
			select w).ToArray();
		List<TankWeapon> list = new List<TankWeapon>();
		list.Add(cannonWeapons[0]);
		list.Add(componentInParent.weapons.First((TankWeapon w) => w is TankMachineGun));
		List<TankWeapon> list2 = list;
		componentInParent.weapons = list2.ToArray();
	}

	protected override void Update()
	{
		base.Update();
		UpdateTurrets();
	}

	private void UpdateTurrets()
	{
		TankWeapon tankWeapon = allWeapons.FirstOrDefault((TankWeapon t) => t.active);
		if (!(tankWeapon is TankMachineGun))
		{
			Array.ForEach(cannonWeapons, delegate(TankWeapon cw)
			{
				cw.active = false;
			});
			TankWeapon tankWeapon2 = (from cw in cannonWeapons
				orderby Vector2.Dot(cameraTransform.forward.XZ().normalized, cw.transform.forward.XZ().normalized) descending
				select cw).First();
			if (tankWeapon == null || Vector2.Dot(tankWeapon.transform.forward.XZ(), tankWeapon2.transform.forward.XZ()) < 0.99f)
			{
				tankWeapon2.active = true;
				player.GetComponentInParent<ArmedPlayer>().weapon = tankWeapon2;
			}
			else
			{
				tankWeapon.active = true;
			}
			Array.ForEach(cannonWeapons, delegate(TankWeapon w)
			{
				w.UpdateActiveWeapon();
			});
		}
	}
}
