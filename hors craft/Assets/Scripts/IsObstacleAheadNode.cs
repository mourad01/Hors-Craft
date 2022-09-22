// DecompilerFi decompiler from Assembly-CSharp.dll class: IsObstacleAheadNode
using Common.BehaviourTrees;
using UnityEngine;

public class IsObstacleAheadNode : MobNode
{
	private float checkInterval;

	private TriggerIndicator forwardObstacleIndicator;

	private float nextCheckTime;

	public IsObstacleAheadNode(Mob mob, float interval)
		: base(mob)
	{
		checkInterval = interval;
	}

	public override void Update()
	{
		if (forwardObstacleIndicator == null)
		{
			forwardObstacleIndicator = base.mob.forwardObstacleIndicator;
		}
		base.status = Status.FAILURE;
		if (Time.time > nextCheckTime)
		{
			if (forwardObstacleIndicator.active)
			{
				base.status = Status.SUCCESS;
			}
			nextCheckTime = Time.time + checkInterval;
		}
	}
}
