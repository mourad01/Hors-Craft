// DecompilerFi decompiler from Assembly-CSharp.dll class: CollectiveWeapon
using System.Collections.Generic;

public class CollectiveWeapon : Weapon
{
	public List<WeaponHandler> weapons = new List<WeaponHandler>();

	public override void OnPress()
	{
		foreach (WeaponHandler weapon in weapons)
		{
			weapon.OnPress();
		}
	}

	public override void OnRelease()
	{
		foreach (WeaponHandler weapon in weapons)
		{
			weapon.OnRelease();
		}
	}
}
