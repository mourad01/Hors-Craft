// DecompilerFi decompiler from Assembly-CSharp.dll class: WeaponHandler
using com.ootii.Cameras;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay;
using Gameplay.Audio;
using States;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponHandler : Weapon, IAmmoWeapon
{
	public float shootingRateHz = 1f;

	public float distance = 10f;

	[Range(0f, 1f)]
	public float range = 0.1f;

	public float recoilStrength = 2f;

	public bool areaDamage;

	public bool sniping;

	public CameraFovAnimation shotAnimation;

	public GameObject particlePrefab;

	public string shootStateName = "GunShot";

	public GameObject ProjectilePrefab;

	public GameObject prefabToSpawnOnEnemyKill;

	public AudioClip shootClip;

	public AudioClip reloadClip;

	public int magazineBulletsSize = 30;

	public float reloadTime = 2f;

	public GameObject muzzle;

	public Transform projectilePos;

	[HideInInspector]
	public AmmoType ammoType;

	protected bool shooting;

	private float shootingTime;

	private List<GameObject> targets = new List<GameObject>();

	private List<GameObject> particles = new List<GameObject>();

	private float animationLength = 1f;

	private Animator animator;

	private int muzzleCounter;

	protected int magazineBulletsLeft;

	public int magazineMaxBulletsSize;

	private float reloadTimer;

	private bool isReloading;

	public GameObject shootTrailPrefab;

	public PlayerStats weaponStats;

	private SurvivalAmmoCountContext ammoCountContext;

	private AmmoContextGenerator ammoContextGenerator;

	private AmmoContext ammoContext;

	private NewWeaponEquipedContext newWeaponEquip;

	protected LineRenderer _shootTrails;

	private Camera _cam;

	private GameObject _crosshair;

	private SurvivalGameplayState _gameplay;

	protected LineRenderer shootTrails
	{
		get
		{
			if (_shootTrails != null)
			{
				return _shootTrails;
			}
			GameObject gameObject = Object.Instantiate(shootTrailPrefab);
			_shootTrails = gameObject.GetComponent<LineRenderer>();
			return _shootTrails;
		}
	}

	protected Camera cam => _cam ?? (_cam = CameraController.instance.MainCamera);

	public GameObject crosshairToMove
	{
		get
		{
			if (_crosshair != null || !(Manager.Get<StateMachineManager>().currentState is SurvivalGameplayState))
			{
				return _crosshair;
			}
			CombatCrosshairModule combatCrosshairModule = gameplay.GetModules().FirstOrDefault((GameplayModule m) => m is CombatCrosshairModule) as CombatCrosshairModule;
			if (combatCrosshairModule != null)
			{
				_crosshair = combatCrosshairModule.crosshair.gameObject;
			}
			return _crosshair;
		}
	}

	private SurvivalGameplayState gameplay => _gameplay ?? (_gameplay = (Manager.Get<StateMachineManager>().currentState as SurvivalGameplayState));

	protected virtual void Start()
	{
		animator = GetComponentInChildren<Animator>();
		if (animator != null)
		{
			AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
			foreach (AnimationClip animationClip in animationClips)
			{
				if (animationClip.name == shootStateName)
				{
					animationLength = animationClip.length;
				}
			}
			animator.SetFloat("VariableAnimSpeed", shootingRateHz * animationLength);
		}
		if (muzzle == null)
		{
			Transform transform = base.transform.FindChildRecursively("Muzzle");
			if (transform != null)
			{
				muzzle = transform.gameObject;
			}
		}
		if (projectilePos == null)
		{
			projectilePos = base.transform.FindChildRecursively("ProjectileSpawn");
		}
		if (Manager.Contains<SurvivalManager>())
		{
			ammoContext = SurvivalContextsBroadcaster.instance.GetContext<AmmoContext>();
			ammoContextGenerator = Manager.Get<SurvivalManager>().gameObject.GetComponent<AmmoContextGenerator>();
		}
		int num;
		if (ammoContextGenerator != null)
		{
			int a = magazineBulletsSize;
			AmmoContext.AmmoPair ammoForType = ammoContext.GetAmmoForType(ammoType);
			num = Mathf.Min(a, ammoForType.currentAmmo);
		}
		else
		{
			num = magazineBulletsSize;
		}
		magazineBulletsLeft = num;
		if (magazineBulletsLeft == -1)
		{
			magazineBulletsLeft = magazineBulletsSize;
		}
		ammoCountContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalAmmoCountContext>(Fact.SURVIVAL_AMMO_COUNT);
		ammoCountContext = (ammoCountContext ?? new SurvivalAmmoCountContext());
		UpdateAmmoCountContext();
		PlayerStats.Modifier modifier = new PlayerStats.Modifier();
		modifier.value = damage;
		modifier.priority = 0;
		modifier.Action = ((float toAction, float value) => value + toAction);
		PlayerStats.Modifier modifier2 = modifier;
		if (weaponStats != null)
		{
			weaponStats.Add(modifier2);
			weaponStats.Register(OnStatsChanged);
			OnStatsChanged();
		}
		newWeaponEquip = (SurvivalContextsBroadcaster.instance.GetContext<NewWeaponEquipedContext>() ?? new NewWeaponEquipedContext());
		newWeaponEquip.weaponGameObject = base.gameObject;
		SurvivalContextsBroadcaster.instance.UpdateContext(newWeaponEquip);
	}

	public void Reset()
	{
		shooting = false;
		shootingTime = 0f;
	}

	protected virtual void Update()
	{
		if (!(Manager.Get<StateMachineManager>().currentState as GameplayState == null))
		{
			UpdateMuzzle();
			UpdateShooting();
			UpdateReloading();
			UpdateCrosshair();
		}
	}

	private void UpdateMuzzle()
	{
		if (muzzleCounter <= 0 && muzzle.activeSelf)
		{
			muzzle.SetActive(value: false);
		}
		else if (muzzleCounter > 0)
		{
			muzzleCounter--;
		}
	}

	protected void UpdateShooting()
	{
		if (!shooting)
		{
			return;
		}
		shootingTime += Time.deltaTime;
		if (shootingTime >= 1f / shootingRateHz)
		{
			shooting = false;
			shootingTime = 0f;
			Animator componentInChildren = GetComponentInChildren<Animator>();
			if (componentInChildren != null)
			{
				componentInChildren.speed = 1f;
			}
		}
	}

	protected void UpdateReloading()
	{
		if (isReloading)
		{
			if (reloadTimer <= 0f)
			{
				magazineBulletsLeft = magazineBulletsSize;
				isReloading = false;
			}
			if (ammoContext != null)
			{
				AmmoContext.AmmoPair ammoForType = ammoContext.GetAmmoForType(ammoContext.currentAmmoType);
				if (ammoForType.currentAmmo == 0)
				{
					goto IL_0090;
				}
			}
			reloadTimer -= Time.deltaTime;
		}
		else if (!shooting && magazineBulletsLeft <= 0)
		{
			Reload();
		}
		goto IL_0090;
		IL_0090:
		ShowReloadCooldown();
	}

	protected virtual void ShowReloadCooldown()
	{
		if (!(gameplay == null))
		{
			UpdateAmmoCountContext();
		}
	}

	public override void OnPress()
	{
		if (!shooting && !isReloading && magazineBulletsLeft > 0)
		{
			Shoot();
		}
	}

	protected virtual void Shoot()
	{
		Manager.Get<SurvivalManager>().ShootFired();
		if (ammoContextGenerator != null)
		{
			ammoContextGenerator.TakeAmmo(ammoType, 1);
		}
		Crosshair.instance.FindTargets();
		if (Crosshair.instance.IsOutOfRange())
		{
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_OUT_OF_RANGE);
		}
		magazineBulletsLeft--;
		if (areaDamage)
		{
			DoAreaDamage();
		}
		else
		{
			DoSingleTargetDamage();
		}
		ShootEffects();
		Vector3 normalized = (muzzle.transform.position + base.transform.forward * distance - muzzle.transform.position).normalized;
		if ((bool)shootTrailPrefab)
		{
			StartCoroutine(ShootTrail(muzzle.transform.position, normalized, muzzle.transform.position + base.transform.forward * distance));
		}
	}

	protected virtual IEnumerator ShootTrail(Vector3 fr, Vector3 forward, Vector3 to)
	{
		shootTrails.enabled = true;
		int count = Mathf.Max(2, Mathf.CeilToInt((to - fr).magnitude) / 10);
		shootTrails.positionCount = count;
		shootTrails.SetPosition(0, muzzle.transform.position);
		for (int j = 1; j < count; j++)
		{
			shootTrails.SetPosition(j, Vector3.Lerp(fr, to, (float)(j + 1) / (float)count));
		}
		shootTrails.SetPosition(count - 1, to - forward * 0.25f);
		for (int i = 0; i < 15; i++)
		{
			yield return new WaitForFixedUpdate();
		}
		shootTrails.enabled = false;
	}

	private void DoAreaDamage()
	{
		Crosshair.instance.GetAllTargets(targets);
		foreach (GameObject target in targets)
		{
			Vector3 vector = cam.WorldToScreenPoint(target.transform.position);
			float num = 1f - vector.z / distance;
			Health componentInChildren = target.GetComponentInChildren<Health>();
			bool flag = componentInChildren.hp > 0f;
			componentInChildren.OnHit(Mathf.Ceil(damage * num), base.transform.forward);
			if (flag && componentInChildren.hp <= 0f)
			{
				TargetDiedEffect(componentInChildren);
			}
		}
		if (particlePrefab != null)
		{
			SpawnParticles();
		}
	}

	private void DoSingleTargetDamage()
	{
		GameObject closestTarget = Crosshair.instance.GetClosestTarget();
		if (closestTarget != null)
		{
			Health componentInChildren = closestTarget.GetComponentInChildren<Health>();
			bool flag = componentInChildren.hp > 0f;
			componentInChildren.OnHit(damage, base.transform.forward);
			if (flag && componentInChildren.hp <= 0f)
			{
				TargetDiedEffect(componentInChildren);
			}
		}
		if (particlePrefab != null)
		{
			SpawnParticles();
		}
	}

	protected void ShootEffects()
	{
		if (animator != null)
		{
			animator.SetTrigger("shot");
		}
		shooting = true;
		if (shotAnimation != null)
		{
			shotAnimation.StartAnimation(recoilStrength, 1f / shootingRateHz);
		}
		FireProjectile();
		if (muzzle != null)
		{
			PlaySound(shootClip);
			muzzle.SetActive(value: true);
			muzzleCounter = 2;
		}
	}

	protected virtual void FireProjectile()
	{
		if (ProjectilePrefab != null)
		{
			GameObject gameObject = Object.Instantiate(ProjectilePrefab, projectilePos.transform.position, projectilePos.transform.rotation);
			gameObject.GetComponent<Projectilemove>().stopEmittingAfterMeters = distance;
		}
	}

	private void Reload()
	{
		isReloading = true;
		reloadTimer = reloadTime;
		if (animator != null)
		{
			animator.SetTrigger("reload");
		}
		if (reloadClip != null)
		{
			PlaySound(reloadClip);
		}
	}

	private void SpawnParticles()
	{
		for (int i = 0; i < 5; i++)
		{
			Vector2 vector = Random.insideUnitCircle * range * cam.pixelHeight / 3f;
			Ray ray = cam.ScreenPointToRay(new Vector3((float)cam.pixelWidth / 2f + vector.x, (float)cam.pixelHeight / 2f + vector.y, 0f));
			SpawnParticle(ray);
		}
	}

	private void SpawnParticle(Ray ray)
	{
		if (Physics.Raycast(ray, out RaycastHit hitInfo, distance))
		{
			GameObject gameObject = GetParticles(particlePrefab);
			gameObject.transform.position = hitInfo.point - 0.1f * ray.direction;
			gameObject.GetComponent<ParticleSystem>().Stop();
			gameObject.GetComponent<ParticleSystem>().Emit(1);
		}
	}

	protected void TargetDiedEffect(Health health)
	{
		Manager.Get<QuestManager>().OnEnemyKilled();
		if (prefabToSpawnOnEnemyKill != null)
		{
			Object.Instantiate(prefabToSpawnOnEnemyKill, health.transform.position, health.transform.rotation);
		}
	}

	private GameObject GetParticles(GameObject prefab)
	{
		for (int i = 0; i < particles.Count; i++)
		{
			if (!particles[i].GetComponent<ParticleSystem>().IsAlive(withChildren: true))
			{
				return particles[i];
			}
		}
		GameObject gameObject = Object.Instantiate(prefab);
		particles.Add(gameObject);
		return gameObject;
	}

	protected void PlaySound(AudioClip clip)
	{
		if (!(clip == null))
		{
			Sound sound = new Sound();
			sound.clip = clip;
			sound.mixerGroup = Manager.Get<MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}
	}

	public override void OnRelease()
	{
	}

	protected virtual void UpdateCrosshair()
	{
		if (crosshairToMove != null)
		{
			crosshairToMove.transform.position = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		}
	}

	protected void UpdateAmmoCountContext()
	{
		ammoCountContext.isReloading = isReloading;
		ammoCountContext.maxAmmo = magazineBulletsSize;
		ammoCountContext.currentAmmo = ((!isReloading) ? magazineBulletsLeft : ((int)((float)magazineBulletsSize / reloadTime * (reloadTime - reloadTimer))));
		ammoCountContext.customSprite = sprite;
		if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.SURVIVAL_AMMO_COUNT, ammoCountContext))
		{
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_AMMO_COUNT);
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		if (_shootTrails != null)
		{
			_shootTrails.enabled = false;
			UnityEngine.Object.Destroy(_shootTrails.gameObject);
		}
		ammoCountContext = null;
		if (weaponStats != null)
		{
			weaponStats.Unregister(OnStatsChanged);
		}
		SurvivalContextsBroadcaster.instance.UpdateContext(newWeaponEquip);
	}

	public AmmoType GetAmmoType()
	{
		return ammoType;
	}

	public void SetAmmoType(AmmoType ammoType)
	{
		this.ammoType = ammoType;
	}

	public void OnStatsChanged()
	{
		damage = weaponStats.GetStats();
	}
}
