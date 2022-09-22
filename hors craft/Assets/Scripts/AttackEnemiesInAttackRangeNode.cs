// DecompilerFi decompiler from Assembly-CSharp.dll class: AttackEnemiesInAttackRangeNode
using Common.BehaviourTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class AttackEnemiesInAttackRangeNode : MobNode
{
	private Func<List<GameObject>> getTargets;

	private Func<GameObject, float> distanceMultiplier;

	public AttackEnemiesInAttackRangeNode(IFighting mob, Func<List<GameObject>> getTargetsFunction, Func<GameObject, float> distanceFunction = null)
		: base(mob.GetMob())
	{
		getTargets = getTargetsFunction;
		distanceMultiplier = distanceFunction;
		if (distanceMultiplier == null)
		{
			distanceMultiplier = GetDistanceMultiplier;
		}
	}

	public override void Update()
	{
		List<GameObject> list = getTargets();
		if (list == null || list.Count == 0)
		{
			base.status = Status.FAILURE;
			return;
		}
		GameObject gameObject = (from e in list
			let d = distanceMultiplier(e) * Vector3.Distance(base.mob.transform.position, e.transform.position)
			orderby d
			select e).First();
		(base.mob as IFighting).Attack(gameObject.transform);
		base.status = Status.SUCCESS;
	}

	private float GetDistanceMultiplier(GameObject enemy)
	{
		if (enemy.GetComponentInParent<PlayerMovement>() != null)
		{
			return 2f;
		}
		if (enemy.GetComponentInParent<Soldier>() != null)
		{
			return 3f;
		}
		return 1f;
	}
}
