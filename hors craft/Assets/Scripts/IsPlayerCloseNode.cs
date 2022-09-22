// DecompilerFi decompiler from Assembly-CSharp.dll class: IsPlayerCloseNode
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class IsPlayerCloseNode : MobNode
{
	private const float PLAYER_IS_CLOSE_DISTANCE = 10f;

	private const float CHECK_INTERVAL = 2f;

	private float closeDistance;

	private Transform mainCameraTransform;

	private float nextCheckTime;

	public IsPlayerCloseNode(Mob mob, float distance = 10f)
		: base(mob)
	{
		closeDistance = distance;
	}

	public override void Update()
	{
		if (mainCameraTransform == null)
		{
			mainCameraTransform = CameraController.instance.MainCamera.transform;
		}
		if (Time.time > nextCheckTime)
		{
			Vector3 vector = base.mob.transform.position - mainCameraTransform.position;
			vector.y = 0f;
			if (vector.magnitude < closeDistance)
			{
				base.status = Status.SUCCESS;
			}
			else
			{
				base.status = Status.FAILURE;
			}
			nextCheckTime = Time.time + 2f;
		}
	}
}
