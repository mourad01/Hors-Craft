// DecompilerFi decompiler from Assembly-CSharp.dll class: ShootAndReturn
using UnityEngine;

public class ShootAndReturn : ObjectPoolItem
{
	public Transform target;

	public float speed = 5f;

	private float currentRot;

	private float distanceToTarget;

	private float lifeTime;

	private const float DISTANCE_CHECK_INTERVAL = 0.6f;

	private const float MIN_DISTANCE_TO_TARGET = 1f;

	public override void Init(int id)
	{
		if (target == null)
		{
			target = PlayerGraphic.GetControlledPlayerInstance().gameObject.transform;
		}
		distanceToTarget = float.MaxValue;
		lifeTime = 0f;
		base.Init(id);
	}

	private void Update()
	{
		if (target == null)
		{
			target = PlayerGraphic.GetControlledPlayerInstance().gameObject.transform;
		}
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0f)
		{
			distanceToTarget = Vector3.Distance(base.transform.position, target.position);
			lifeTime = 0.6f;
		}
		if (distanceToTarget < 1f)
		{
			Fixable component = target.parent.GetComponent<Fixable>();
			if (component != null)
			{
				component.Fix();
			}
			base.End();
		}
		base.transform.Translate(Vector3.forward * speed * 2f * Time.deltaTime);
		Vector3 toDirection = target.position - base.transform.position;
		Quaternion b = Quaternion.FromToRotation(Vector3.forward, toDirection);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, speed / 50f * (currentRot += Time.deltaTime));
	}
}
