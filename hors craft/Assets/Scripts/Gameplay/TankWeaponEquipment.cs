// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.TankWeaponEquipment
using Common.Audio;
using Common.Managers;
using Gameplay.Audio;
using System;
using System.Collections;
using UnityEngine;

namespace Gameplay
{
	public class TankWeaponEquipment : WeaponEquipment
	{
		public TankWeaponEquipment(GameObject[] weapons, ArmedPlayer player)
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
			Transform handle = armedPlayer.handle;
			IEnumerator enumerator = handle.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			Array.ForEach(base.weapons, delegate(GameObject w)
			{
				w.GetComponent<TankWeapon>().active = false;
			});
			weapon.GetComponent<TankWeapon>().active = true;
			ConfigOnEquip(weapon);
		}

		protected override void EquipWeaponHandler(GameObject instance, WeaponHandler weaponHandler)
		{
			weaponHandler.shotAnimation = armedPlayer.shotAnimation;
		}
	}
}
