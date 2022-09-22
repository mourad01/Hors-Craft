// DecompilerFi decompiler from Assembly-CSharp.dll class: StandInPlaceNode
using Common.BehaviourTrees;

public class StandInPlaceNode : MobNode
{
	private const float MAX_DESTINATION_DISTANCE = 10f;

	public StandInPlaceNode(Mob mob)
		: base(mob)
	{
	}

	public override void Update()
	{
		base.mob.navigator.enabled = false;
		base.mob.body.freezeRotation = true;
		base.status = Status.SUCCESS;
	}
}
