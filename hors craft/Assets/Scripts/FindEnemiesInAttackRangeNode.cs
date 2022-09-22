// DecompilerFi decompiler from Assembly-CSharp.dll class: FindEnemiesInAttackRangeNode
using Common.BehaviourTrees;
using System;
using System.Linq;
using UnityEngine;

public class FindEnemiesInAttackRangeNode : MobNode
{
	private Func<GameObject, bool> enemyCondition;

	private Func<GameObject[]> getSurroundingObjects;

	private const float CHECK_INTERVAL = 0.5f;

	private float nextCheckTime;

	public Transform[] enemies
	{
		get;
		private set;
	}

	public FindEnemiesInAttackRangeNode(Mob mob, Func<GameObject, bool> enemyCondition, Func<GameObject[]> getObjects)
		: base(mob)
	{
		this.enemyCondition = enemyCondition;
		getSurroundingObjects = getObjects;
	}

	public override void Update()
	{
		if (Time.time > nextCheckTime)
		{
			GameObject[] source = getSurroundingObjects();
			enemies = (from c in source
				where enemyCondition(c)
				select c.transform).ToArray();
			if (enemies == null || enemies.Length == 0)
			{
				base.status = Status.FAILURE;
				return;
			}
			base.status = Status.SUCCESS;
			nextCheckTime = Time.time + 0.5f;
		}
	}
}
