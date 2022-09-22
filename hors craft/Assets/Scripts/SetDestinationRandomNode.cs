// DecompilerFi decompiler from Assembly-CSharp.dll class: SetDestinationRandomNode
using Common.BehaviourTrees;
using UnityEngine;

public class SetDestinationRandomNode : MobNode
{
	private float minDestinationDistance;

	private float maxDestinationDistance;

	public SetDestinationRandomNode(Mob mob, float minDestinationDistance = 0f, float maxDestinationDistance = 10f)
		: base(mob)
	{
		this.minDestinationDistance = minDestinationDistance;
		this.maxDestinationDistance = maxDestinationDistance;
	}

	public override void Update()
	{
		float d = Random.Range(minDestinationDistance, maxDestinationDistance);
		base.mob.navigator.SetDestination(base.mob.transform.position + Random.insideUnitSphere.normalized * d);
		base.status = Status.SUCCESS;
	}
}
