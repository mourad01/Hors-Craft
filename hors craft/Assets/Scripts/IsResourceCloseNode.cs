// DecompilerFi decompiler from Assembly-CSharp.dll class: IsResourceCloseNode
using Common.BehaviourTrees;
using UnityEngine;

public class IsResourceCloseNode : MobNode
{
	private const float CHECK_INTERVAL = 2f;

	private float closeDistance;

	private Vector3 resourcePosition;

	private float nextCheckTime;

	public IsResourceCloseNode(Mob mob)
		: base(mob)
	{
		PettableFriend component = mob.GetComponent<PettableFriend>();
		if (component != null)
		{
			if (component.spawnedSign != null)
			{
				resourcePosition = component.spawnedSign.transform.position;
			}
			else
			{
				resourcePosition = mob.transform.position;
			}
		}
		else
		{
			resourcePosition = mob.transform.position;
		}
	}

	public override void Update()
	{
		if (Time.time > nextCheckTime)
		{
			Vector3 a = base.mob.transform.position - resourcePosition;
			a.y = 0f;
			if (a.magnitude < 2.5f)
			{
				base.status = Status.SUCCESS;
				base.mob.GetComponent<PettableFriend>().shouldGoToResource = false;
				base.mob.transform.rotation = Quaternion.LookRotation(-a);
			}
			else
			{
				base.status = Status.FAILURE;
			}
			nextCheckTime = Time.time + 2f;
		}
	}
}
