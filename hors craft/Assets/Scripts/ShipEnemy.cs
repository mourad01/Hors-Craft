// DecompilerFi decompiler from Assembly-CSharp.dll class: ShipEnemy
using Common.Audio;
using Common.Behaviours;
using Common.BehaviourTrees;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class ShipEnemy : EnemyMob
{
	public float rotationSpeed = 20f;

	public float shootRange = 20f;

	public float shootMaxOffDegrees = 5f;

	public GameObject shootTrailPrefab;

	public Transform[] shootStartTransforms;

	public GameObject explosionPrefab;

	public GameObject shootParticles;

	public GameObject turret;

	public float explosionRange;

	public float afterShootCooldown = 2.5f;

	private const float AFTER_ATTACK_COOLDOWN = 2.5f;

	protected CooldownNode shootCooldownNode;

	protected ShipEnemyCannon[] cannons;

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

	protected override void Start()
	{
		base.Start();
		List<ShipEnemyCannon> list = new List<ShipEnemyCannon>();
		for (int i = 0; i < shootStartTransforms.Length; i++)
		{
			ShipEnemyCannon componentInChildren = shootStartTransforms[i].GetComponentInChildren<ShipEnemyCannon>();
			if (componentInChildren != null)
			{
				list.Add(componentInChildren);
			}
		}
		if (list.Count > 0)
		{
			cannons = list.ToArray();
		}
	}

	public override void Attack(Transform target)
	{
		Shoot(target);
	}

	private void Shoot(Transform target)
	{
		if (cannons != null && cannons.Length > 0)
		{
			for (int i = 0; i < cannons.Length; i++)
			{
				ShootFromPoint(cannons[i].shootPoint, target);
			}
			return;
		}
		Transform[] array = shootStartTransforms;
		foreach (Transform cannonPoint in array)
		{
			ShootFromPoint(cannonPoint, target);
		}
	}

	protected void ShootFromPoint(Transform cannonPoint, Transform target)
	{
		Vector3 hitPoint = cannonPoint.position + cannonPoint.forward * shootRange;
		if (Physics.Raycast(cannonPoint.position, cannonPoint.forward, out RaycastHit hitInfo, shootRange, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			hitPoint = hitInfo.point;
		}
		CreateFakeBulletExplosion(hitPoint);
		shootCooldownNode.SetCooldown(afterShootCooldown);
		ShootEffects(cannonPoint, hitPoint);
	}

	private void ShootEffects(Transform shootStartTransform, Vector3 hitPoint)
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
		Quaternion to = Quaternion.LookRotation(target - base.transform.position, Vector3.up);
		Vector3 eulerAngles = Quaternion.RotateTowards(base.transform.rotation, to, 40f * Time.deltaTime).eulerAngles;
		float y = eulerAngles.y;
		Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
		base.transform.rotation = Quaternion.Euler(new Vector3(eulerAngles2.x, y, eulerAngles2.z));
	}

	public void LootAtSideway(Vector3 target)
	{
		Quaternion to = Quaternion.LookRotation(target - base.transform.position, Vector3.up);
		to.eulerAngles += new Vector3(0f, 90f, 0f);
		Vector3 eulerAngles = Quaternion.RotateTowards(base.transform.rotation, to, rotationSpeed * Time.deltaTime).eulerAngles;
		float y = eulerAngles.y;
		Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
		base.transform.rotation = Quaternion.Euler(new Vector3(eulerAngles2.x, y, eulerAngles2.z));
	}

	protected override ParallelNode ConstructTopParallelNode()
	{
		ParallelNode parallelNode = new ParallelNode();
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(ConstructDieBehaviour());
		selectorNode.Add(ConstructShootEnemies());
		selectorNode.Add(ConstructMovementLogic());
		parallelNode.Add(selectorNode);
		return parallelNode;
	}

	protected Node ConstructShootEnemies()
	{
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(new IsPlayerCloseNode(this, shootRange));
		sequenceNode.Add(new AimAtTargetNode(this, shootMaxOffDegrees));
		shootCooldownNode = new CooldownNode();
		sequenceNode.Add(shootCooldownNode);
		sequenceNode.Add(new AttackAheadNode(this));
		return sequenceNode;
	}

	private Node ConstructMovementLogic()
	{
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(ConstructRunToPlayer());
		return selectorNode;
	}

	private Node ConstructObstacleAvoidance()
	{
		SelectorNode selectorNode = new SelectorNode();
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(new ShouldAvoidObstacle(this, 25f));
		sequenceNode.Add(new AvoidObstacle(this, 25f));
		sequenceNode.Add(new GoToDestinationNode(this, runSpeed));
		selectorNode.Add(new IsPlayerCloseNode(this, shootRange));
		selectorNode.Add(sequenceNode);
		return selectorNode;
	}

	private Node ConstructRunToPlayer()
	{
		SelectorNode selectorNode = new SelectorNode();
		SequenceNode sequenceNode = new SequenceNode();
		selectorNode.Add(new IsPlayerCloseNode(this, shootRange * 1.2f));
		selectorNode.Add(sequenceNode);
		sequenceNode.Add(new LookAtPlayerNode(this));
		sequenceNode.Add(new SetDestinationNearPlayer(this, shootRange));
		sequenceNode.Add(new GoToDestinationNode(this, runSpeed));
		return selectorNode;
	}

	private IEnumerator ExplodeAfter(GameObject bullet, float time)
	{
		yield return new WaitForSeconds(time);
		VoxelExplosion voxelExplosion = bullet.GetComponent<VoxelExplosion>();
		//voxelExplosion.ExplodeArea((Func<GameObject, bool>)((EnemyMob)this).EnemySpottedCondition, digDown: false, canDestroyAllBlocks: false);
		UnityEngine.Object.Destroy(bullet);
	}

	public override bool EnemySpottedCondition(GameObject c)
	{
		if (c.GetComponentInParent<Health>() == null)
		{
			return false;
		}
		Transform transform = c.transform;
		while (transform.GetComponent<Health>() == null)
		{
			transform = transform.parent;
		}
		return base.EnemySpottedCondition(c) || (bool)transform.GetComponentInChildren<VehicleController>();
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

	private void OnDrawGizmosSelected()
	{
		if (cannons == null || cannons.Length <= 0)
		{
			return;
		}
		Gizmos.color = Color.black;
		for (int i = 0; i < cannons.Length; i++)
		{
			Vector3 to = cannons[i].shootPoint.position + cannons[i].shootPoint.forward * shootRange;
			if (Physics.Raycast(cannons[i].shootPoint.position, cannons[i].shootPoint.forward, out RaycastHit hitInfo, shootRange, int.MaxValue, QueryTriggerInteraction.Ignore))
			{
				to = hitInfo.point;
			}
			Gizmos.DrawLine(cannons[i].shootPoint.position, to);
		}
	}
}
