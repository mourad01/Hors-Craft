// DecompilerFi decompiler from Assembly-CSharp.dll class: RollercoasterCoin
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Rollercoaster/Things/Coin")]
public class RollercoasterCoin : RollercoasterThingOnTrack
{
	public GameObject prefab;

	public override void Hit(RollercoasterController controller)
	{
		UnityEngine.Object.Destroy(controller.currentNode.thingsOnTrackAdditionalGraphics[controller.currentDirection]);
		controller.IncreaseScore();
	}

	public override void Setup(RollercoasterController.Direction direction, RollercoasterController.RollercoasterNode currentNode, Vector3 offset)
	{
		currentNode.thingsOnTrackScripts[direction] = this;
		GameObject gameObject = Object.Instantiate(prefab);
		Vector3 vector = (currentNode.nextNodeLocation - currentNode.nodeLocation).SetPositionY(0f);
		Quaternion rotation = Quaternion.identity;
		if (currentNode.type == RollercoasterController.TrackType.Elevation)
		{
			Vector3 vector2 = currentNode.nextNodeLocation - currentNode.previousNodeLocation;
			rotation = ((!(vector2.y > 0f)) ? Quaternion.LookRotation((currentNode.nextNodeLocation - currentNode.nodeLocation).SetPositionY(-1f)) : Quaternion.LookRotation((currentNode.nextNodeLocation - currentNode.nodeLocation).SetPositionY(1f)));
		}
		else if (currentNode.type == RollercoasterController.TrackType.Flat)
		{
			rotation = Quaternion.LookRotation((currentNode.nextNodeLocation - currentNode.nodeLocation).SetPositionY(0f));
		}
		switch (direction)
		{
		case RollercoasterController.Direction.Left:
			gameObject.transform.position = currentNode.graphicInstance.transform.position + vector.Rotate90left() / 2f;
			gameObject.transform.rotation = rotation;
			break;
		case RollercoasterController.Direction.Right:
			gameObject.transform.position = currentNode.graphicInstance.transform.position + vector.Rotate90right() / 2f;
			gameObject.transform.rotation = rotation;
			break;
		case RollercoasterController.Direction.Up:
			if (currentNode.type == RollercoasterController.TrackType.Elevation)
			{
				Vector3 vector3 = currentNode.nextNodeLocation - currentNode.previousNodeLocation;
				if (vector3.y > 0f)
				{
					gameObject.transform.position = currentNode.graphicInstance.transform.position + Vector3.up - vector;
				}
				else
				{
					gameObject.transform.position = currentNode.graphicInstance.transform.position + Vector3.up + vector;
				}
			}
			else
			{
				gameObject.transform.position = currentNode.graphicInstance.transform.position + Vector3.up;
			}
			gameObject.transform.rotation = rotation;
			break;
		default:
			gameObject.transform.position = currentNode.graphicInstance.transform.position;
			gameObject.transform.rotation = rotation;
			break;
		}
		currentNode.thingsOnTrackAdditionalGraphics[direction] = gameObject;
	}
}
