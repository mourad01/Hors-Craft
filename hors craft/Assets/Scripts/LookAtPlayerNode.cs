// DecompilerFi decompiler from Assembly-CSharp.dll class: LookAtPlayerNode
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class LookAtPlayerNode : MobNode
{
	private Transform cameraTransform;

	public LookAtPlayerNode(Mob mob)
		: base(mob)
	{
	}

	public override void Update()
	{
		if (cameraTransform == null)
		{
			cameraTransform = CameraController.instance.MainCamera.transform;
		}
		base.mob.LookAt(cameraTransform.position + Vector3.down);
		base.status = Status.SUCCESS;
	}
}
