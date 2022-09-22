// DecompilerFi decompiler from Assembly-CSharp.dll class: AvoidObstacle
using Common.BehaviourTrees;
using UnityEngine;

public class AvoidObstacle : MobNode
{
	private float range;

	public AvoidObstacle(Mob mob, float r = 8f)
		: base(mob)
	{
		range = r;
	}

	public override void Update()
	{
		float num = DoRaycast(new Ray(base.mob.groundPosition + Vector3.up * 0.5f, base.mob.transform.forward - base.mob.transform.right * 0.6f));
		float num2 = DoRaycast(new Ray(base.mob.groundPosition + Vector3.up * 0.5f, base.mob.transform.forward + base.mob.transform.right * 0.6f));
		float num3 = 0f;
		num3 = ((!(num > num2)) ? Mathf.Lerp(60f, 0f, num / (range - 2f)) : Mathf.Lerp(-60f, 0f, num2 / (range - 2f)));
		base.mob.GetComponent<MobNavigator>().SetDestination(base.mob.transform.position + Quaternion.Euler(0f, num3 * Time.deltaTime, 0f) * (base.mob.transform.forward * range));
		base.status = Status.SUCCESS;
	}

	private float DoRaycast(Ray ray)
	{
		if (Physics.Raycast(ray, out RaycastHit hitInfo, range, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			return hitInfo.distance;
		}
		return range;
	}
}
