// DecompilerFi decompiler from Assembly-CSharp.dll class: ShootingEnemy
using Common.Audio;
using Common.BehaviourTrees;
using Common.Managers;
using Common.Managers.Audio;
using Common.Utils;
using Gameplay.Audio;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class ShootingEnemy : HumanMob, IFighting
{
	public GameSound hurtSound;

	public int dmgShoot;

	public float shootRange = 20f;

	public float shootMaxOffDegrees = 5f;

	public GameObject shootTrailPrefab;

	public int dmgAttack;

	public List<Transform> shootStartTransforms = new List<Transform>();

	public float afterShootCooldown = 2.5f;

	public float afterGrenadeCooldown = 2.5f;

	public int grenadesCount = 3;

	public GameObject grenadePrefab;

	private const float AFTER_ATTACK_COOLDOWN = 2.5f;

	protected CooldownNode shootCooldownNode;

	protected CooldownNode grenadeCooldownNode;

	private TriggerTargetAcquisition targetAcquisition;

	public bool allowDamagingVehicles;

	public bool setRandomSuit = true;

	[Header("Clips override enums")]
	public AudioClip hurtSoundClip;

	public AudioClip shootSoundClip;

	private LineRenderer _shootTrails;

	public Health health
	{
		get;
		private set;
	}

	private LineRenderer shootTrails
	{
		get
		{
			if (_shootTrails == null)
			{
				GameObject gameObject = Object.Instantiate(shootTrailPrefab);
				_shootTrails = gameObject.GetComponent<LineRenderer>();
			}
			return _shootTrails;
		}
	}

	public virtual bool IsEnemy()
	{
		return true;
	}

	public Health GetHealth()
	{
		return health;
	}

	public Mob GetMob()
	{
		return this;
	}

	protected virtual bool DestroyBlocks()
	{
		return false;
	}

	protected virtual void Awake()
	{
		health = GetComponent<Health>();
		if (setRandomSuit)
		{
			SetRandomSuit();
		}
	}

	private void SetRandomSuit()
	{
		SoldiersRecruiterContext context = SurvivalContextsBroadcaster.instance.GetContext<SoldiersRecruiterContext>();
		Material material = (!IsEnemy()) ? context.friendlySoldierClothes.Random() : context.enemySoldierClothes.Random();
		Renderer[] renderers = health.renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.material = material;
		}
	}

	protected override void Start()
	{
		base.Start();
		health.onHitAction += OnHitEffect;
		targetAcquisition = GetComponent<TriggerTargetAcquisition>();
		if (targetAcquisition == null)
		{
			targetAcquisition = base.gameObject.AddComponent<TriggerTargetAcquisition>();
			targetAcquisition.setRange = shootRange;
		}
	}

	private void OnHitEffect(Vector3 dir)
	{
		Sound sound = new Sound();
		sound.clip = ((!(hurtSoundClip == null)) ? hurtSoundClip : Manager.Get<ClipsManager>().GetClipFor(hurtSound));
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		sound.volumeFrom = Sound.EVolumeFrom.POSITION;
		Sound sound2 = sound;
		sound2.Play(base.transform);
	}

	public void Attack(Transform target)
	{
		Shoot(target);
	}

	protected virtual void Shoot(Transform target)
	{
		Transform transform = base.transform;
		Vector3 up = Vector3.up;
		Vector3 eulerAngles = Quaternion.LookRotation(target.transform.position - base.transform.position).eulerAngles;
		transform.eulerAngles = up * eulerAngles.y;
		base.animator.SetTrigger("shoot");
		Transform transform2 = shootStartTransforms.Random();
		Vector3 normalized = (target.position - transform2.position).normalized;
		normalized = Vector3.Lerp(normalized, Random.insideUnitSphere, shootMaxOffDegrees / 360f);
		int mask = LayerMask.GetMask("Mobs", "Ignore Raycast", "NoClipMode", "Vehicle");
		Vector3 to;
		if (Physics.Raycast(transform2.position, normalized, out RaycastHit hitInfo, shootRange, mask, QueryTriggerInteraction.Ignore) && EnemySpottedCondition(hitInfo.collider.gameObject))
		{
			to = hitInfo.point;
			Health componentInParent = hitInfo.collider.GetComponentInParent<Health>();
			if (componentInParent != null)
			{
				componentInParent.OnHit(dmgShoot, normalized, 0.1f);
			}
			else if (DestroyBlocks())
			{
				Engine.VoxelRaycast(transform2.position, normalized, shootRange, ignoreTransparent: true)?.SetVoxel(0, updateMesh: true, 0);
			}
		}
		else
		{
			to = transform2.position + normalized * shootRange;
		}
		shootCooldownNode.SetCooldown(afterShootCooldown);
		StartCoroutine(ShootTrail(transform2.position, normalized, to, transform2));
		Sound sound = new Sound();
		sound.clip = ((!(hurtSoundClip == null)) ? shootSoundClip : Manager.Get<ClipsManager>().GetClipFor(GameSound.ENEMY_SHOOT));
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		sound.volumeFrom = Sound.EVolumeFrom.POSITION;
		Sound sound2 = sound;
		sound2.Play(base.transform);
	}

	public void ThrowGrenade()
	{
		StartCoroutine(ThrowGrenadeCoroutine());
		grenadesCount--;
		if (grenadesCount > 0)
		{
			grenadeCooldownNode.SetCooldown(afterGrenadeCooldown);
		}
		else
		{
			grenadeCooldownNode.SetCooldown(9999999f);
		}
	}

	protected virtual IEnumerator ShootTrail(Vector3 fr, Vector3 forward, Vector3 to, Transform shootStartTransform)
	{
		shootTrails.enabled = true;
		int count = Mathf.Max(2, Mathf.CeilToInt((to - fr).magnitude) / 10);
		shootTrails.positionCount = count;
		shootTrails.SetPosition(0, shootStartTransform.position);
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

	public override void Die()
	{
		StopAllCoroutines();
		if (_shootTrails != null && _shootTrails.gameObject != null)
		{
			_shootTrails.enabled = false;
			UnityEngine.Object.Destroy(_shootTrails.gameObject);
		}
		shootCooldownNode.SetCooldown(10f);
		if (grenadeCooldownNode != null)
		{
			grenadeCooldownNode.SetCooldown(10f);
		}
		DropAmmo();
		MonoBehaviourSingleton<ProgressCounter>.get.Increment("EnemyKill");
		if (Manager.Contains<AbstractAchievementManager>())
		{
			Manager.Get<AbstractAchievementManager>().RegisterEvent("enemies.killed");
		}
		base.Die();
	}

	private void DropAmmo()
	{
		DropAmmo component = GetComponent<DropAmmo>();
		if (component != null)
		{
			component.Drop(base.transform.position);
		}
	}

	protected override ParallelNode ConstructTopParallelNode()
	{
		ParallelNode parallelNode = new ParallelNode();
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(ConstructDieBehaviour());
		selectorNode.Add(ConstructAttackEnemies());
		parallelNode.Add(selectorNode);
		parallelNode.Add(ConstructRunToPlayer());
		return parallelNode;
	}

	protected Node ConstructAttackEnemies()
	{
		SelectorNode selectorNode = new SelectorNode();
		if (grenadePrefab != null)
		{
			selectorNode.Add(ConstructThrowGrenade());
		}
		selectorNode.Add(ConstructShootEnemies());
		return selectorNode;
	}

	protected Node ConstructThrowGrenade()
	{
		SequenceNode sequenceNode = new SequenceNode();
		grenadeCooldownNode = new CooldownNode();
		grenadeCooldownNode.SetCooldown(afterGrenadeCooldown);
		sequenceNode.Add(grenadeCooldownNode);
		sequenceNode.Add(new IsPlayerNotInLineOfSight(this));
		sequenceNode.Add(new ThrowGrenade(this));
		return sequenceNode;
	}

	protected Node ConstructShootEnemies()
	{
		SequenceNode sequenceNode = new SequenceNode();
		shootCooldownNode = new CooldownNode();
		sequenceNode.Add(shootCooldownNode);
		sequenceNode.Add(new AttackEnemiesInAttackRangeNode(this, GetTargets));
		return sequenceNode;
	}

	protected virtual bool EnemySpottedCondition(GameObject c)
	{
		return c != null && (IsPlayer(c) || IsCivilian(c) || IsEnemySoldier(c) || IsVehicle(c));
	}

	private bool IsPlayer(GameObject c)
	{
		return c.GetComponentInParent<PlayerMovement>() != null;
	}

	private bool IsCivilian(GameObject c)
	{
		return c.GetComponentInParent<HumanMob>() != null && c.GetComponentInParent<Health>() != null && c.GetComponentInParent<IFighting>() == null;
	}

	private bool IsEnemySoldier(GameObject c)
	{
		return c.GetComponentInParent<Soldier>() != null;
	}

	private bool IsVehicle(GameObject c)
	{
		if (allowDamagingVehicles)
		{
			return c.GetComponentInParent<VehicleController>() != null;
		}
		return false;
	}

	protected List<GameObject> GetTargets()
	{
		return targetAcquisition.GetTargets(EnemySpottedCondition);
	}

	protected virtual Node ConstructRunToPlayer()
	{
		LoopNode loopNode = new LoopNode();
		loopNode.Add(new SetDestinationNearPlayer(this, 6f));
		loopNode.Add(new GoToDestinationNode(this, runSpeed));
		return loopNode;
	}

	private IEnumerator ThrowGrenadeCoroutine()
	{
		base.animator.SetTrigger("grenade");
		yield return new WaitForSeconds(0.7f);
		GameObject grenade = Object.Instantiate(grenadePrefab);
		grenade.transform.position = GetMob().transform.position + GetMob().transform.forward * 0.2f + GetMob().transform.up * 0.2f;
		grenade.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		Grenade grenadeScript = grenade.GetComponent<Grenade>();
		Vector3 direction = Quaternion.Euler(-30f, 0f, 0f) * GetMob().transform.forward;
		grenadeScript.isFriendly = false;
		grenadeScript.Throw(direction.normalized * 10f);
	}

	public void HealtMultiplier(float multiplier, float baseValue = -1f)
	{
		if (baseValue != -1f)
		{
			health.maxHp = baseValue;
		}
		health.maxHp *= multiplier;
		health.hp = health.maxHp;
	}

	public void DamageMultiplier(float multiplier, float baseValue = -1f)
	{
		if (baseValue != -1f)
		{
			dmgShoot = (int)baseValue;
			dmgAttack = (int)baseValue;
		}
		dmgShoot = (int)((float)dmgShoot * multiplier);
		dmgAttack = (int)((float)dmgAttack * multiplier);
	}

	private void OnDestroy()
	{
		if (_shootTrails != null && _shootTrails.gameObject != null)
		{
			_shootTrails.enabled = false;
			UnityEngine.Object.Destroy(_shootTrails.gameObject);
		}
	}
}
