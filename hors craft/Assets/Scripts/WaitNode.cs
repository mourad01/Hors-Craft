// DecompilerFi decompiler from Assembly-CSharp.dll class: WaitNode
using Common.BehaviourTrees;
using UnityEngine;

internal class WaitNode : MobNode
{
	private Status statusOnWait;

	private float from;

	private float to;

	private float endTime;

	public WaitNode(Mob mob, float fromDuration, float toDuration, Status statusToReturn = Status.RUNNING)
		: base(mob)
	{
		from = fromDuration;
		to = toDuration;
		statusOnWait = statusToReturn;
	}

	public WaitNode(Mob mob, float duration, Status statusToReturn = Status.RUNNING)
		: base(mob)
	{
		from = (to = duration);
		statusOnWait = statusToReturn;
	}

	public override void Update()
	{
		if (base.status != statusOnWait)
		{
			endTime = Time.time + Random.Range(from, to);
			base.status = statusOnWait;
		}
		else if (Time.time > endTime)
		{
			base.status = Status.SUCCESS;
		}
	}
}
