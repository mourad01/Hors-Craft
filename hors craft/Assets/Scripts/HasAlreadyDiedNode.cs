// DecompilerFi decompiler from Assembly-CSharp.dll class: HasAlreadyDiedNode
using Common.BehaviourTrees;

public class HasAlreadyDiedNode : MobNode
{
	private bool hasDied;

	private const float CHECK_INTERVAL = 0.5f;

	private float nextCheckTime;

	public HasAlreadyDiedNode(Mob mob)
		: base(mob)
	{
		hasDied = false;
	}

	public override void Update()
	{
		if (!hasDied)
		{
			hasDied = true;
			base.status = Status.FAILURE;
		}
		else
		{
			base.status = Status.SUCCESS;
		}
	}
}
