// DecompilerFi decompiler from Assembly-CSharp.dll class: ShipMobNavigator
using Uniblocks;
using UnityEngine;

public class ShipMobNavigator : SwimmingMobNavigator
{
	public override void SetDestination(Vector3 pos)
	{
		Vector3 groundPosition = base.mob.groundPosition;
		destination = pos.WithY(groundPosition.y);
		StartMovement();
	}

	protected override void UpdateMovement()
	{
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
		base.transform.position.SetPositionY(Engine.TerrainGenerator.waterHeight);
	}
}
