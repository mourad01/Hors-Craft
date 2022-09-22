// DecompilerFi decompiler from Assembly-CSharp.dll class: GoToResourceDestinationNode
using Common.BehaviourTrees;

public class GoToResourceDestinationNode : MobNode
{
	private float speed = 1f;

	public GoToResourceDestinationNode(Mob mob, float speed)
		: base(mob)
	{
		this.speed = speed;
	}

	public override void Update()
	{
		base.mob.navigator.speed = speed;
		base.status = Status.SUCCESS;
	}
}
