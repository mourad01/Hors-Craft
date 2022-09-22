// DecompilerFi decompiler from Assembly-CSharp.dll class: EnemyMob
using Common.Audio;
using Common.BehaviourTrees;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class EnemyMob : Mob, IFighting
{
	public GameSound walkSound;

	public GameSound hurtSound;

	[Header("Clips override enums")]
	public AudioClip walkSoundClip;

	public AudioClip hurtSoundClip;

	public bool disableWalkSound;

	public int dmg;

	public float runSpeed = 7f;

	protected TriggerTargetAcquisition targetAcquisition;

	protected CooldownNode cooldownNode;

	private float nextSoundTime;

	private const float SOUND_INTERVAL_MIN = 5f;

	private const float SOUND_INTERVAL_MAX = 10f;

	public bool allowDamagingVehicles;

	private Health _healt;

	protected virtual float afterAttackCooldown => 2.5f;

	protected virtual float attackRange => 2.5f;

	public Health health => _healt ?? (_healt = GetComponentInChildren<Health>());

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

	protected override void Start()
	{
		health.onHitAction += OnHitEffect;
		nextSoundTime = Time.time + 2f;
		targetAcquisition = base.gameObject.GetComponent<TriggerTargetAcquisition>();
		if (targetAcquisition == null)
		{
			targetAcquisition = base.gameObject.AddComponent<TriggerTargetAcquisition>();
			targetAcquisition.setRange = attackRange;
		}
		base.Start();
	}

	public virtual void Attack(Transform target)
	{
		Health componentInParent = target.GetComponentInParent<Health>();
		componentInParent.OnHit(dmg, target.position - base.transform.position);
		base.animator.SetTrigger("attack");
		cooldownNode.SetCooldown(afterAttackCooldown);
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
			dmg = (int)baseValue;
		}
		dmg = (int)((float)dmg * multiplier);
	}

	protected virtual Node ConstructAttackPlayer()
	{
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(cooldownNode);
		sequenceNode.Add(new AttackEnemiesInAttackRangeNode(this, GetTargets));
		return sequenceNode;
	}

	protected virtual Node ConstructRunToEnemy()
	{
		LoopNode loopNode = new LoopNode();
		loopNode.Add(new SetDestinationNearTargetEnemy(this, GetTargets));
		loopNode.Add(new GoToDestinationNode(this, runSpeed));
		return loopNode;
	}

	public virtual bool EnemySpottedCondition(GameObject c)
	{
		return c != null && (IsPlayer(c) || IsCivilian(c) || IsEnemySoldier(c) || IsVehicle(c));
	}

	protected List<GameObject> GetTargets()
	{
		return targetAcquisition.GetTargets(EnemySpottedCondition);
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

	protected virtual void UpdateWalkingEffect()
	{
		if (!disableWalkSound && Time.time > nextSoundTime)
		{
			Sound sound = new Sound();
			sound.clip = ((!(walkSoundClip == null)) ? walkSoundClip : Manager.Get<ClipsManager>().GetClipFor(walkSound));
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			sound.volume = 0.7f;
			sound.volumeFrom = Sound.EVolumeFrom.POSITION;
			Sound sound2 = sound;
			sound2.Play(base.transform);
			nextSoundTime = Time.time + Random.Range(5f, 10f);
		}
	}

	protected virtual void OnHitEffect(Vector3 dir)
	{
		Sound sound = new Sound();
		sound.clip = ((!(hurtSoundClip == null)) ? hurtSoundClip : Manager.Get<ClipsManager>().GetClipFor(hurtSound));
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		sound.volumeFrom = Sound.EVolumeFrom.POSITION;
		Sound sound2 = sound;
		sound2.Play(base.transform);
	}

	public override void Die()
	{
		base.Die();
		MonoBehaviourSingleton<ProgressCounter>.get.Increment("EnemyKill");
		if (Manager.Contains<AbstractAchievementManager>())
		{
			Manager.Get<AbstractAchievementManager>().RegisterEvent("enemies.killed");
		}
	}
}
