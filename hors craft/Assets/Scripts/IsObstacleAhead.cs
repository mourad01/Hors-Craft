// DecompilerFi decompiler from Assembly-CSharp.dll class: IsObstacleAhead
using Common.BehaviourTrees;
using UnityEngine;

public class IsObstacleAhead : MobNode
{
	private float range;

	public IsObstacleAhead(Mob mob, float r = 5f)
		: base(mob)
	{
		range = r;
	}

	public override void Update()
	{
		Ray ray = new Ray(base.mob.transform.position, base.mob.transform.forward);
		if (base.mob.forwardObstacleIndicator.active || Physics.Raycast(ray, out RaycastHit _, range, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			base.status = Status.SUCCESS;
		}
		else
		{
			base.status = Status.FAILURE;
		}
	}
}
