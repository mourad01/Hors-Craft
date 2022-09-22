// DecompilerFi decompiler from Assembly-CSharp.dll class: FearFallowPlayerLoopNode
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class FearFallowPlayerLoopNode : MobNode
{
	private float distanceFromPlayer;

	private float timeout;

	private float endTime;

	private float runSpeed;

	private float nextUpdate;

	private float interval;

	private bool isStarted;

	public FearFallowPlayerLoopNode(Mob mob, float distanceFromPlayer = 2f, float timeout = 10f, float runSpeed = 4f, float interval = 1f)
		: base(mob)
	{
		this.distanceFromPlayer = distanceFromPlayer;
		this.timeout = timeout;
		this.runSpeed = runSpeed;
		this.interval = interval;
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
			if (Time.time > nextUpdate)
			{
				SetNewDestination();
			}
		}
		else
		{
			base.status = Status.SUCCESS;
		}
	}

	private void SetNewDestination()
	{
		nextUpdate = Time.time + interval;
		Vector3 position = CameraController.instance.MainCamera.transform.position;
		Vector3 normalized = (base.mob.transform.position - position).normalized;
		base.mob.navigator.SetDestination(position + normalized * distanceFromPlayer);
	}
}
