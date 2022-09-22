// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.BehaviourTrees.SelectorNode
namespace Common.BehaviourTrees
{
	public class SelectorNode : CompositeNode
	{
		public SelectorNode(params Node[] nodes)
			: base(nodes)
		{
		}

		public override void Update()
		{
			for (int i = 0; i < children.Count; i++)
			{
				Node node = children[i];
				node.Update();
				if (node.status != Status.FAILURE)
				{
					base.status = node.status;
					return;
				}
			}
			base.status = Status.FAILURE;
		}
	}
}
