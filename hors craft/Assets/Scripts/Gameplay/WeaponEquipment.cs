// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.WeaponEquipment
using com.ootii.Cameras;
using Common.Audio;
using Common.Managers;
using Gameplay.Audio;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class WeaponEquipment
	{
		protected int currentWeapon;

		protected ArmedPlayer armedPlayer;

		protected RenderTexture renderTexture;

		protected WeaponEquippedContext _combatContext;

		private AmmoContextGenerator _ammoContextGenerator;

		public GameObject[] weapons
		{
			get;
			private set;
		}

		protected WeaponEquippedContext combatContext => _combatContext ?? (_combatContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<WeaponEquippedContext>(Fact.WEAPON_EQUIPPED)) ?? (_combatContext = new WeaponEquippedContext
		{
			crosshairSprite = null,
			shootSprite = null
		});

		private AmmoContextGenerator ammoContextGenerator
		{
			get
			{
				if (_ammoContextGenerator == null)
				{
					_ammoContextGenerator = Manager.Get<SurvivalManager>().gameObject.GetComponent<AmmoContextGenerator>();
				}
				return _ammoContextGenerator;
			}
		}

		public WeaponEquipment(GameObject[] weapons, ArmedPlayer player)
		{
			currentWeapon = 0;
			this.weapons = weapons;
			armedPlayer = player;
		}

		public void EquipDefaultWeapon()
		{
			Equip(weapons[0]);
		}

		public virtual void SwitchToPreviousWeapon()
		{
			int num = 0;
			int num2;
			while (true)
			{
				if (num < weapons.Length)
				{
					num2 = (currentWeapon - num - 1) % weapons.Length;
					if (num2 < 0)
					{
						num2 += weapons.Length;
					}
					if (CanEquipWeapon(num2))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (currentWeapon != num2)
			{
				currentWeapon = num2;
				Equip(weapons[currentWeapon]);
			}
		}

		public virtual void SwitchToNextWeapon()
		{
			int num = 0;
			int num2;
			while (true)
			{
				if (num < weapons.Length)
				{
					num2 = (currentWeapon + num + 1) % weapons.Length;
					if (CanEquipWeapon(num2))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (currentWeapon != num2)
			{
				currentWeapon = num2;
				Equip(weapons[currentWeapon]);
			}
		}

		public virtual AmmoType GetCurrentAmmoType()
		{
			WeaponHandler component = weapons[currentWeapon].GetComponent<WeaponHandler>();
			if (component == null)
			{
				return null;
			}
			return component.ammoType;
		}

		protected virtual bool CanEquipWeapon(int index)
		{
			return IsWeaponActive(weapons[index]);
		}

		private bool IsWeaponActive(GameObject weapon)
		{
			Weapon component = weapon.GetComponent<Weapon>();
			if (component.isUnlockedAtStart)
			{
				return true;
			}
			CraftableList craftableListInstance = Manager.Get<CraftingManager>().GetCraftableListInstance();
			Craftable weaponById = craftableListInstance.GetWeaponById(component.id);
			if (weaponById != null)
			{
				int craftableCount = Singleton<PlayerData>.get.playerItems.GetCraftableCount(weaponById.id);
				if (craftableCount > 0)
				{
					return true;
				}
			}
			else
			{
				WeaponConfig weaponConfig = (from weapon2 in SurvivalContextsBroadcaster.instance.GetContext<WeaponsContext>().weaponsConfigs
					where object.ReferenceEquals(weapon2.prefab, weapon)
					select weapon2).FirstOrDefault();
				if (weaponConfig.prefab != null)
				{
					return weaponConfig.claimed;
				}
			}
			return false;
		}

		public virtual void EquipWeaponWithId(int id)
		{
			for (int i = 0; i < weapons.Length; i++)
			{
				if (weapons[i].GetComponent<Weapon>().id == id)
				{
					currentWeapon = i;
					Equip(weapons[i]);
				}
			}
		}

		public virtual void Equip(GameObject weapon)
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
			GameObject gameObject = UnityEngine.Object.Instantiate(weapon, handle.position, handle.rotation);
			gameObject.transform.SetParent(handle);
			ConfigOnEquip(gameObject);
		}

		public virtual Weapon GetWeapon(int id)
		{
			GameObject gameObject = weapons.FirstOrDefault((GameObject w) => w.GetComponent<Weapon>().id == id);
			return (!(gameObject == null)) ? gameObject.GetComponent<Weapon>() : null;
		}

		public bool HasMultipleActiveWeapons()
		{
			return weapons.Count(IsWeaponActive) > 1;
		}

		protected virtual void ConfigOnEquip(GameObject instance)
		{
			Weapon component = instance.GetComponent<Weapon>();
			component.owner = armedPlayer.gameObject;
			if (component.setSpriteOnAttackButton)
			{
				combatContext.shootSprite = component.sprite;
			}
			else
			{
				combatContext.shootSprite = null;
			}
			combatContext.crosshairSprite = component.crosshair;
			if (component is WeaponHandler)
			{
				EquipWeaponHandler(instance, (WeaponHandler)component);
			}
			if (component is IAmmoWeapon && ammoContextGenerator != null)
			{
				ammoContextGenerator.SetAmmoType(((IAmmoWeapon)component).GetAmmoType());
			}
			if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.WEAPON_EQUIPPED, combatContext))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.WEAPON_EQUIPPED);
			}
			armedPlayer.weapon = component;
			ChangeHandsOnEquip(instance, component);
		}

		protected virtual void EquipWeaponHandler(GameObject instance, WeaponHandler weaponHandler)
		{
			weaponHandler.shotAnimation = armedPlayer.shotAnimation;
			if (weaponHandler.sniping)
			{
				SniperRifle component = instance.GetComponent<SniperRifle>();
				if ((bool)component)
				{
					component.scopeFrame.transform.localScale = Vector3.one * 2.55f;
				}
				Bounds bounds = component.scope.GetComponent<MeshRenderer>().bounds;
				Vector3 vector = CameraController.instance.MainCamera.WorldToScreenPoint(bounds.min);
				Vector3 vector2 = CameraController.instance.MainCamera.WorldToScreenPoint(bounds.max);
				int num = (int)Mathf.Abs(vector2.x - vector.x);
				int num2 = (int)Mathf.Abs(vector2.y - vector.y);
				if (renderTexture != null && (renderTexture.width != num || renderTexture.height != num2))
				{
					renderTexture.Release();
					renderTexture = null;
				}
				if (renderTexture == null)
				{
					renderTexture = new RenderTexture(num, num2, 16, RenderTextureFormat.ARGB32);
				}
				component.sniperCamera.targetTexture = renderTexture;
				component.sniperCamera.gameObject.SetActive(value: true);
				component.scope.GetComponent<MeshRenderer>().material.mainTexture = renderTexture;
				CameraController.instance.MainCamera.GetComponent<Animator>().enabled = false;
			}
			else
			{
				CameraController.instance.MainCamera.GetComponent<Animator>().enabled = true;
			}
			Crosshair.instance.SetParameters(weaponHandler);
		}

		private void ChangeHandsOnEquip(GameObject instance, Weapon weaponScript)
		{
			PlayerGraphic componentInChildren = armedPlayer.GetComponentInChildren<PlayerGraphic>();
			if (weaponScript.hidePlayerHands)
			{
				componentInChildren.HideHands();
			}
			else
			{
				componentInChildren.ShowHands();
			}
			AssignProperArmSkin(componentInChildren, instance.transform, "LArm");
			AssignProperArmSkin(componentInChildren, instance.transform, "LArm2");
			AssignProperArmSkin(componentInChildren, instance.transform, "RArm");
			AssignProperArmSkin(componentInChildren, instance.transform, "RArm2");
		}

		private void AssignProperArmSkin(PlayerGraphic pg, Transform weapon, string name)
		{
			Transform transform = weapon.FindChildRecursively(name);
			if (!(transform == null))
			{
				Renderer component = transform.GetComponent<Renderer>();
				if (!(component == null))
				{
					component.material = ((!(SkinList.customPlayerSkinList != null)) ? MonoBehaviourSingleton<SkinList>.get.GetClothes(pg.GetCurrentCloth(BodyPart.Body)) : SkinList.customPlayerSkinList.GetClothes(pg.GetCurrentCloth(BodyPart.Body)));
				}
			}
		}
	}
}
