// DecompilerFi decompiler from Assembly-CSharp.dll class: IsCloseToTargetNode
using Common.BehaviourTrees;
using UnityEngine;

public class IsCloseToTargetNode : MobNode
{
	private const float TARGET_IS_CLOSE_DISTANCE = 10f;

	private float closeDistance;

	public IsCloseToTargetNode(Mob mob, float distance = 10f)
		: base(mob)
	{
		closeDistance = distance;
	}

	public override void Update()
	{
		Vector3 vector = base.mob.transform.position - base.mob.navigator.GetDestination();
		vector.y = 0f;
		if (vector.magnitude < closeDistance)
		{
			base.status = Status.SUCCESS;
		}
		else
		{
			base.status = Status.FAILURE;
		}
	}
}
