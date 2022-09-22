// DecompilerFi decompiler from Assembly-CSharp.dll class: LookAtGameObjectNode
using Common.BehaviourTrees;
using UnityEngine;

public class LookAtGameObjectNode : MobNode
{
	private GameObject target;

	public LookAtGameObjectNode(Mob mob)
		: base(mob)
	{
	}

	public override void Update()
	{
		if (!(target == null))
		{
			base.mob.LookAt(target);
			base.status = Status.SUCCESS;
		}
	}

	public void SetTarget(GameObject target)
	{
		this.target = target;
	}
}
