// DecompilerFi decompiler from Assembly-CSharp.dll class: RollercoasterTrackSetupCenterWithAround
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Rollercoaster/Setups/CenterWithThingsAround")]
public class RollercoasterTrackSetupCenterWithAround : RollercoasterTrackSetup
{
	public RollercoasterController.Direction direction;

	public RollercoasterTrackSetupDefault defaultTrack;

	public List<RollercoasterController.TractTypeToPrefab> prefabs;

	public RollercoasterThingOnTrack aroundObject;

	public RollercoasterThingOnTrack centerObject;

	private Dictionary<RollercoasterController.TrackType, GameObject> trackPrefabsDictionary = new Dictionary<RollercoasterController.TrackType, GameObject>();

	private void SetupDictionary()
	{
		foreach (RollercoasterController.TractTypeToPrefab prefab in prefabs)
		{
			trackPrefabsDictionary[prefab.type] = prefab.prefab;
		}
	}

	public override void Setup(RollercoasterController controller)
	{
		if (trackPrefabsDictionary.IsNullOrEmpty())
		{
			SetupDictionary();
		}
		float actionLength = controller.actionLength;
		for (int i = 0; (float)i < actionLength; i++)
		{
			RollercoasterController.RollercoasterNode currentSpawnedNode = controller.currentSpawnedNode;
			if (i == (int)(actionLength / 2f))
			{
				currentSpawnedNode.graphicInstance = Object.Instantiate(trackPrefabsDictionary[currentSpawnedNode.type]);
				currentSpawnedNode.graphicInstance.transform.position = currentSpawnedNode.nodeLocation;
				currentSpawnedNode.graphicInstance.transform.rotation = controller.RotateTrack(currentSpawnedNode);
				SetupAroundObject(direction, currentSpawnedNode);
				centerObject.Setup(direction, currentSpawnedNode, Vector3.zero);
				controller.BuildNextNode();
			}
			else
			{
				defaultTrack.Setup(controller);
				SetupAroundObject(direction, currentSpawnedNode);
			}
		}
	}

	private void SetupAroundObject(RollercoasterController.Direction direction, RollercoasterController.RollercoasterNode currentNode)
	{
		switch (direction)
		{
		case RollercoasterController.Direction.Left:
			aroundObject.Setup(RollercoasterController.Direction.Right, currentNode, Vector3.zero);
			break;
		case RollercoasterController.Direction.Right:
			aroundObject.Setup(RollercoasterController.Direction.Left, currentNode, Vector3.zero);
			break;
		case RollercoasterController.Direction.Up:
			aroundObject.Setup(RollercoasterController.Direction.Down, currentNode, Vector3.zero);
			break;
		case RollercoasterController.Direction.Down:
			aroundObject.Setup(RollercoasterController.Direction.Up, currentNode, Vector3.zero);
			break;
		}
	}
}
