// DecompilerFi decompiler from Assembly-CSharp.dll class: IsMobVisibleNode
using Common.BehaviourTrees;
using System.Linq;
using UnityEngine;

public class IsMobVisibleNode : MobNode
{
	private Renderer[] mobRenderers;

	public IsMobVisibleNode(Mob mob)
		: base(mob)
	{
		mobRenderers = (from r in mob.GetComponentsInChildren<Renderer>()
			where !(r is ParticleSystemRenderer)
			select r).ToArray();
	}

	public override void Update()
	{
		if (mobRenderers.Any((Renderer m) => m.isVisible))
		{
			base.status = Status.SUCCESS;
		}
		else
		{
			base.status = Status.FAILURE;
		}
	}
}
