// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.MechCollectiveWeapon
using Common.Managers;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class MechCollectiveWeapon : CollectiveWeapon, IFormChangeListener
	{
		private SurvivalAmmoCountContext ammoCountContext;

		private void Start()
		{
			ammoCountContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalAmmoCountContext>(Fact.SURVIVAL_AMMO_COUNT);
			ammoCountContext = (ammoCountContext ?? new SurvivalAmmoCountContext());
		}

		public void OnFormChange()
		{
			if (Application.isPlaying)
			{
				AssignWeapons();
			}
		}

		private void Update()
		{
			if (weapons.Count == 0)
			{
				AssignWeapons();
			}
		}

		private void AssignWeapons()
		{
			weapons = GetComponentsInChildren<WeaponHandler>().ToList();
			(from w in weapons
				where w is TankWeapon
				select w).ToList().ForEach(delegate(WeaponHandler w)
			{
				(w as TankWeapon).active = true;
			});
			if (weapons.Count > 0)
			{
				ArmedPlayer componentInParent = GetComponentInParent<ArmedPlayer>();
				componentInParent.DestroyCurrentWeaponEquipment();
				componentInParent.EquipWeaponEquipment(new CollectiveWeaponEquipment(new GameObject[1]
				{
					base.gameObject
				}, componentInParent));
				Manager.Get<StateMachineManager>().gameObject.GetComponentInChildren<GameplaySubstateMechAttackButton>().ChangeSprites();
			}
		}

		protected void UpdateAmmoCountContext()
		{
			ammoCountContext.isReloading = false;
			ammoCountContext.customSprite = sprite;
			if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.SURVIVAL_AMMO_COUNT, ammoCountContext))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_AMMO_COUNT);
			}
		}
	}
}
