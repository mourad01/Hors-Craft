// DecompilerFi decompiler from Assembly-CSharp.dll class: ShipCannonGroup
using UnityEngine;

public class ShipCannonGroup : WeaponHandler
{
	[SerializeField]
	public TankCannon[] Cannons
	{
		get;
		private set;
	}

	protected override void Start()
	{
		base.Start();
		Cannons = GetComponentsInChildren<TankCannon>();
	}

	public override void OnPress()
	{
		shooting = true;
		magazineBulletsLeft--;
		TankCannon[] cannons = Cannons;
		foreach (TankCannon tankCannon in cannons)
		{
			if (tankCannon.canShoot)
			{
				tankCannon.OnPress();
			}
		}
	}

	public override void OnRelease()
	{
		if (Cannons != null)
		{
			TankCannon[] cannons = Cannons;
			foreach (TankCannon tankCannon in cannons)
			{
				tankCannon.OnRelease();
			}
		}
	}

	protected override void Update()
	{
		UpdateShooting();
		UpdateReloading();
	}

	public void DeactiveCannons()
	{
		for (int i = 0; i < Cannons.Length; i++)
		{
			Cannons[i].active = false;
		}
	}

	public void ActiveCannons()
	{
		for (int i = 0; i < Cannons.Length; i++)
		{
			Cannons[i].active = true;
		}
	}
}
