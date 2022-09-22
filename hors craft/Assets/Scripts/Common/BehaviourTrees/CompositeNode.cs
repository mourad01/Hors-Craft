// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.BehaviourTrees.CompositeNode
using System.Collections.Generic;

namespace Common.BehaviourTrees
{
	public abstract class CompositeNode : Node
	{
		protected List<Node> children = new List<Node>();

		public CompositeNode(params Node[] nodes)
		{
			foreach (Node item in nodes)
			{
				children.Add(item);
			}
		}

		public void Add(params Node[] nodes)
		{
			foreach (Node item in nodes)
			{
				children.Add(item);
			}
		}
	}
}
