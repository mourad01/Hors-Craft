// DecompilerFi decompiler from Assembly-CSharp.dll class: IsCloseToWaterNode
using Common.BehaviourTrees;
using Uniblocks;
using UnityEngine;

public class IsCloseToWaterNode : MobNode
{
	private float waterHeight;

	public IsCloseToWaterNode(Mob mob)
		: base(mob)
	{
		waterHeight = Engine.TerrainGenerator.waterHeight;
	}

	public override void Update()
	{
		Vector3 position = base.mob.transform.position;
		if (position.y < waterHeight + 1f)
		{
			base.status = Status.SUCCESS;
		}
		else
		{
			base.status = Status.FAILURE;
		}
	}
}
