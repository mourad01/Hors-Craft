// DecompilerFi decompiler from Assembly-CSharp.dll class: IsPlayerNotInLineOfSight
using Common.BehaviourTrees;
using UnityEngine;

public class IsPlayerNotInLineOfSight : MobNode
{
	public IsPlayerNotInLineOfSight(Mob mob)
		: base(mob)
	{
	}

	public override void Update()
	{
		Transform transform = PlayerGraphic.GetControlledPlayerInstance().transform;
		Ray ray = new Ray(base.mob.transform.position + Vector3.up * 0.3f, transform.position - base.mob.transform.position);
		float maxDistance = Vector3.Distance(base.mob.transform.position, transform.position);
		int mask = LayerMask.GetMask("Default");
		if (Physics.Raycast(ray, out RaycastHit _, maxDistance, mask, QueryTriggerInteraction.Ignore))
		{
			base.status = Status.SUCCESS;
		}
		else
		{
			base.status = Status.FAILURE;
		}
	}
}
