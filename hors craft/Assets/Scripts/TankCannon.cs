// DecompilerFi decompiler from Assembly-CSharp.dll class: TankCannon
using Common.Behaviours;
using System.Collections;
using UnityEngine;

public class TankCannon : TankWeapon
{
	public float explosionRange = 4f;

	public float baseExplosionDmg;

	public GameObject explosionPrefab;

	public const float BULLET_VELOCITY = 50f;

	private float shootDistance;

	public bool allowFakeBullets;

	public GameObject[] fakeBullets;

	protected override void Shoot()
	{
		Vector3 forward = muzzle.transform.forward;
		Vector3 hitPoint = muzzle.transform.position + forward * distance;
		Ray ray = new Ray(muzzle.transform.position + forward * shootForwardFromMuzzleDistance, forward);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			hitPoint = hitInfo.point;
		}
		CreateFakeBulletExplosion(hitPoint);
		ShootEffects();
		if (allowFakeBullets)
		{
			ShootFakeBullets();
		}
		magazineBulletsLeft--;
	}

	private void ShootFakeBullets()
	{
		StartCoroutine(ShootFakeProjectiles());
	}

	private IEnumerator ShootFakeProjectiles()
	{
		for (int i = 0; i < fakeBullets.Length; i++)
		{
			int index = Random.Range(0, fakeBullets.Length - 1);
			ShootFakeProjectile(fakeBullets[index].transform.position);
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void ShootFakeProjectile(Vector3 position)
	{
		if (ProjectilePrefab != null)
		{
			GameObject gameObject = Object.Instantiate(ProjectilePrefab, position, projectilePos.transform.rotation);
			gameObject.GetComponentInChildren<Projectilemove>().stopEmittingAfterMeters = shootDistance;
			gameObject.GetComponentInChildren<DestroyAfter>().delay = shootDistance / 50f;
		}
	}

	private void CreateFakeBulletExplosion(Vector3 hitPoint)
	{
		GameObject gameObject = new GameObject("Fake Bullet");
		TriggerTargetAcquisition triggerTargetAcquisition = gameObject.AddComponent<TriggerTargetAcquisition>();
		triggerTargetAcquisition.setRange = explosionRange;
		VoxelExplosion voxelExplosion = gameObject.AddComponent<VoxelExplosion>();
		voxelExplosion.radius = explosionRange;
		voxelExplosion.maxDamage = damage;
		voxelExplosion.baseDamage = baseExplosionDmg;
		GameObject gameObject2 = Object.Instantiate(explosionPrefab);
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		gameObject2.transform.localPosition = Vector3.zero;
		voxelExplosion.explosionSystem = gameObject2.GetComponent<ParticleSystem>();
		gameObject.transform.position = hitPoint;
		shootDistance = Vector3.Distance(base.transform.position, hitPoint);
		float time = shootDistance / 50f;
		StartCoroutine(ExplodeAfter(gameObject, time));
	}

	protected override void FireProjectile()
	{
		if (ProjectilePrefab != null)
		{
			GameObject gameObject = Object.Instantiate(ProjectilePrefab, projectilePos.transform.position, projectilePos.transform.rotation);
			gameObject.GetComponentInChildren<Projectilemove>().stopEmittingAfterMeters = shootDistance;
			gameObject.GetComponentInChildren<DestroyAfter>().delay = shootDistance / 50f;
		}
	}

	private IEnumerator ExplodeAfter(GameObject bullet, float time)
	{
		yield return new WaitForSeconds(time);
		VoxelExplosion voxelExplosion = bullet.GetComponent<VoxelExplosion>();
		voxelExplosion.ExplodeArea(digDown: false);
		UnityEngine.Object.Destroy(bullet);
	}
}
