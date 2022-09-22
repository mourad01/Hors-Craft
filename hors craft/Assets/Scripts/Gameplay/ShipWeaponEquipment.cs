// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ShipWeaponEquipment
using Common.Audio;
using Common.Managers;
using Gameplay.Audio;
using UnityEngine;

namespace Gameplay
{
	public class ShipWeaponEquipment : WeaponEquipment
	{
		public ShipWeaponEquipment(GameObject[] weapons, ArmedPlayer player)
			: base(weapons, player)
		{
		}

		protected override bool CanEquipWeapon(int index)
		{
			return true;
		}

		public override void Equip(GameObject weapon)
		{
			Gameplay.Audio.MixersManager.Play(Manager.Get<ClipsManager>().GetClipFor(GameSound.BOW_ARM));
			ConfigOnEquip(weapon);
		}

		protected override void EquipWeaponHandler(GameObject instance, WeaponHandler weaponHandler)
		{
			weaponHandler.shotAnimation = armedPlayer.shotAnimation;
		}
	}
}
