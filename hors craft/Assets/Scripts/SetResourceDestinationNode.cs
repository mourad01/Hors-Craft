// DecompilerFi decompiler from Assembly-CSharp.dll class: SetResourceDestinationNode
using Common.BehaviourTrees;
using UnityEngine;

public class SetResourceDestinationNode : MobNode
{
	private const float CHECK_INTERVAL = 2f;

	private Vector3 checkPosition;

	private float closeDistance;

	private PettableFriend pettable;

	private Vector3 lastSetPosition;

	public SetResourceDestinationNode(Mob mob)
		: base(mob)
	{
		pettable = mob.GetComponent<PettableFriend>();
		if (pettable != null)
		{
			if (pettable.spawnedSign != null)
			{
				checkPosition = pettable.spawnedSign.transform.position;
			}
			else
			{
				checkPosition = mob.transform.position;
			}
		}
		else
		{
			checkPosition = mob.transform.position;
		}
	}

	public override void Update()
	{
		if (pettable.shouldGoToResource)
		{
			if (lastSetPosition.Equals(checkPosition))
			{
				base.status = Status.SUCCESS;
				return;
			}
			base.mob.navigator.SetDestination(checkPosition);
			lastSetPosition = checkPosition;
			base.status = Status.FAILURE;
		}
		else
		{
			base.status = Status.FAILURE;
		}
	}
}
