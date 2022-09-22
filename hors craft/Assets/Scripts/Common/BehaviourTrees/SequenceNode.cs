// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.BehaviourTrees.SequenceNode
namespace Common.BehaviourTrees
{
	public class SequenceNode : CompositeNode
	{
		public SequenceNode(params Node[] nodes)
			: base(nodes)
		{
		}

		public override void Update()
		{
			for (int i = 0; i < children.Count; i++)
			{
				Node node = children[i];
				node.Update();
				if (node.status != 0)
				{
					base.status = node.status;
					return;
				}
			}
			base.status = Status.SUCCESS;
		}
	}
}
