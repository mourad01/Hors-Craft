// DecompilerFi decompiler from Assembly-CSharp.dll class: TankNavigator
using UnityEngine;

public class TankNavigator : MobNavigator
{
	protected override void Update()
	{
		base.Update();
		Vector3 normalized = (destination - base.mob.transform.position).normalized;
		normalized.y = 0f;
		Quaternion rotation = Quaternion.RotateTowards(base.mob.transform.rotation, Quaternion.LookRotation(normalized, Vector3.up), 30f * Time.deltaTime);
		base.mob.transform.rotation = rotation;
	}

	protected override void UpdateJump()
	{
	}

	public override Vector3 GetDestination()
	{
		return destination;
	}
}
