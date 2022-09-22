// DecompilerFi decompiler from Assembly-CSharp.dll class: SetDestinationAwayFromPlayer
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class SetDestinationAwayFromPlayer : MobNode
{
	private float goDistance;

	private Camera mainCamera;

	private const float GO_AWAY_DISTANCE = 4f;

	private const float RANDOMIZATION_RADIUS = 2f;

	public SetDestinationAwayFromPlayer(Mob mob, float goDistance = 4f)
		: base(mob)
	{
		this.goDistance = goDistance;
	}

	public override void Update()
	{
		if (mainCamera == null)
		{
			mainCamera = CameraController.instance.MainCamera;
		}
		Vector3 vector = base.mob.groundPosition - mainCamera.transform.position;
		vector.y = 0f;
		Vector3 b = Random.insideUnitSphere * 2f;
		base.mob.navigator.SetDestination(vector.normalized * goDistance + b);
		base.status = Status.SUCCESS;
	}
}
