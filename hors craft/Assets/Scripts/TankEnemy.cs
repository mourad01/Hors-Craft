// DecompilerFi decompiler from Assembly-CSharp.dll class: TankEnemy
using Common.Audio;
using Common.Behaviours;
using Common.BehaviourTrees;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using System;
using System.Collections;
using Uniblocks;
using UnityEngine;

public class TankEnemy : EnemyMob
{
	public float shootRange = 20f;

	public float shootMaxOffDegrees = 5f;

	public GameObject shootTrailPrefab;

	public Transform shootStartTransform;

	public GameObject explosionPrefab;

	public GameObject shootParticles;

	public GameObject turret;

	public float explosionRange;

	public float afterShootCooldown = 2.5f;

	private const float AFTER_ATTACK_COOLDOWN = 2.5f;

	protected CooldownNode shootCooldownNode;

	private LineRenderer _shootTrails;

	protected override float attackRange => shootRange;

	private LineRenderer shootTrails
	{
		get
		{
			if (_shootTrails == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(shootTrailPrefab);
				_shootTrails = gameObject.GetComponent<LineRenderer>();
			}
			return _shootTrails;
		}
	}

	protected virtual bool DestroyBlocks()
	{
		return false;
	}

	protected override void Start()
	{
		base.Start();
	}

	public override void Attack(Transform target)
	{
		Shoot(target);
	}

	private void Shoot(Transform target)
	{
		Vector3 zero = Vector3.zero;
		if (target != null)
		{
			Transform transform = turret.transform;
			Vector3 up = Vector3.up;
			Vector3 eulerAngles = Quaternion.LookRotation(target.transform.position - base.transform.position).eulerAngles;
			transform.eulerAngles = up * eulerAngles.y;
			zero = (target.position - shootStartTransform.position).normalized;
			zero = Vector3.Lerp(zero, UnityEngine.Random.insideUnitSphere, shootMaxOffDegrees / 360f);
		}
		else
		{
			zero = shootStartTransform.forward;
		}
		Vector3 hitPoint = shootStartTransform.position + zero * shootRange;
		if (Physics.Raycast(shootStartTransform.position, zero, out RaycastHit hitInfo, shootRange, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			hitPoint = hitInfo.point;
		}
		CreateFakeBulletExplosion(hitPoint);
		shootCooldownNode.SetCooldown(afterShootCooldown);
		ShootEffects(hitPoint);
	}

	private void ShootEffects(Vector3 hitPoint)
	{
		Sound sound = new Sound();
		sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.ENEMY_SHOOT);
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		sound.volumeFrom = Sound.EVolumeFrom.POSITION;
		Sound sound2 = sound;
		sound2.Play(base.transform);
		if (shootParticles != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(shootParticles, shootStartTransform.position, shootStartTransform.rotation);
			gameObject.GetComponentInChildren<Projectilemove>().stopEmittingAfterMeters = Vector3.Distance(hitPoint, base.transform.position);
			gameObject.GetComponentInChildren<DestroyAfter>().delay = 5f;
		}
	}

	private void CreateFakeBulletExplosion(Vector3 hitPoint)
	{
		GameObject gameObject = new GameObject("Fake Bullet");
		TriggerTargetAcquisition triggerTargetAcquisition = gameObject.AddComponent<TriggerTargetAcquisition>();
		triggerTargetAcquisition.setRange = explosionRange;
		VoxelExplosion voxelExplosion = gameObject.AddComponent<VoxelExplosion>();
		voxelExplosion.radius = explosionRange;
		voxelExplosion.baseDamage = dmg;
		voxelExplosion.maxDamage = 0f;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(explosionPrefab);
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		gameObject2.transform.localPosition = Vector3.zero;
		voxelExplosion.explosionSystem = gameObject2.GetComponent<ParticleSystem>();
		gameObject.transform.position = hitPoint;
		float time = Vector3.Distance(base.transform.position, hitPoint) / 50f;
		StartCoroutine(ExplodeAfter(gameObject, time));
	}

	public override void Die()
	{
		StopAllCoroutines();
		if (_shootTrails != null)
		{
			_shootTrails.enabled = false;
			UnityEngine.Object.Destroy(_shootTrails.gameObject);
		}
		shootCooldownNode.SetCooldown(10f);
		base.Die();
	}

	public override void LookAt(Vector3 target)
	{
		Quaternion to = Quaternion.LookRotation(target - turret.transform.position, Vector3.up);
		Vector3 eulerAngles = Quaternion.RotateTowards(turret.transform.rotation, to, 40f * Time.deltaTime).eulerAngles;
		float y = eulerAngles.y;
		Vector3 eulerAngles2 = turret.transform.rotation.eulerAngles;
		turret.transform.rotation = Quaternion.Euler(new Vector3(eulerAngles2.x, y, eulerAngles2.z));
	}

	protected override ParallelNode ConstructTopParallelNode()
	{
		ParallelNode parallelNode = new ParallelNode();
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(ConstructDieBehaviour());
		selectorNode.Add(ConstructShootEnemies());
		parallelNode.Add(selectorNode);
		parallelNode.Add(ConstructMovementLogic());
		parallelNode.Add(new LookAtPlayerNode(this));
		return parallelNode;
	}

	protected Node ConstructShootEnemies()
	{
		SequenceNode sequenceNode = new SequenceNode();
		shootCooldownNode = new CooldownNode();
		sequenceNode.Add(shootCooldownNode);
		sequenceNode.Add(new AttackEnemiesInAttackRangeNode(this, base.GetTargets, GetDistanceMultiplier));
		return sequenceNode;
	}

	private Node ConstructMovementLogic()
	{
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(ConstructObstacleAvoidance());
		selectorNode.Add(ConstructRunToPlayer());
		return selectorNode;
	}

	private Node ConstructObstacleAvoidance()
	{
		SelectorNode selectorNode = new SelectorNode();
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(shootCooldownNode);
		sequenceNode.Add(new AttackAheadNode(this));
		SequenceNode sequenceNode2 = new SequenceNode();
		sequenceNode2.Add(new ShouldAvoidObstacle(this));
		sequenceNode2.Add(new AvoidObstacle(this));
		sequenceNode2.Add(new GoToDestinationNode(this, runSpeed));
		selectorNode.Add(sequenceNode);
		selectorNode.Add(sequenceNode2);
		return selectorNode;
	}

	private Node ConstructRunToPlayer()
	{
		SelectorNode selectorNode = new SelectorNode();
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(new SetDestinationNearPlayer(this, 15f));
		sequenceNode.Add(new GoToDestinationNode(this, runSpeed));
		selectorNode.Add(new IsPlayerCloseNode(this, shootRange));
		selectorNode.Add(sequenceNode);
		return selectorNode;
	}

	private IEnumerator ExplodeAfter(GameObject bullet, float time)
	{
		yield return new WaitForSeconds(time);
		VoxelExplosion voxelExplosion = bullet.GetComponent<VoxelExplosion>();
		voxelExplosion.ExplodeArea((Func<GameObject, bool>)((EnemyMob)this).EnemySpottedCondition, digDown: false, canDestroyAllBlocks: false);
		UnityEngine.Object.Destroy(bullet);
	}

	public override bool EnemySpottedCondition(GameObject c)
	{
		return base.EnemySpottedCondition(c) || (c.GetComponentInParent<VehicleController>() != null && c.GetComponentInParent<Health>() != null);
	}

	private float GetDistanceMultiplier(GameObject enemy)
	{
		if (enemy.GetComponentInParent<PlayerMovement>() != null)
		{
			return 3f;
		}
		if (enemy.GetComponentInParent<VehicleController>() != null && enemy.GetComponentInParent<Health>() != null)
		{
			return 2.7f;
		}
		if (enemy.GetComponentInParent<Soldier>() != null)
		{
			return 2f;
		}
		return 1f;
	}
}
