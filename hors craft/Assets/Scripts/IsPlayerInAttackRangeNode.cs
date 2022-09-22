// DecompilerFi decompiler from Assembly-CSharp.dll class: IsPlayerInAttackRangeNode
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class IsPlayerInAttackRangeNode : MobNode
{
	private float range;

	private const float CHECK_INTERVAL = 0.5f;

	private Transform mainCameraTransform;

	private float nextCheckTime;

	public IsPlayerInAttackRangeNode(Mob mob, float range)
		: base(mob)
	{
		this.range = range;
	}

	public override void Update()
	{
		if (mainCameraTransform == null)
		{
			mainCameraTransform = CameraController.instance.MainCamera.transform;
		}
		if (Time.time > nextCheckTime)
		{
			if ((base.mob.transform.position - mainCameraTransform.position).magnitude < range)
			{
				base.status = Status.SUCCESS;
			}
			else
			{
				base.status = Status.FAILURE;
			}
			nextCheckTime = Time.time + 0.5f;
		}
	}
}
