// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.BehaviourTrees.ParallelNode
namespace Common.BehaviourTrees
{
	public class ParallelNode : CompositeNode
	{
		public ParallelNode(params Node[] nodes)
			: base(nodes)
		{
		}

		public override void Update()
		{
			bool flag = true;
			for (int i = 0; i < children.Count; i++)
			{
				Node node = children[i];
				node.Update();
				if (node.status != Status.RUNNING)
				{
					flag = false;
				}
			}
			base.status = ((!flag) ? Status.RUNNING : Status.SUCCESS);
		}
	}
}
