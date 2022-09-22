// DecompilerFi decompiler from Assembly-CSharp.dll class: Grenade
using Uniblocks;
using UnityEngine;

public class Grenade : VoxelExplosion
{
	public float explosionTime = 5f;

	[HideInInspector]
	public Vector3 translation;

	[HideInInspector]
	public float damage;

	[HideInInspector]
	public bool isFriendly = true;

	private const float POSITION_ADJUST_TIME = 0.2f;

	private float positionAdjustEndTime;

	private bool isFlying;

	private Rigidbody body;

	public float flyingTime
	{
		get;
		private set;
	}

	private void Awake()
	{
		baseDamage = damage;
		body = GetComponent<Rigidbody>();
		body.useGravity = false;
		body.isKinematic = true;
		positionAdjustEndTime = 0f;
		InitTargetAcquisition();
	}

	private void Update()
	{
		if (Time.time <= positionAdjustEndTime)
		{
			base.transform.position += translation * (Time.deltaTime / 0.2f);
		}
		Vector3 position = base.transform.position;
		if (position.y < -64f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (isFlying)
		{
			flyingTime += Time.deltaTime;
			if (flyingTime >= explosionTime)
			{
				Explode();
			}
		}
	}

	public void Throw(Vector3 force)
	{
		body.useGravity = true;
		body.isKinematic = false;
		body.AddForce(force, ForceMode.Impulse);
		isFlying = true;
		flyingTime = 0f;
		positionAdjustEndTime = Time.time + 0.2f;
		base.transform.SetParent(null);
		if (!isFriendly)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.GRENADE_INCOMING, new GrenadeIncomingContext
			{
				pivot = PlayerGraphic.GetControlledPlayerInstance().transform,
				grenade = base.transform
			});
		}
	}

	private void Explode()
	{
		if (isFriendly)
		{
			ExplodeArea(digDown: false);
		}
		else
		{
			ExplodeArea((GameObject t) => t.GetComponentInParent<PlayerMovement>() != null || t.GetComponentInParent<Tank>() != null, digDown: false);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		IFighting fighting = collision.collider.gameObject.GetComponentInParent<Mob>() as IFighting;
		if (isFlying && isFriendly && fighting != null && fighting.IsEnemy())
		{
			Explode();
		}
	}
}
