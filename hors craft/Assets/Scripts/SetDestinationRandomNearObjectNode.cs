// DecompilerFi decompiler from Assembly-CSharp.dll class: SetDestinationRandomNearObjectNode
using Common.BehaviourTrees;
using UnityEngine;

public class SetDestinationRandomNearObjectNode : MobNode
{
	private float minDestinationDistance;

	private float maxDestinationDistance;

	private Vector3 target;

	public SetDestinationRandomNearObjectNode(Mob mob, Vector3 target, float minDestinationDistance = 0f, float maxDestinationDistance = 20f)
		: base(mob)
	{
		this.minDestinationDistance = minDestinationDistance;
		this.maxDestinationDistance = maxDestinationDistance;
		this.target = target;
	}

	public override void Update()
	{
		float d = Random.Range(minDestinationDistance, maxDestinationDistance);
		base.mob.navigator.SetDestination(target + Random.insideUnitSphere.normalized * d);
		base.status = Status.SUCCESS;
	}
}
