// DecompilerFi decompiler from Assembly-CSharp.dll class: Ghost
using Common.BehaviourTrees;

public class Ghost : EnemyMob
{
	protected override float afterAttackCooldown => 1.5f;

	protected override float attackRange => 2.3f;

	protected override void Update()
	{
		base.Update();
		UpdateWalkingEffect();
	}

	public override void Die()
	{
		cooldownNode.SetCooldown(10f);
		base.Die();
	}

	protected override ParallelNode ConstructTopParallelNode()
	{
		ParallelNode parallelNode = base.ConstructTopParallelNode();
		SelectorNode selectorNode = new SelectorNode();
		cooldownNode = new CooldownNode();
		selectorNode.Add(new IsDeadNode(this, base.health));
		selectorNode.Add(ConstructAttackPlayer());
		parallelNode.Add(selectorNode);
		parallelNode.Add(ConstructRunToEnemy());
		return parallelNode;
	}

	protected override Node ConstructAttackPlayer()
	{
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(cooldownNode);
		sequenceNode.Add(new IsMobVisibleNode(this));
		sequenceNode.Add(new AttackEnemiesInAttackRangeNode(this, base.GetTargets));
		return sequenceNode;
	}

	protected override Node ConstructRunToEnemy()
	{
		LoopNode loopNode = new LoopNode();
		loopNode.Add(new SetDestinationNearTargetEnemy(this, base.GetTargets, 1f));
		loopNode.Add(new GoToDestinationNode(this, runSpeed));
		return loopNode;
	}
}
