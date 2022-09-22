// DecompilerFi decompiler from Assembly-CSharp.dll class: RollercoasterObstacle
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Rollercoaster/Things/Obstacle")]
public class RollercoasterObstacle : RollercoasterThingOnTrack
{
	public override void Hit(RollercoasterController controller)
	{
		controller.Die();
	}

	public override void Setup(RollercoasterController.Direction direction, RollercoasterController.RollercoasterNode currentNode, Vector3 offset)
	{
		switch (direction)
		{
		case RollercoasterController.Direction.Left:
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Left] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Up] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Down] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Default] = this;
			break;
		case RollercoasterController.Direction.Right:
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Right] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Up] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Down] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Default] = this;
			break;
		case RollercoasterController.Direction.Up:
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Right] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Left] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Up] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Default] = this;
			break;
		case RollercoasterController.Direction.Down:
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Right] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Left] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Down] = this;
			currentNode.thingsOnTrackScripts[RollercoasterController.Direction.Default] = this;
			break;
		}
	}
}
