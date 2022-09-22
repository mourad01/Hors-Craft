// DecompilerFi decompiler from Assembly-CSharp.dll class: SpiderNavigator
using UnityEngine;

public class SpiderNavigator : MobNavigator
{
	protected override void UpdateJump()
	{
		if (base.obstacleAhead && !(Time.time < lastJumpTime + 0.5f))
		{
			Rigidbody body = base.mob.body;
			Vector3 velocity = base.mob.body.velocity;
			float x = velocity.x;
			float jumpPower = base.jumpPower;
			Vector3 velocity2 = base.mob.body.velocity;
			body.velocity = new Vector3(x, jumpPower, velocity2.z);
			lastJumpTime = Time.time;
		}
	}
}
