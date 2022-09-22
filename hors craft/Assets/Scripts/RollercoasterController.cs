// DecompilerFi decompiler from Assembly-CSharp.dll class: RollercoasterController
using Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RollercoasterController : MonoBehaviour
{
	[Serializable]
	public class RollercoasterSerializableNode
	{
		public Vector3 nodeLocation;

		public List<Vector3> neighbourLocations;

		public TrackType type;

		public RollercoasterSerializableNode(Vector3 nodeLocation, List<Vector3> neighbourLocations, TrackType type)
		{
			this.nodeLocation = nodeLocation;
			this.neighbourLocations = neighbourLocations;
			this.type = type;
		}
	}

	public class RollercoasterNode
	{
		public GameObject graphicInstance;

		public Vector3 nodeLocation;

		public Vector3 previousNodeLocation;

		public Vector3 nextNodeLocation;

		public HashSet<Vector3> neighbourLocations;

		public TrackType type;

		public bool safe;

		public Quaternion currentLookRotation;

		public Quaternion trackRotation;

		public Quaternion graphicsRotation;

		public Dictionary<Direction, RollercoasterThingOnTrack> thingsOnTrackScripts = new Dictionary<Direction, RollercoasterThingOnTrack>();

		public Dictionary<Direction, GameObject> thingsOnTrackAdditionalGraphics = new Dictionary<Direction, GameObject>();

		public RollercoasterNode(Vector3 nodeLocation, List<Vector3> neighbourLocations, TrackType type)
		{
			this.nodeLocation = nodeLocation;
			this.neighbourLocations = new HashSet<Vector3>(neighbourLocations);
			this.type = type;
		}
	}

	[Serializable]
	public class TrackConfig
	{
		public RollercoasterTrackSetup track;

		public float probability;

		public float probabilityChange;
	}

	[Serializable]
	public class TractTypeToPrefab
	{
		public TrackType type;

		public GameObject prefab;
	}

	public enum Direction
	{
		Default,
		Up,
		Down,
		Left,
		Right
	}

	public enum TrackType
	{
		Flat,
		Turn,
		Elevation
	}

	public List<RollercoasterSerializableNode> nodes = new List<RollercoasterSerializableNode>();

	public Vector3 startingNodeLocation;

	public Vector3 secondNodeLocation;

	public int startingSafeTracks = 5;

	public int aroundCornersSafeTracks = 3;

	public List<TrackConfig> tracksConfig = new List<TrackConfig>();

	public RollercoasterTrackSetup defaultTrackSetup;

	public Transform player;

	public Animator playerAnimator;

	public Transform cameraTransform;

	public RollercoasterEffects effects;

	private Dictionary<Vector3, RollercoasterNode> nodesDictionary = new Dictionary<Vector3, RollercoasterNode>();

	[HideInInspector]
	public Direction currentDirection;

	private Vector3 currentVectorDirection;

	private float move;

	private float currentNodeProgress;

	private float actionProgress;

	private int numOfCurrentTrack;

	private int numOfBuiltTracks;

	private const int spawnDistance = 30;

	public RollercoasterNode currentNode;

	public RollercoasterNode currentSpawnedNode;

	[HideInInspector]
	public float actionLength = 3f;

	public float speed = 5f;

	public float maxDownfallSpeedChange = 2f;

	private float downfallSpeedChange;

	private void SetupNodes()
	{
		foreach (RollercoasterSerializableNode node in nodes)
		{
			RollercoasterNode value = new RollercoasterNode(node.nodeLocation, node.neighbourLocations, node.type);
			nodesDictionary[node.nodeLocation] = value;
		}
		RollercoasterNode rollercoasterNode = nodesDictionary[startingNodeLocation];
		rollercoasterNode.safe = true;
		rollercoasterNode.nextNodeLocation = secondNodeLocation;
		rollercoasterNode = nodesDictionary[secondNodeLocation];
		rollercoasterNode.previousNodeLocation = startingNodeLocation;
		while (rollercoasterNode.nodeLocation != startingNodeLocation)
		{
			foreach (Vector3 neighbourLocation in rollercoasterNode.neighbourLocations)
			{
				if (neighbourLocation != rollercoasterNode.previousNodeLocation)
				{
					nodesDictionary[neighbourLocation].previousNodeLocation = rollercoasterNode.nodeLocation;
					rollercoasterNode.nextNodeLocation = neighbourLocation;
					rollercoasterNode = nodesDictionary[neighbourLocation];
					break;
				}
			}
		}
		rollercoasterNode = nodesDictionary[startingNodeLocation];
		bool flag = true;
		while (rollercoasterNode.nodeLocation != startingNodeLocation || flag)
		{
			flag = false;
			if (rollercoasterNode.type != nodesDictionary[rollercoasterNode.previousNodeLocation].type || rollercoasterNode.type == TrackType.Turn)
			{
				rollercoasterNode.safe = true;
				Vector3 previousNodeLocation = rollercoasterNode.previousNodeLocation;
				Vector3 nextNodeLocation = rollercoasterNode.nextNodeLocation;
				for (int i = 0; i < aroundCornersSafeTracks; i++)
				{
					nodesDictionary[previousNodeLocation].safe = true;
					previousNodeLocation = nodesDictionary[previousNodeLocation].previousNodeLocation;
					nodesDictionary[nextNodeLocation].safe = true;
					nextNodeLocation = nodesDictionary[nextNodeLocation].nextNodeLocation;
				}
			}
			Vector3 forward = rollercoasterNode.nextNodeLocation - rollercoasterNode.nodeLocation;
			Vector3 forward2 = nodesDictionary[rollercoasterNode.nextNodeLocation].nextNodeLocation - rollercoasterNode.nextNodeLocation;
			if (nodesDictionary[rollercoasterNode.nextNodeLocation].type == TrackType.Elevation && forward.y == 0f)
			{
				rollercoasterNode.currentLookRotation = Quaternion.LookRotation(forward2);
			}
			else if (nodesDictionary[rollercoasterNode.nextNodeLocation].type == TrackType.Flat && forward.y != 0f)
			{
				rollercoasterNode.currentLookRotation = Quaternion.LookRotation(forward2);
			}
			else
			{
				rollercoasterNode.currentLookRotation = Quaternion.LookRotation(forward);
			}
			rollercoasterNode.trackRotation = RotateTrack(rollercoasterNode);
			rollercoasterNode = nodesDictionary[rollercoasterNode.nextNodeLocation];
		}
	}

	private void Start()
	{
		TimeScaleHelper.value = 1f;
		SetupNodes();
		currentSpawnedNode = nodesDictionary[startingNodeLocation];
		currentNode = currentSpawnedNode;
		while (numOfBuiltTracks <= 30)
		{
			SetupNextTrack();
		}
		player.position = startingNodeLocation;
		player.rotation = currentNode.currentLookRotation;
		cameraTransform.rotation = currentNode.currentLookRotation;
		GetCurrentDirection();
	}

	private void Update()
	{
		CheckForDownFall();
		move = (speed + downfallSpeedChange) * Time.deltaTime;
		move = Mathf.Clamp01(move);
		if (currentDirection != 0)
		{
			actionProgress += move;
		}
		if (nodesDictionary[currentNode.nextNodeLocation].type == TrackType.Elevation)
		{
			Transform child = player.transform.GetChild(0);
			Vector3 localPosition = player.transform.GetChild(0).localPosition;
			Vector3 localPosition2 = player.transform.GetChild(0).localPosition;
			child.localPosition = localPosition.SetPositionY(Mathf.Lerp(localPosition2.y, 0f, 0.1f));
		}
		else
		{
			Transform child2 = player.transform.GetChild(0);
			Vector3 localPosition3 = player.transform.GetChild(0).localPosition;
			Vector3 localPosition4 = player.transform.GetChild(0).localPosition;
			child2.localPosition = localPosition3.SetPositionY(Mathf.Lerp(localPosition4.y, -0.5f, 0.1f));
		}
		if (player.rotation != currentNode.currentLookRotation)
		{
			player.rotation = Quaternion.Lerp(player.rotation, currentNode.currentLookRotation, 0.1f);
		}
		if (currentNodeProgress + move >= 1f)
		{
			player.position += (1f - currentNodeProgress) * currentVectorDirection;
			GoToNextNode();
			move -= 1f - currentNodeProgress;
			currentNodeProgress = 0f;
		}
		currentNodeProgress += move;
		player.position += move * currentVectorDirection;
		if (actionProgress > actionLength)
		{
			SetDirection(Direction.Default);
		}
		CheckForObjectHit();
	}

	private void LateUpdate()
	{
		cameraTransform.position = player.position;
		if (cameraTransform.rotation != currentNode.currentLookRotation)
		{
			cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, currentNode.currentLookRotation, 0.02f);
		}
	}

	private void CheckForDownFall()
	{
		if (currentVectorDirection.y < 0f)
		{
			downfallSpeedChange += maxDownfallSpeedChange * Time.deltaTime / 2f;
		}
		else
		{
			downfallSpeedChange -= maxDownfallSpeedChange * Time.deltaTime;
		}
		downfallSpeedChange = Mathf.Clamp(downfallSpeedChange, 0f, maxDownfallSpeedChange);
		effects.UpdateEffects(downfallSpeedChange / maxDownfallSpeedChange);
	}

	public void SetDirection(Direction direction)
	{
		if (currentDirection != 0)
		{
			playerAnimator.SetBool(currentDirection.ToString(), value: false);
		}
		if (direction != 0)
		{
			playerAnimator.SetBool(direction.ToString(), value: true);
		}
		actionProgress = 0f;
		currentDirection = direction;
	}

	private void SetupNextTrack()
	{
		if (startingSafeTracks > 0)
		{
			startingSafeTracks--;
			defaultTrackSetup.Setup(this);
			return;
		}
		if (!currentSpawnedNode.safe)
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			float num2 = 0f;
			foreach (TrackConfig item in tracksConfig)
			{
				num2 += item.probability + item.probabilityChange * (float)numOfBuiltTracks;
				if (num <= num2)
				{
					item.track.Setup(this);
					return;
				}
			}
		}
		defaultTrackSetup.Setup(this);
	}

	public void BuildNextNode()
	{
		currentSpawnedNode = nodesDictionary[currentSpawnedNode.nextNodeLocation];
		ClearNode(currentSpawnedNode);
		numOfBuiltTracks++;
	}

	private void ClearNode(RollercoasterNode node)
	{
		if (node.graphicInstance != null)
		{
			UnityEngine.Object.Destroy(node.graphicInstance);
		}
		foreach (GameObject value in node.thingsOnTrackAdditionalGraphics.Values)
		{
			UnityEngine.Object.Destroy(value);
		}
		node.thingsOnTrackAdditionalGraphics.Clear();
		node.thingsOnTrackScripts.Clear();
	}

	private void GoToNextNode()
	{
		currentNode = nodesDictionary[currentNode.nextNodeLocation];
		numOfCurrentTrack++;
		GetCurrentDirection();
		if (numOfBuiltTracks - numOfCurrentTrack < 30)
		{
			SetupNextTrack();
		}
	}

	private void GetCurrentDirection()
	{
		currentVectorDirection = currentNode.nextNodeLocation - currentNode.nodeLocation;
	}

	private void CheckForObjectHit()
	{
		if (currentNode.thingsOnTrackScripts.TryGetValue(currentDirection, out RollercoasterThingOnTrack value))
		{
			value.Hit(this);
			currentNode.thingsOnTrackScripts.Remove(currentDirection);
		}
		if (currentDirection == Direction.Default && currentNode.thingsOnTrackScripts.TryGetValue(Direction.Down, out value))
		{
			value.Hit(this);
			currentNode.thingsOnTrackScripts.Remove(Direction.Down);
		}
	}

	public Quaternion RotateTrack(RollercoasterNode node)
	{
		switch (node.type)
		{
		case TrackType.Flat:
			return Quaternion.LookRotation((node.nextNodeLocation - node.nodeLocation).SetPositionY(0f));
		case TrackType.Elevation:
		{
			Vector3 v = node.nextNodeLocation - node.previousNodeLocation;
			if (v.y < 0f)
			{
				return Quaternion.LookRotation(Vector3.down, v.SetPositionY(0f));
			}
			return Quaternion.LookRotation(v.SetPositionY(0f));
		}
		case TrackType.Turn:
		{
			Vector3 vector = node.nextNodeLocation - node.nodeLocation;
			Vector3 vector2 = node.nodeLocation - node.previousNodeLocation;
			if ((vector2.x < 0f && vector.z < 0f) || (vector2.z > 0f && vector.x > 0f))
			{
				return Quaternion.LookRotation(Vector3.forward);
			}
			if ((vector2.z > 0f && vector.x < 0f) || (vector2.x > 0f && vector.z < 0f))
			{
				return Quaternion.LookRotation(Vector3.right);
			}
			if ((vector2.x > 0f && vector.z > 0f) || (vector2.z < 0f && vector.x < 0f))
			{
				return Quaternion.LookRotation(Vector3.back);
			}
			if ((vector2.x < 0f && vector.z > 0f) || (vector2.z < 0f && vector.x > 0f))
			{
				return Quaternion.LookRotation(Vector3.left);
			}
			return Quaternion.identity;
		}
		default:
			return Quaternion.identity;
		}
	}

	public void IncreaseScore(int amount = 1)
	{
		UnityEngine.Debug.LogError("+1");
	}

	public void Die()
	{
		UnityEngine.Debug.LogError("You Died");
	}
}
