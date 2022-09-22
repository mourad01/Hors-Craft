// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ArmedPlayer
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using States;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class ArmedPlayer : MonoBehaviour, IGameCallbacksListener
	{
		private Transform _handle;

		public CameraFovAnimation shotAnimation;

		[HideInInspector]
		public Weapon weapon;

		public PlayerStats[] statsToReset;

		public static bool died;

		private Health _health;

		private Inventory inventory;

		private List<WeaponEquipment> weaponEquipments = new List<WeaponEquipment>();

		private RenderTexture renderTexture;

		private int _activeEquipment;

		private Action onPlayerDied;

		public Transform handle => _handle ?? (_handle = CameraController.instance.MainCamera.transform.GetChild(0));

		private Health health => _health ?? (_health = GetComponent<Health>());

		private int activeWeaponEquipment
		{
			get
			{
				return _activeEquipment;
			}
			set
			{
				_activeEquipment = value;
				UpdateWeaponCountFact();
			}
		}

		private void OnEnable()
		{
			if (!(this.health == null))
			{
				Health health = this.health;
				health.onDieAction = (Action)Delegate.Combine(health.onDieAction, new Action(Die));
			}
		}

		private void OnDisable()
		{
			Health health = this.health;
			health.onDieAction = (Action)Delegate.Remove(health.onDieAction, new Action(Die));
		}

		private void Start()
		{
			shotAnimation = CameraController.instance.MainCamera.gameObject.AddComponent<CameraFovAnimation>();
			Manager.Get<SurvivalManager>().RegisterPlayer(base.gameObject);
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
			weaponEquipments.Add(new WeaponEquipment(SurvivalContextsBroadcaster.instance.GetContext<WeaponsContext>().GetPrefabs(), this));
			if (weaponEquipments.Count > 0)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.HAS_MORE_THAN_ONE_WEAPON, HasMoreThan1Weapons());
			}
			inventory = GetComponent<Inventory>();
			health.onHitAction += OnHitEffects;
			GetComponent<Looting>().onCollectAction = OnCollectEffects;
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.SURVIVAL_SHOOTING_CONFIG, new ShootingPanelContext
			{
				onLeftButton = SwitchToPreviousWeapon,
				onRightButton = SwitchToNextWeapon,
				onShoot = OnShootPress
			});
			activeWeaponEquipment = 0;
		}

		public bool HasMoreThan1Weapons()
		{
			if (activeWeaponEquipment < 0 || activeWeaponEquipment >= weaponEquipments.Count)
			{
				return false;
			}
			return weaponEquipments[activeWeaponEquipment].HasMultipleActiveWeapons();
		}

		public void OnShootPress(bool pressed)
		{
			if (weapon == null)
			{
				weapon = GetComponentInChildren<Weapon>();
			}
			if (weapon != null)
			{
				if (pressed)
				{
					weapon.OnPress();
				}
				else
				{
					weapon.OnRelease();
				}
				UpdateWeaponCountFact();
			}
		}

		public void SwitchToPreviousWeapon()
		{
			weaponEquipments[activeWeaponEquipment].SwitchToPreviousWeapon();
		}

		public void SwitchToNextWeapon()
		{
			weaponEquipments[activeWeaponEquipment].SwitchToNextWeapon();
		}

		private void OnHitEffects(Vector3 dir)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.GOT_HIT, new GotHitContext
			{
				angle = Mathf.Atan2(0f - dir.z, 0f - dir.x) * 57.29578f
			});
			HitShakeEffect componentInChildren = GetComponentInChildren<HitShakeEffect>();
			if (componentInChildren != null)
			{
				componentInChildren.StartShake();
			}
			PlaySound(GameSound.ZOMBIE_HIT);
		}

		private void OnCollectEffects(CollectibleVoxel.Type type)
		{
			switch (type)
			{
			case CollectibleVoxel.Type.HEALTH:
				PlaySound(GameSound.HP_COLLECT);
				break;
			case CollectibleVoxel.Type.INVENTORY_ITEM:
				PlaySound(GameSound.ARROW_COLLECT);
				break;
			case CollectibleVoxel.Type.FOOD:
				PlaySound(GameSound.HP_COLLECT);
				break;
			}
		}

		public void EquipDefaultWeapon()
		{
			SurvivalPhaseContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE);
			if (factContext == null || !factContext.isCombat)
			{
				Disarm();
			}
			else if (weaponEquipments.Count > 0)
			{
				weaponEquipments[activeWeaponEquipment].EquipDefaultWeapon();
			}
		}

		public void Disarm()
		{
			PlaySound(GameSound.BOW_DISARM);
			GetComponentInChildren<PlayerGraphic>().ShowHands();
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.WEAPON_EQUIPPED);
			for (int i = 0; i < handle.childCount; i++)
			{
				UnityEngine.Object.Destroy(handle.GetChild(i).gameObject);
			}
		}

		public void EquipWeaponWithId(int id)
		{
			weaponEquipments[activeWeaponEquipment].EquipWeaponWithId(id);
			UpdateWeaponCountFact();
		}

		public Weapon GetWeapon(int id)
		{
			return weaponEquipments.FirstOrDefault((WeaponEquipment we) => we.GetWeapon(id) != null).GetWeapon(id);
		}

		public void OnVehicleEnter(VehicleController vehicle)
		{
			IWeaponProvider componentInParent = vehicle.gameObject.GetComponentInParent<IWeaponProvider>();
			if (componentInParent != null)
			{
				EquipWithWeaponProvider(componentInParent);
				health.invulnerability = true;
			}
			else
			{
				health.invulnerability = false;
			}
		}

		public void EquipWithWeaponProvider(IWeaponProvider provider, bool setAsActive = true)
		{
			WeaponEquipment equipment = provider.ConstructWeaponEquipment(this);
			EquipWeaponEquipment(equipment, setAsActive);
		}

		public void EquipWeaponEquipment(WeaponEquipment equipment, bool setAsActive = true)
		{
			weaponEquipments.Add(equipment);
			if (setAsActive)
			{
				activeWeaponEquipment = weaponEquipments.Count - 1;
				EquipDefaultWeapon();
			}
		}

		public void DestroyCurrentWeaponEquipment()
		{
			if (weaponEquipments.Count > 0)
			{
				weaponEquipments.RemoveAt(weaponEquipments.Count - 1);
				activeWeaponEquipment = 0;
				EquipDefaultWeapon();
			}
		}

		public void OnVehicleExit()
		{
			activeWeaponEquipment = 0;
			weaponEquipments = (from we in weaponEquipments
				where !(we is TankWeaponEquipment)
				select we).ToList();
			EquipDefaultWeapon();
			health.invulnerability = false;
		}

		private void Die()
		{
			if (Manager.Contains<SurvivalRankManager>())
			{
				Manager.Get<SurvivalRankManager>().PlayerDie();
			}
			died = true;
			Manager.Get<StateMachineManager>().PushState<GameOverState>();
			for (int i = 0; i < statsToReset.Length; i++)
			{
				statsToReset[i].Reset();
			}
			health.hp = health.maxHp;
		}

		public void OnGameSavedFrequent()
		{
			PlayerPrefs.SetFloat("player.health", health.hp);
			PlayerPrefs.SetString("player.inventoryPath", inventory.inventoryPath);
			inventory.Save();
		}

		public void OnGameSavedInfrequent()
		{
		}

		public void OnGameplayStarted()
		{
			health.hp = PlayerPrefs.GetFloat("player.health", health.maxHp);
			inventory.inventoryPath = PlayerPrefs.GetString("player.inventoryPath", "inventory.player.");
			inventory.Load();
		}

		public void OnGameplayRestarted()
		{
			PlayerPrefs.DeleteKey("player.health");
			PlayerPrefs.DeleteKey("player.inventoryPath");
			inventory.Restart();
		}

		private void PlaySound(GameSound gameSound)
		{
			Sound sound = new Sound();
			sound.clip = Manager.Get<ClipsManager>().GetClipFor(gameSound);
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}

		private void UpdateWeaponCountFact()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.HAS_MORE_THAN_ONE_WEAPON, HasMoreThan1Weapons());
		}

		private void OnDestroy()
		{
			if (Manager.Get<GameCallbacksManager>() != null)
			{
				Manager.Get<GameCallbacksManager>().UnregisterObject(this);
			}
			if (MonoBehaviourSingleton<GameplayFacts>.get != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.SURVIVAL_SHOOTING_CONFIG);
			}
		}
	}
}
