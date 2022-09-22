// DecompilerFi decompiler from Assembly-CSharp.dll class: FlyingMobNavigator
using Common.Utils;
using Uniblocks;
using UnityEngine;

public class FlyingMobNavigator : MobNavigator
{
	public float maxFlyingSpeed = 10f;

	private const float SMOOTHED_ACCELERATION = 0.5f;

	private new const float ROTATION_SPEED = 120f;

	private const float FALLING_START_DISTANCE = 10f;

	private float waterHeight;

	private bool isDestinationSet;

	protected override void Start()
	{
		base.Start();
		waterHeight = Engine.TerrainGenerator.waterHeight;
		if (!isDestinationSet)
		{
			destination = base.mob.transform.position;
		}
	}

	protected override void UpdateMovement()
	{
		Vector3 vector = destination - base.mob.groundPosition;
		vector.y = 0f;
		vector.Normalize();
		if (!base.obstacleAhead)
		{
			Vector3 velocity = vector * speed;
			Vector3 velocity2 = base.mob.body.velocity;
			velocity.y = velocity2.y;
			base.mob.body.velocity = velocity;
		}
		if (base.mob.transform.forward != vector)
		{
			Quaternion to = Quaternion.LookRotation(vector);
			base.mob.body.MoveRotation(Quaternion.RotateTowards(base.mob.transform.rotation, to, Time.fixedDeltaTime * 120f));
		}
	}

	protected override void UpdateJump()
	{
		Vector3 vector = destination - base.mob.groundPosition;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		Vector3 velocity = base.mob.body.velocity;
		float num;
		if (magnitude < 10f)
		{
			num = 0f;
			float num2 = 1f - Easing.Ease(EaseType.InCubic, 0f, 1f, 1f - magnitude / 10f);
			velocity.x *= num2;
			velocity.z *= num2;
			goto IL_00ff;
		}
		if (!base.grounded && !base.obstacleAhead)
		{
			Vector3 position = base.mob.transform.position;
			if (!(position.y < waterHeight + 3f))
			{
				num = 0f;
				goto IL_00e9;
			}
		}
		num = maxFlyingSpeed;
		goto IL_00e9;
		IL_00ff:
		base.mob.body.velocity = velocity;
		return;
		IL_00e9:
		velocity.y = (velocity.y + num) * 0.5f;
		goto IL_00ff;
	}

	public override void SetDestination(Vector3 pos)
	{
		if (Physics.Raycast(pos + Vector3.up * 100f, Vector3.down, out RaycastHit hitInfo, 200f))
		{
			destination = hitInfo.point;
			Vector3 vector = destination - base.transform.position;
			vector.y = 0f;
			if (vector.magnitude >= 1.25f)
			{
				StartMovement();
			}
			isDestinationSet = true;
		}
	}
}
