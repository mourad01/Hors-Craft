// DecompilerFi decompiler from Assembly-CSharp.dll class: CanGoToPlayer
using Common.BehaviourTrees;
using UnityEngine;

public class CanGoToPlayer : MobNode
{
	private float range;

	public CanGoToPlayer(Mob mob, float r = 5f)
		: base(mob)
	{
		range = r;
	}

	public override void Update()
	{
		Transform transform = PlayerGraphic.GetControlledPlayerInstance().transform;
		Ray ray = new Ray(base.mob.transform.position, transform.position - base.mob.transform.position);
		if (Physics.Raycast(ray, out RaycastHit _, range, int.MaxValue, QueryTriggerInteraction.Ignore))
		{
			base.status = Status.FAILURE;
		}
		else
		{
			base.status = Status.SUCCESS;
		}
	}
}
