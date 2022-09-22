// DecompilerFi decompiler from Assembly-CSharp.dll class: TankMachineGun
using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMachineGun : TankWeapon
{
	private TriggerTargetAcquisition targetAcquisition;

	public bool useCrosshair;

	public float projectileSpread = 30f;

	public float timeBetweenShootSound = 0.25f;

	private float currentTimeBetweenShootSound;

	private bool isConstantSoundPlaying;

	private void Awake()
	{
		targetAcquisition = GetComponent<TriggerTargetAcquisition>();
		if (targetAcquisition == null)
		{
			targetAcquisition = base.gameObject.AddComponent<TriggerTargetAcquisition>();
		}
		currentTimeBetweenShootSound = timeBetweenShootSound;
	}

	protected override void Update()
	{
		base.Update();
		currentTimeBetweenShootSound += Time.deltaTime;
		if (isConstantSoundPlaying && currentTimeBetweenShootSound > timeBetweenShootSound)
		{
			PlaySound(shootClip);
			currentTimeBetweenShootSound = 0f;
		}
	}

	public override void OnPress()
	{
		base.OnPress();
		if (shooting && magazineBulletsLeft > 0)
		{
			Vector3 forward = muzzle.transform.forward;
			forward = Vector3.Lerp(forward, UnityEngine.Random.insideUnitSphere, projectileSpread / 360f);
			if (shootTrailPrefab != null)
			{
				StartCoroutine(ShootTrail(muzzle.transform.position, forward, muzzle.transform.position + base.transform.forward * distance + UnityEngine.Random.insideUnitSphere * projectileSpread));
			}
		}
		isConstantSoundPlaying = true;
	}

	public override void OnRelease()
	{
		base.OnRelease();
		isConstantSoundPlaying = false;
	}

	protected override void Shoot()
	{
		if (useCrosshair)
		{
			ShootWithCrosshair();
			return;
		}
		List<GameObject> targets = targetAcquisition.GetTargets(TargetRecognision);
		Transform transform = null;
		Health health = null;
		bool flag = false;
		if (targets != null && targets.Count > 0)
		{
			transform = targets.Random().transform;
			health = transform.GetComponentInParent<Health>();
			flag = (health.hp > 0f);
		}
		Vector3 vector = muzzle.transform.forward;
		float maxDistance = distance;
		if (transform != null)
		{
			vector = (transform.position - muzzle.transform.position).normalized;
			maxDistance = Vector3.Distance(transform.position, muzzle.transform.position);
		}
		Ray ray = new Ray(muzzle.transform.position + vector * shootForwardFromMuzzleDistance, vector);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			Health componentInParent = hitInfo.collider.gameObject.GetComponentInParent<Health>();
			if (componentInParent == null)
			{
				health = null;
			}
			else if (health != componentInParent && TargetRecognision(componentInParent.gameObject))
			{
				health = componentInParent;
				transform = hitInfo.collider.transform;
			}
		}
		if (health != null)
		{
			health.OnHit(damage, vector, 0.1f);
			if (flag && health.hp <= 0f)
			{
				TargetDiedEffect(health);
			}
		}
		ShootEffects();
	}

	private void ShootWithCrosshair()
	{
		Crosshair.instance.SetParameters(this);
		Crosshair.instance.FindTargets();
		if (Crosshair.instance.IsOutOfRange())
		{
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.SURVIVAL_OUT_OF_RANGE);
		}
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
		ShootEffects();
	}

	protected override IEnumerator ShootTrail(Vector3 fr, Vector3 forward, Vector3 to)
	{
		base.shootTrails.enabled = true;
		int count = Mathf.Max(2, Mathf.CeilToInt((to - fr).magnitude) / 10);
		base.shootTrails.positionCount = count;
		for (int j = 0; j < count; j++)
		{
			base.shootTrails.SetPosition(j, Vector3.Lerp(fr, to, (float)(j + 1) / (float)count));
		}
		base.shootTrails.SetPosition(count - 1, to - forward * 0.25f);
		Color c3 = Color.red;
		Color c2 = new Color32(byte.MaxValue, 129, 6, byte.MaxValue);
		for (int i = 0; i < 15; i++)
		{
			c3.a = (c2.a = 1f - (float)(i + 1) * (71f / (339f * (float)Math.PI)));
			_shootTrails.startColor = c3;
			_shootTrails.endColor = c2;
			yield return new WaitForFixedUpdate();
		}
		base.shootTrails.enabled = false;
	}

	private bool TargetRecognision(GameObject target)
	{
		IFighting fighting = target.GetComponentInParent<Mob>() as IFighting;
		return fighting != null && fighting.IsEnemy() && !(fighting is TankEnemy);
	}

	private void OnDestroy()
	{
		if (_shootTrails != null)
		{
			_shootTrails.enabled = false;
			UnityEngine.Object.Destroy(_shootTrails.gameObject);
		}
	}
}
