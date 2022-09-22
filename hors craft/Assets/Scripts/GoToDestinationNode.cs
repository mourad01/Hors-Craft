// DecompilerFi decompiler from Assembly-CSharp.dll class: GoToDestinationNode
using Common.BehaviourTrees;

public class GoToDestinationNode : MobNode
{
	private float speed = 1f;

	private bool waitToEndMoving = true;

	public GoToDestinationNode(Mob mob, float speed, bool waitToEndMoving = true)
		: base(mob)
	{
		this.speed = speed;
		this.waitToEndMoving = waitToEndMoving;
	}

	public override void Update()
	{
		base.mob.navigator.speed = speed;
		if (base.mob.navigator.moving && waitToEndMoving)
		{
			base.status = Status.RUNNING;
		}
		else
		{
			base.status = Status.SUCCESS;
		}
	}
}
