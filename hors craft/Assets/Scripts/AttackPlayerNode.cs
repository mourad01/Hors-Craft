// DecompilerFi decompiler from Assembly-CSharp.dll class: AttackPlayerNode
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class AttackPlayerNode : MobNode
{
	private Camera mainCamera;

	public AttackPlayerNode(IFighting mob)
		: base(mob.GetMob())
	{
	}

	public override void Update()
	{
		if (mainCamera == null)
		{
			mainCamera = CameraController.instance.MainCamera;
		}
		(base.mob as IFighting).Attack(mainCamera.transform);
		base.status = Status.SUCCESS;
	}
}
