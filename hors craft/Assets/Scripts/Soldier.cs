// DecompilerFi decompiler from Assembly-CSharp.dll class: Soldier
using Common.BehaviourTrees;
using UnityEngine;

public class Soldier : ShootingEnemy
{
	public override bool IsEnemy()
	{
		return false;
	}

	protected override ParallelNode ConstructTopParallelNode()
	{
		ParallelNode parallelNode = new ParallelNode();
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(ConstructDieBehaviour());
		selectorNode.Add(ConstructShootEnemies());
		selectorNode.Add(ConstructFollowPlayer());
		parallelNode.Add(selectorNode);
		return parallelNode;
	}

	protected override bool EnemySpottedCondition(GameObject c)
	{
		return c != null && c.GetComponentInParent<ShootingEnemy>() != null && c.GetComponentInParent<Soldier>() == null;
	}

	private Node ConstructFollowPlayer()
	{
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(new IsPlayerCloseNode(this, 5f));
		SequenceNode sequenceNode = new SequenceNode();
		selectorNode.Add(sequenceNode);
		sequenceNode.Add(new WaitNode(this, 1f, 2f));
		sequenceNode.Add(new SetDestinationNearPlayer(this, 4f));
		sequenceNode.Add(new GoToDestinationNode(this, runSpeed));
		return selectorNode;
	}
}
