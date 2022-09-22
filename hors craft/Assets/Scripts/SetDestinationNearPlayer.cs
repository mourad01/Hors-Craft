// DecompilerFi decompiler from Assembly-CSharp.dll class: SetDestinationNearPlayer
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class SetDestinationNearPlayer : MobNode
{
	private Camera mainCamera;

	private float distanceFromPlayer;

	public SetDestinationNearPlayer(Mob mob, float distanceFromPlayer = 0f)
		: base(mob)
	{
		this.distanceFromPlayer = distanceFromPlayer;
	}

	public override void Update()
	{
		if (mainCamera == null)
		{
			mainCamera = CameraController.instance.MainCamera;
		}
		Vector3 position = mainCamera.transform.position;
		Vector3 normalized = (base.mob.transform.position - position).normalized;
		base.mob.navigator.SetDestination(position + normalized * distanceFromPlayer);
		base.status = Status.SUCCESS;
	}
}
