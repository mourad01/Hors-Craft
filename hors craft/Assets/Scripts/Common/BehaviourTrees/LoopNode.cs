// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.BehaviourTrees.LoopNode
namespace Common.BehaviourTrees
{
	public class LoopNode : CompositeNode
	{
		private int currentNodeIndex;

		public LoopNode(params Node[] nodes)
			: base(nodes)
		{
			base.status = Status.RUNNING;
		}

		public override void Update()
		{
			if (currentNodeIndex == children.Count)
			{
				currentNodeIndex = 0;
			}
			Node node = children[currentNodeIndex];
			node.Update();
			if (node.status != Status.RUNNING)
			{
				currentNodeIndex++;
			}
		}
	}
}
