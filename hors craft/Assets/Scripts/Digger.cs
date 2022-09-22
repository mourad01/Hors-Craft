// DecompilerFi decompiler from Assembly-CSharp.dll class: Digger
using Common.BehaviourTrees;
using UnityEngine;

public class Digger : EnemyMob
{
	public GameObject explosion;

	public bool explodeObstacles = true;

	public bool isSuicidal;

	private const float DIG_COOLDOWN = 2f;

	protected override float afterAttackCooldown => 3f;

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
		if (explodeObstacles)
		{
			selectorNode.Add(ConstructDig());
		}
		parallelNode.Add(selectorNode);
		parallelNode.Add(ConstructRunToEnemy());
		return parallelNode;
	}

	protected override Node ConstructAttackPlayer()
	{
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(cooldownNode);
		sequenceNode.Add(new AttackEnemiesInAttackRangeNode(this, base.GetTargets));
		sequenceNode.Add(new ExplodeNode(this, explosion.GetComponent<ParticleSystem>()));
		if (isSuicidal)
		{
			sequenceNode.Add(new DieNode(this));
		}
		return sequenceNode;
	}

	private Node ConstructDig()
	{
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(new IsObstacleAheadNode(this, 2f));
		sequenceNode.Add(new ExplodeNode(this, explosion.GetComponent<ParticleSystem>()));
		return sequenceNode;
	}
}
