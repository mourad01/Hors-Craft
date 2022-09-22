// DecompilerFi decompiler from Assembly-CSharp.dll class: GrenadeController
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using Gameplay.Audio;
using UnityEngine;

public class GrenadeController : Weapon, IAmmoWeapon
{
	public GameObject grenadePrefab;

	public Transform grenadePivot;

	public Animator grenadeAnimator;

	public float nextThrowCooldown = 1f;

	public float throwPower = 10f;

	public float throwTime = 2f;

	public AudioClip throwSound;

	public AudioClip unlockSound;

	public AmmoType ammoType;

	private SurvivalAmmoCountContext ammoCountContext;

	private AmmoContextGenerator ammoContextGenerator;

	private GameObject grenade;

	private float throwCooldown;

	private float throwTimer;

	private bool isThrowing;

	private void Start()
	{
		throwTimer = 0f;
		throwCooldown = 0f;
		isThrowing = false;
		ammoContextGenerator = Manager.Get<SurvivalManager>().gameObject.GetComponent<AmmoContextGenerator>();
		ammoCountContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalAmmoCountContext>(Fact.SURVIVAL_AMMO_COUNT);
		ammoCountContext = (ammoCountContext ?? new SurvivalAmmoCountContext());
		UpdateAmmoCountContext();
		SpawnGrenade();
	}

	private void SpawnGrenade()
	{
		grenade = Object.Instantiate(grenadePrefab);
		grenade.transform.SetParent(grenadePivot, worldPositionStays: false);
	}

	private void Update()
	{
		if (throwCooldown > 0f)
		{
			throwCooldown -= Time.deltaTime;
			if (throwCooldown <= 0f)
			{
				SpawnGrenade();
			}
		}
		if (isThrowing)
		{
			throwTimer -= Time.deltaTime;
			if (throwTimer <= 0f)
			{
				OnThrow();
			}
		}
	}

	public override void OnPress()
	{
		Craftable weaponById = Manager.Get<CraftingManager>().GetCraftableListInstance().GetWeaponById(id);
		int num;
		if (weaponById != null)
		{
			num = Singleton<PlayerData>.get.playerItems.GetCraftableCount(weaponById.id);
		}
		else
		{
			AmmoContext.AmmoPair ammoForType = SurvivalContextsBroadcaster.instance.GetContext<AmmoContext>().GetAmmoForType(ammoType);
			num = ammoForType.currentAmmo;
		}
		if (!isThrowing && throwCooldown <= 0f && num > 0)
		{
			throwTimer = throwTime;
			isThrowing = true;
			grenadeAnimator.SetTrigger("shot");
			MixersManager.Play(unlockSound);
		}
	}

	private void OnThrow()
	{
		Craftable weaponById = Manager.Get<CraftingManager>().GetCraftableListInstance().GetWeaponById(id);
		if (weaponById != null)
		{
			Singleton<PlayerData>.get.playerItems.AddCraftable(weaponById.id, -1);
		}
		else
		{
			if (ammoContextGenerator != null)
			{
				ammoContextGenerator.TakeAmmo(ammoType, 1);
			}
			Manager.Get<SurvivalManager>().ShootFired();
		}
		Grenade component = grenade.GetComponent<Grenade>();
		component.isFriendly = true;
		component.translation = CameraController.instance.MainCamera.transform.position - grenade.transform.position;
		Vector3 forward = CameraController.instance.MainCamera.transform.forward;
		component.Throw(forward * throwPower);
		throwCooldown = nextThrowCooldown;
		isThrowing = false;
		grenadeAnimator.SetTrigger("reload");
		MixersManager.Play(throwSound);
		SwitchWeaponIfNoGrenades();
	}

	private void SwitchWeaponIfNoGrenades()
	{
		Craftable weaponById = Manager.Get<CraftingManager>().GetCraftableListInstance().GetWeaponById(id);
		int num;
		if (weaponById != null)
		{
			num = Singleton<PlayerData>.get.playerItems.GetCraftableCount(weaponById.id);
		}
		else
		{
			AmmoContext.AmmoPair ammoForType = SurvivalContextsBroadcaster.instance.GetContext<AmmoContext>().GetAmmoForType(ammoType);
			num = ammoForType.currentAmmo;
		}
		if (num <= 0)
		{
			ArmedPlayer component = PlayerGraphic.GetControlledPlayerInstance().GetComponent<ArmedPlayer>();
			component.SwitchToNextWeapon();
		}
	}

	public override void OnRelease()
	{
	}

	public AmmoType GetAmmoType()
	{
		return ammoType;
	}

	public void SetAmmoType(AmmoType ammoType)
	{
		this.ammoType = ammoType;
	}

	protected void UpdateAmmoCountContext()
	{
		ammoCountContext.isReloading = false;
		ammoCountContext.maxAmmo = 1;
		ammoCountContext.currentAmmo = 1;
		ammoCountContext.customSprite = sprite;
		if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.SURVIVAL_AMMO_COUNT, ammoCountContext))
		{
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_AMMO_COUNT);
		}
	}
}
