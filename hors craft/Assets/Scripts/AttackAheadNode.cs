// DecompilerFi decompiler from Assembly-CSharp.dll class: AttackAheadNode
using Common.BehaviourTrees;

public class AttackAheadNode : MobNode
{
	public AttackAheadNode(IFighting mob)
		: base(mob.GetMob())
	{
	}

	public override void Update()
	{
		(base.mob as IFighting).Attack(null);
		base.status = Status.SUCCESS;
	}
}
