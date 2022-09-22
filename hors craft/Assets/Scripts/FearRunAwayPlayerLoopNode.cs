// DecompilerFi decompiler from Assembly-CSharp.dll class: FearRunAwayPlayerLoopNode
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class FearRunAwayPlayerLoopNode : MobNode
{
	private float distanceFromPlayer;

	private float timeout;

	private float endTime;

	private float runSpeed;

	private bool isStarted;

	public FearRunAwayPlayerLoopNode(Mob mob, float distanceFromPlayer = 10f, float timeout = 5f, float runSpeed = 4f)
		: base(mob)
	{
		this.distanceFromPlayer = distanceFromPlayer;
		this.timeout = timeout;
		this.runSpeed = runSpeed;
		isStarted = false;
	}

	public override void Update()
	{
		if (!isStarted)
		{
			base.status = Status.RUNNING;
			SetNewDestination();
			base.mob.navigator.speed = runSpeed;
			endTime = Time.time + timeout;
			isStarted = true;
		}
		if (Time.time < endTime)
		{
			base.status = Status.RUNNING;
		}
		else
		{
			base.status = Status.SUCCESS;
		}
	}

	private void SetNewDestination()
	{
		Vector3 position = CameraController.instance.MainCamera.transform.position;
		Vector3 normalized = (base.mob.transform.position - position).normalized;
		normalized.y = 0f;
		base.mob.navigator.SetDestination(position + normalized * distanceFromPlayer);
	}
}
