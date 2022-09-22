// DecompilerFi decompiler from Assembly-CSharp.dll class: RollercoasterTrackSetupDefault
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Rollercoaster/Setups/Default")]
public class RollercoasterTrackSetupDefault : RollercoasterTrackSetup
{
	public List<RollercoasterController.TractTypeToPrefab> prefabs;

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
		RollercoasterController.RollercoasterNode currentSpawnedNode = controller.currentSpawnedNode;
		currentSpawnedNode.graphicInstance = Object.Instantiate(trackPrefabsDictionary[currentSpawnedNode.type]);
		currentSpawnedNode.graphicInstance.transform.position = currentSpawnedNode.nodeLocation;
		currentSpawnedNode.graphicInstance.transform.rotation = controller.RotateTrack(currentSpawnedNode);
		controller.BuildNextNode();
	}
}
