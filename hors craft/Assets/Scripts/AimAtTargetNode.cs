// DecompilerFi decompiler from Assembly-CSharp.dll class: AimAtTargetNode
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class AimAtTargetNode : MobNode
{
	private float aimOffDegree = 3f;

	private PlayerController _playerController;

	public AimAtTargetNode(ShipEnemy mob, float aimOffDegree)
		: base(mob)
	{
		this.aimOffDegree = aimOffDegree;
	}

	public override void Update()
	{
		if (_playerController == null)
		{
			_playerController = CameraController.instance.Anchor.GetComponent<PlayerController>();
		}
		GameObject gameObject = _playerController.gameObject;
		Vector3 vector = gameObject.transform.position + Vector3.down;
		((ShipEnemy)base.mob).LootAtSideway(vector);
		Quaternion quaternion = Quaternion.LookRotation(vector - base.mob.transform.position, Vector3.up);
		quaternion.eulerAngles += new Vector3(0f, 90f, 0f);
		Vector3 eulerAngles = base.mob.transform.rotation.eulerAngles;
		float y = eulerAngles.y;
		Vector3 eulerAngles2 = quaternion.eulerAngles;
		float num = Mathf.Abs(y - eulerAngles2.y);
		base.status = ((!(num < aimOffDegree)) ? Status.RUNNING : Status.SUCCESS);
	}
}
