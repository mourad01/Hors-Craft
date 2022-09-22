// DecompilerFi decompiler from Assembly-CSharp.dll class: ShootPlayerNode
using com.ootii.Cameras;
using Common.BehaviourTrees;

public class ShootPlayerNode : MobNode
{
	private IFighting enemy;

	private PlayerGraphic _playerGraphic;

	public ShootPlayerNode(IFighting mob)
		: base(mob.GetMob())
	{
		enemy = mob;
		_playerGraphic = CameraController.instance.Anchor.GetComponent<PlayerGraphic>();
	}

	public override void Update()
	{
		if (_playerGraphic == null)
		{
			_playerGraphic = CameraController.instance.Anchor.GetComponent<PlayerGraphic>();
		}
		enemy.Attack(_playerGraphic.mainBody.transform);
		base.status = Status.SUCCESS;
	}
}
