// DecompilerFi decompiler from Assembly-CSharp.dll class: IsNotMovingNode
using Common.BehaviourTrees;

public class IsNotMovingNode : MobNode
{
	public IsNotMovingNode(Mob mob)
		: base(mob)
	{
	}

	public override void Update()
	{
		if (base.mob.navigator.moving)
		{
			base.status = Status.FAILURE;
		}
		else
		{
			base.status = Status.SUCCESS;
		}
	}
}
