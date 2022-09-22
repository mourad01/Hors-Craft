// DecompilerFi decompiler from Assembly-CSharp.dll class: Spider
using Common.BehaviourTrees;

public class Spider : EnemyMob
{
	protected override float afterAttackCooldown => 1.5f;

	protected override float attackRange => 2f;

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
}
