// DecompilerFi decompiler from Assembly-CSharp.dll class: IsTimePassedNode
using Common.BehaviourTrees;
using UnityEngine;

internal class IsTimePassedNode : MobNode
{
	private float duration;

	private float endTime;

	private bool isStarted;

	public IsTimePassedNode(Mob mob, float duration)
		: base(mob)
	{
		this.duration = duration;
		isStarted = false;
	}

	public override void Update()
	{
		if (!isStarted)
		{
			endTime = Time.time + duration;
			isStarted = true;
		}
		if (Time.time > endTime)
		{
			base.status = Status.SUCCESS;
		}
		else
		{
			base.status = Status.FAILURE;
		}
	}
}
