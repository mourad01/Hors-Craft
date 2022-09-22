// DecompilerFi decompiler from Assembly-CSharp.dll class: AntiTankEnemy
using Common.Audio;
using Common.Behaviours;
using Common.BehaviourTrees;
using Common.Managers;
using Common.Managers.Audio;
using Common.Utils;
using Gameplay.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AntiTankEnemy : ShootingEnemy
{
	public GameObject shootParticles;

	public GameObject explosionPrefab;

	public float explosionRange;

	protected override void Shoot(Transform target)
	{
		Transform transform = base.transform;
		Vector3 up = Vector3.up;
		Vector3 eulerAngles = Quaternion.LookRotation(target.transform.position - base.transform.position).eulerAngles;
		transform.eulerAngles = up * eulerAngles.y;
		base.animator.SetTrigger("shoot");
		Transform transform2 = shootStartTransforms.Random();
		Vector3 normalized = (target.position - transform2.position).normalized;
		normalized = Vector3.Lerp(normalized, UnityEngine.Random.insideUnitSphere, shootMaxOffDegrees / 360f);
		Vector3 hitPoint = transform2.position + normalized * shootRange;
		if (Physics.Raycast(transform2.position, normalized, out RaycastHit hitInfo, shootRange, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			hitPoint = hitInfo.point;
		}
		CreateFakeBulletExplosion(hitPoint, transform2);
		shootCooldownNode.SetCooldown(afterShootCooldown);
		ShootEffects(hitPoint, transform2);
	}

	private void ShootEffects(Vector3 hitPoint, Transform shootStartTransform)
	{
		Sound sound = new Sound();
		sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.ENEMY_SHOOT);
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		sound.volumeFrom = Sound.EVolumeFrom.POSITION;
		Sound sound2 = sound;
		sound2.Play(base.transform);
		if (shootParticles != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(shootParticles, shootStartTransform.position, Quaternion.LookRotation(hitPoint - shootStartTransform.position));
			gameObject.GetComponentInChildren<Projectilemove>().stopEmittingAfterMeters = Vector3.Distance(hitPoint, shootStartTransform.position);
			gameObject.GetComponentInChildren<DestroyAfter>().delay = 5f;
		}
	}

	private void CreateFakeBulletExplosion(Vector3 hitPoint, Transform shootStartTransform)
	{
		GameObject gameObject = new GameObject("Fake Bullet");
		TriggerTargetAcquisition triggerTargetAcquisition = gameObject.AddComponent<TriggerTargetAcquisition>();
		triggerTargetAcquisition.setRange = explosionRange;
		VoxelExplosion voxelExplosion = gameObject.AddComponent<VoxelExplosion>();
		voxelExplosion.radius = explosionRange;
		voxelExplosion.baseDamage = dmgShoot;
		voxelExplosion.maxDamage = 0f;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(explosionPrefab);
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		gameObject2.transform.localPosition = Vector3.zero;
		voxelExplosion.explosionSystem = gameObject2.GetComponent<ParticleSystem>();
		gameObject.transform.position = hitPoint;
		float time = Vector3.Distance(shootStartTransform.position, hitPoint) / 50f;
		StartCoroutine(ExplodeAfter(gameObject, time));
	}

	private IEnumerator ExplodeAfter(GameObject bullet, float time)
	{
		yield return new WaitForSeconds(time);
		VoxelExplosion voxelExplosion = bullet.GetComponent<VoxelExplosion>();
		//voxelExplosion.ExplodeArea((Func<GameObject, bool>)((ShootingEnemy)this).EnemySpottedCondition, digDown: false, canDestroyAllBlocks: false);
		UnityEngine.Object.Destroy(bullet);
	}

	protected override bool EnemySpottedCondition(GameObject c)
	{
		return base.EnemySpottedCondition(c) || (c.GetComponentInParent<VehicleController>() != null && c.GetComponentInParent<Health>() != null);
	}

	protected override Node ConstructRunToPlayer()
	{
		LoopNode loopNode = new LoopNode();
		loopNode.Add(new SetDestinationNearPlayer(this, 8f));
		loopNode.Add(new GoToDestinationNode(this, runSpeed));
		return loopNode;
	}
}
