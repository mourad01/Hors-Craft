// DecompilerFi decompiler from Assembly-CSharp.dll class: StationaryEnemy
using com.ootii.Cameras;
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

public class StationaryEnemy : EnemyMob
{
	[SerializeField]
	private float shootRange;

	[SerializeField]
	private float cooldown;

	[SerializeField]
	private float chanceToHit = 1f;

	[SerializeField]
	private float shootMaxOffDegrees = 25f;

	[SerializeField]
	private GameObject baseObject;

	[SerializeField]
	private GameObject barrel;

	[SerializeField]
	private bool turnBarrel = true;

	[SerializeField]
	private GameObject shootTrailPrefab;

	[SerializeField]
	private GameObject projectilePrefab;

	[SerializeField]
	private AudioClip shootSound;

	[SerializeField]
	private GameObject deathAnimation;

	private const float BULLET_VELOCITY = 50f;

	protected CooldownNode shootCooldownNode;

	private LookAtGameObjectNode lookAtGameObjectNode;

	private GameObject target;

	private float shootDistance;

	private LineRenderer _shootTrails;

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

	protected override float attackRange => shootRange;

	protected override void Start()
	{
		base.Start();
		target = CameraController.instance.MainCamera.gameObject;
		lookAtGameObjectNode = new LookAtGameObjectNode(this);
		lookAtGameObjectNode.SetTarget(target);
	}

	public List<GameObject> GetTargetAsList()
	{
		List<GameObject> list = GetTargets();
		if (list.Count > 0)
		{
			GameObject gameObject = list[0];
			foreach (GameObject item in list)
			{
				if (Vector3.Distance(base.transform.position, item.transform.position) < Vector3.Distance(base.transform.position, gameObject.transform.position))
				{
					gameObject = item;
				}
			}
			VehicleController componentInParent = gameObject.GetComponentInParent<VehicleController>();
			if ((bool)componentInParent)
			{
				gameObject = componentInParent.gameObject;
			}
			List<GameObject> list2 = new List<GameObject>();
			list2.Add(gameObject);
			list = list2;
			target = gameObject;
		}
		return list;
	}

	protected override ParallelNode ConstructTopParallelNode()
	{
		ParallelNode parallelNode = new ParallelNode();
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(ConstructDieBehaviour());
		selectorNode.Add(ConstructShootEnemies());
		parallelNode.Add(selectorNode);
		parallelNode.Add(lookAtGameObjectNode);
		return parallelNode;
	}

	public override void LookAt(GameObject target)
	{
		Quaternion to = Quaternion.LookRotation(target.transform.position - baseObject.transform.position, Vector3.up);
		Vector3 eulerAngles = Quaternion.RotateTowards(baseObject.transform.rotation, to, 80f * Time.deltaTime).eulerAngles;
		float y = eulerAngles.y;
		Vector3 eulerAngles2 = to.eulerAngles;
		float x = eulerAngles2.x;
		Vector3 eulerAngles3 = barrel.transform.rotation.eulerAngles;
		RotateBarrel(x, eulerAngles3);
		Vector3 eulerAngles4 = baseObject.transform.rotation.eulerAngles;
		RotateBase(y, eulerAngles4);
	}

	private void RotateBarrel(float rotationX, Vector3 currentRotation)
	{
		float num = 0f;
		if (turnBarrel)
		{
			num = 1f;
		}
		barrel.transform.localEulerAngles = new Vector3(rotationX, 0f, currentRotation.z + num);
	}

	private void RotateBase(float rotationY, Vector3 currentRotation)
	{
		baseObject.transform.rotation = Quaternion.Euler(new Vector3(currentRotation.x, rotationY, currentRotation.z));
	}

	protected Node ConstructShootEnemies()
	{
		SequenceNode sequenceNode = new SequenceNode();
		shootCooldownNode = new CooldownNode();
		sequenceNode.Add(shootCooldownNode);
		sequenceNode.Add(new AttackEnemiesInAttackRangeNode(this, GetTargetAsList, GetDistanceMultiplier));
		return sequenceNode;
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

	public override void Attack(Transform target)
	{
		if ((bool)this.target)
		{
			Shoot(target.gameObject);
		}
	}

	private void Shoot(GameObject target)
	{
		if ((bool)shootSound)
		{
			PlaySound(shootSound);
		}
		if (chanceToHit >= UnityEngine.Random.Range(0f, 1f))
		{
			DealDamage(this.target);
		}
		shootCooldownNode.SetCooldown(cooldown);
		Vector3 normalized = (target.transform.position - barrel.transform.position).normalized;
		normalized = Vector3.Lerp(normalized, UnityEngine.Random.insideUnitSphere, shootMaxOffDegrees / 360f);
		Vector3 vector = barrel.transform.position + normalized * shootRange;
		shootDistance = Vector3.Distance(base.transform.position, vector);
		if ((bool)shootTrailPrefab)
		{
			StartCoroutine(ShootTrail(barrel.transform.position, normalized, vector));
		}
		if ((bool)projectilePrefab)
		{
			FireProjectile();
		}
	}

	protected virtual void FireProjectile()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(projectilePrefab, barrel.transform.position, barrel.transform.rotation);
		gameObject.GetComponentInChildren<Projectilemove>().stopEmittingAfterMeters = shootDistance;
		gameObject.GetComponentInChildren<DestroyAfter>().delay = shootDistance / 50f;
	}

	protected IEnumerator ShootTrail(Vector3 fr, Vector3 forward, Vector3 to)
	{
		shootTrails.enabled = true;
		int count = Mathf.Max(2, Mathf.CeilToInt((to - fr).magnitude) / 10);
		shootTrails.positionCount = count;
		for (int j = 0; j < count; j++)
		{
			shootTrails.SetPosition(j, Vector3.Lerp(fr, to, (float)(j + 1) / (float)count));
		}
		shootTrails.SetPosition(count - 1, to - forward * 0.25f);
		Color c3 = Color.red;
		Color c2 = new Color32(byte.MaxValue, 129, 6, byte.MaxValue);
		for (int i = 0; i < 15; i++)
		{
			c3.a = (c2.a = 1f - (float)(i + 1) * (71f / (339f * (float)Math.PI)));
			_shootTrails.startColor = c3;
			_shootTrails.endColor = c2;
			yield return new WaitForFixedUpdate();
		}
		shootTrails.enabled = false;
	}

	private void DealDamage(GameObject target)
	{
		Health componentInParent = target.GetComponentInParent<Health>();
		if (!(componentInParent != null))
		{
			return;
		}
		bool flag = componentInParent.hp > 0f;
		componentInParent.OnHit(dmg, target.transform.position - base.transform.position);
		if (flag && componentInParent.hp <= 0f)
		{
			IFighting componentInParent2 = target.GetComponentInParent<IFighting>();
			if (componentInParent2 != null && componentInParent2.IsEnemy())
			{
				Manager.Get<QuestManager>().OnEnemyKilled();
			}
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
	}

	public override void Die()
	{
		if ((bool)deathAnimation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(deathAnimation);
			gameObject.transform.position = base.transform.position;
		}
		base.Die();
	}

	private void PlaySound(AudioClip clip)
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
}
