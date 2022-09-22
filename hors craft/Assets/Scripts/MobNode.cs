// DecompilerFi decompiler from Assembly-CSharp.dll class: MobNode
using Common.BehaviourTrees;

public abstract class MobNode : Node
{
	protected Mob mob
	{
		get;
		private set;
	}

	public MobNode(Mob mob)
	{
		this.mob = mob;
	}
}
