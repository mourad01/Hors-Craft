// DecompilerFi decompiler from Assembly-CSharp.dll class: ShouldAvoidObstacle
using Common.BehaviourTrees;

public class ShouldAvoidObstacle : MobNode
{
	private IsObstacleAhead isObstacleAheadNode;

	private CanGoToPlayer canGoToPlayerNode;

	private bool isAvoidanceOn;

	public ShouldAvoidObstacle(Mob mob, float r = 5f)
		: base(mob)
	{
		isObstacleAheadNode = new IsObstacleAhead(mob, r);
		canGoToPlayerNode = new CanGoToPlayer(mob, r);
	}

	public override void Update()
	{
		if (!isAvoidanceOn)
		{
			isObstacleAheadNode.Update();
			base.status = isObstacleAheadNode.status;
			if (base.status == Status.SUCCESS)
			{
				isAvoidanceOn = true;
			}
		}
		else
		{
			canGoToPlayerNode.Update();
			base.status = ((canGoToPlayerNode.status == Status.SUCCESS) ? Status.FAILURE : Status.SUCCESS);
			if (base.status == Status.FAILURE)
			{
				isAvoidanceOn = false;
			}
		}
	}
}
