// DecompilerFi decompiler from Assembly-CSharp.dll class: ResourceWaitNode
using Common.BehaviourTrees;
using UnityEngine;

public class ResourceWaitNode : Node
{
	private float endTime;

	private float cd;

	private Pettable pettable;

	public ResourceWaitNode(Mob mob, float cooldown = 0f)
	{
		pettable = mob.GetComponent<Pettable>();
		mob.GetComponent<MobNavigator>().ForceEndMovement();
		endTime = Time.time + cooldown;
	}

	public override void Update()
	{
		if (Time.time > endTime)
		{
			base.status = Status.SUCCESS;
			pettable.FollowPlayerMode();
		}
		else
		{
			base.status = Status.RUNNING;
		}
	}
}
