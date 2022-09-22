// DecompilerFi decompiler from Assembly-CSharp.dll class: GhostNavigator
using UnityEngine;

public class GhostNavigator : MobNavigator
{
	protected override void UpdateJump()
	{
	}

	protected override void UpdateMovement()
	{
		Vector3 vector = destination - base.mob.transform.position;
		vector.Normalize();
		Vector3 velocity = vector * speed;
		base.mob.body.velocity = velocity;
		if (base.mob.transform.forward != vector)
		{
			Quaternion to = Quaternion.LookRotation(vector);
			base.mob.body.MoveRotation(Quaternion.RotateTowards(base.mob.transform.rotation, to, Time.fixedDeltaTime * 90f));
		}
		if ((destination - base.mob.transform.position).magnitude < 1.25f)
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			EndMovement();
		}
	}

	public override void SetDestination(Vector3 pos)
	{
		destination = pos;
		if ((destination - base.transform.position).magnitude >= 1.25f)
		{
			StartMovement();
		}
	}
}
