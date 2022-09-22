// DecompilerFi decompiler from Assembly-CSharp.dll class: ThrowGrenade
using Common.BehaviourTrees;

public class ThrowGrenade : MobNode
{
	public ThrowGrenade(ShootingEnemy mob)
		: base(mob.GetMob())
	{
	}

	public override void Update()
	{
		(base.mob as ShootingEnemy).ThrowGrenade();
		base.status = Status.SUCCESS;
	}
}
