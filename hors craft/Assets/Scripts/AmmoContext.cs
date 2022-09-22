// DecompilerFi decompiler from Assembly-CSharp.dll class: AmmoContext
using System;
using UnityEngine;

public class AmmoContext : SurvivalContext
{
	public struct AmmoPair
	{
		public int maxAmmo;

		public int currentAmmo;
	}

	public AmmoType[] ammoTypes;

	public GameObject[] ammoPrefabs;

	public int[] maxAmmo;

	public int[] currentAmmo;

	public AmmoType currentAmmoType;

	public AmmoPair GetAmmoForType(AmmoType ammoType)
	{
		if (ammoType == null)
		{
			AmmoPair result = default(AmmoPair);
			result.maxAmmo = -1;
			result.currentAmmo = int.MaxValue;
			return result;
		}
		int ammoIndex = GetAmmoIndex(ammoType);
		if (ammoIndex == -1)
		{
			UnityEngine.Debug.LogError("Wrong ammo type: GetAmmoForType: " + ammoType.name);
			AmmoPair result2 = default(AmmoPair);
			result2.maxAmmo = -1;
			result2.currentAmmo = -1;
			return result2;
		}
		AmmoPair result3 = default(AmmoPair);
		result3.maxAmmo = maxAmmo[ammoIndex];
		result3.currentAmmo = currentAmmo[ammoIndex];
		return result3;
	}

	public int GetAmmoIndex(AmmoType ammoType)
	{
		return Array.IndexOf(ammoTypes, ammoType);
	}

	public bool IsAmmoFull(AmmoType ammoType)
	{
		int ammoIndex = GetAmmoIndex(ammoType);
		if (ammoIndex == -1)
		{
			UnityEngine.Debug.LogError("Wrong ammo type: GetAmmoForType");
			return true;
		}
		return currentAmmo[ammoIndex] == maxAmmo[ammoIndex];
	}
}
