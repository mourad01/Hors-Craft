// DecompilerFi decompiler from Assembly-CSharp.dll class: Zombie
using Common.BehaviourTrees;

public class Zombie : EnemyMob
{
	public bool isNiceToPlayer;

	protected override float afterAttackCooldown => 2.5f;

	protected override float attackRange => 2.5f;

	protected override void Update()
	{
		base.Update();
		UpdateWalkingEffect();
	}

	public override void Die()
	{
		cooldownNode.SetCooldown(10f);
		DropAmmo();
		base.Die();
		MonoBehaviourSingleton<ProgressCounter>.get.Increment("ZombieKill");
	}

	private void DropAmmo()
	{
		DropAmmo component = GetComponent<DropAmmo>();
		if (component != null)
		{
			component.Drop(base.transform.position);
		}
	}

	protected override ParallelNode ConstructTopParallelNode()
	{
		ParallelNode parallelNode = base.ConstructTopParallelNode();
		SelectorNode selectorNode = new SelectorNode();
		cooldownNode = new CooldownNode();
		selectorNode.Add(new IsDeadNode(this, base.health));
		if (!isNiceToPlayer)
		{
			selectorNode.Add(ConstructAttackPlayer());
		}
		parallelNode.Add(selectorNode);
		if (!isNiceToPlayer)
		{
			parallelNode.Add(ConstructRunToEnemy());
		}
		return parallelNode;
	}
}
