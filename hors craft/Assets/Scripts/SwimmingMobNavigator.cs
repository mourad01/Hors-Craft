// DecompilerFi decompiler from Assembly-CSharp.dll class: SwimmingMobNavigator
using Uniblocks;
using UnityEngine;

public class SwimmingMobNavigator : MobNavigator
{
	protected const float MAX_DISTANCE = 10f;

	protected const int DESTINATION_TRIES_LIMIT = 5;

	protected new const float ROTATION_SPEED = 120f;

	private Vector3 lastWaterKnown;

	private const float CHECK_FREQUENCY = 10f;

	private float nextCheckTime;

	protected override void Start()
	{
		GetComponent<Rigidbody>().useGravity = false;
		SetDestination(base.mob.groundPosition + 5f * Vector3.down);
		base.transform.position = base.transform.position + 3f * Vector3.down;
	}

	public override void SetDestination(Vector3 pos)
	{
		int num = 0;
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(pos);
		bool flag = voxelInfo != null && voxelInfo.GetVoxel() == Engine.usefulIDs.waterBlockID;
		float num2 = Vector3.Distance(base.mob.groundPosition, pos);
		Vector3 vector = pos;
		while (!flag && num < 50)
		{
			int num3 = 0;
			while (!flag && num3 < 5)
			{
				vector = Random.onUnitSphere * num2 + base.mob.groundPosition;
				voxelInfo = Engine.PositionToVoxelInfo(vector);
				flag = (voxelInfo != null && voxelInfo.GetVoxel() == 12);
				num++;
				num3++;
			}
			num2 *= 0.9f;
		}
		if (flag)
		{
			destination = vector;
			StartMovement();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (Time.time > nextCheckTime)
		{
			ChunkData chunkData = Engine.PositionToChunkData(base.mob.groundPosition);
			if (chunkData == null || !chunkData.VoxelsDone)
			{
				base.mob.Despawn();
			}
			nextCheckTime = Time.time + 10f;
		}
	}

	protected override void UpdateMovement()
	{
		if (base.mob == null)
		{
			UnityEngine.Debug.LogError("MOB NIE MA MOBA KURWA");
			return;
		}
		if (base.mob.animator == null)
		{
			UnityEngine.Debug.LogError("MOB NIE MA ANIMATORA KURWA");
			return;
		}
		base.mob.animator.SetBool("walking", value: true);
		Vector3 vector = destination - base.mob.groundPosition;
		vector.Normalize();
		if (!base.obstacleAhead)
		{
			Vector3 velocity = vector * speed;
			base.mob.body.velocity = velocity;
		}
		if (base.mob.transform.forward != vector)
		{
			Quaternion to = Quaternion.LookRotation(vector);
			base.mob.body.MoveRotation(Quaternion.RotateTowards(base.mob.transform.rotation, to, Time.fixedDeltaTime * 120f));
		}
	}

	protected override void EndMovement()
	{
		base.EndMovement();
		base.mob.body.velocity = Vector3.zero;
	}

	protected override void UpdateJump()
	{
	}
}
