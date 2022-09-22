// DecompilerFi decompiler from Assembly-CSharp.dll class: IWeaponProvider
using Gameplay;

public interface IWeaponProvider
{
	Weapon[] GetWeapons();

	WeaponEquipment ConstructWeaponEquipment(ArmedPlayer player);
}
