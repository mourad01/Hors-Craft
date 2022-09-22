// DecompilerFi decompiler from Assembly-CSharp.dll class: SetDestinationNearTargetEnemy
using Common.BehaviourTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class SetDestinationNearTargetEnemy : MobNode
{
	private Func<List<GameObject>> getTargets;

	private float distance;

	public SetDestinationNearTargetEnemy(EnemyMob mob, Func<List<GameObject>> getTargetsFunction, float distance = 0f)
		: base(mob)
	{
		this.distance = distance;
		getTargets = getTargetsFunction;
	}

	public override void Update()
	{
		List<GameObject> list = getTargets();
		GameObject gameObject = (list != null && list.Count != 0) ? (from e in list
			let d = GetDistanceMultiplier(e) * Vector3.Distance(base.mob.transform.position, e.transform.position)
			orderby d
			select e).First() : PlayerGraphic.GetControlledPlayerInstance().gameObject;
		Vector3 position = gameObject.transform.position;
		Vector3 normalized = (base.mob.transform.position - position).normalized;
		base.mob.navigator.SetDestination(position + normalized * distance);
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
