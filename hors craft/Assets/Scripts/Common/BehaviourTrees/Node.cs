// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.BehaviourTrees.Node
namespace Common.BehaviourTrees
{
	public abstract class Node
	{
		public Status status
		{
			get;
			protected set;
		}

		public Node()
		{
			status = Status.SUCCESS;
		}

		public abstract void Update();
	}
}
