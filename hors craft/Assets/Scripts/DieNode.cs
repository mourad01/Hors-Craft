// DecompilerFi decompiler from Assembly-CSharp.dll class: DieNode
using Common.BehaviourTrees;

public class DieNode : MobNode
{
	public DieNode(Mob mob)
		: base(mob)
	{
	}

	public override void Update()
	{
		base.mob.Die();
		base.status = Status.SUCCESS;
	}
}
