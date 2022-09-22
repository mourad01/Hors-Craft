// DecompilerFi decompiler from Assembly-CSharp.dll class: IsDeadNode
using Common.BehaviourTrees;

public class IsDeadNode : MobNode
{
	private Health health;

	public IsDeadNode(Mob mob, Health health)
		: base(mob)
	{
		this.health = health;
	}

	public override void Update()
	{
		if (health.IsDead())
		{
			base.status = Status.SUCCESS;
		}
		else
		{
			base.status = Status.FAILURE;
		}
	}
}
