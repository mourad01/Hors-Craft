// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.RoadController
using System.Collections.Generic;
using UnityEngine;

namespace DragMinigame
{
	public class RoadController : MonoBehaviour
	{
		[SerializeField]
		private DragRacingRoad roadPartPrefab;

		[SerializeField]
		private DragRacingRoad finishPartPrefab;

		[SerializeField]
		private DragRacingRoad startPartPrefab;

		[SerializeField]
		private Transform playerCar;

		[SerializeField]
		private float roadPartLength;

		[SerializeField]
		private int initialRoadparts;

		[SerializeField]
		private float despawnDistance;

		[SerializeField]
		private Vector3 startRoadPartPosition;

		[SerializeField]
		private Transform roadParent;

		private DragRacingRoad startRoadInstance;

		private Transform lastRoadPart;

		private Queue<DragRacingRoad> roadPool = new Queue<DragRacingRoad>();

		private Dictionary<int, DragRacingRoad> spawnedRoads = new Dictionary<int, DragRacingRoad>();

		private float distanceToSpawn;

		private bool isFlagNeeded;

		private bool spawnRoad;

		private int roadsToSpawnAmount;

		private int spawnedRoadsCount;

		public void Init()
		{
			startRoadInstance = UnityEngine.Object.Instantiate(startPartPrefab, startRoadPartPosition, Quaternion.identity);
			startRoadInstance.transform.parent = roadParent;
			spawnRoad = true;
			for (int i = 0; (float)i < (float)initialRoadparts + despawnDistance; i++)
			{
				DragRacingRoad dragRacingRoad = UnityEngine.Object.Instantiate(roadPartPrefab, new Vector3(0f, 200f, 0f), Quaternion.identity);
				dragRacingRoad.transform.parent = roadParent;
				roadPool.Enqueue(dragRacingRoad);
			}
			Vector3 position = playerCar.transform.position;
			distanceToSpawn = position.z + roadPartLength;
			for (int j = 0; j < initialRoadparts; j++)
			{
				SpawnNewRoad();
			}
		}

		private void Update()
		{
			if (spawnRoad)
			{
				Vector3 position = playerCar.position;
				if (position.z >= distanceToSpawn)
				{
					SpawnNewRoad();
					distanceToSpawn += roadPartLength;
				}
			}
		}

		private void SpawnNewRoad()
		{
			spawnedRoadsCount++;
			if (lastRoadPart == null)
			{
				InstantiateFirstPart();
				return;
			}
			HandleDespawning();
			HandleSpawning();
		}

		private void HandleSpawning()
		{
			Vector3 position = lastRoadPart.position;
			float x = position.x;
			Vector3 position2 = lastRoadPart.position;
			float y = position2.y;
			Vector3 position3 = lastRoadPart.position;
			Vector3 position4 = new Vector3(x, y, position3.z + roadPartLength);
			DragRacingRoad dragRacingRoad = null;
			if (spawnedRoadsCount >= roadsToSpawnAmount)
			{
				isFlagNeeded = true;
				spawnRoad = false;
			}
			if (!isFlagNeeded)
			{
				dragRacingRoad = roadPool.Dequeue();
			}
			else
			{
				dragRacingRoad = UnityEngine.Object.Instantiate(finishPartPrefab, roadParent);
				position4 = lastRoadPart.transform.position + finishPartPrefab.transform.position;
			}
			dragRacingRoad.transform.position = position4;
			lastRoadPart = dragRacingRoad.transform;
			Dictionary<int, DragRacingRoad> dictionary = spawnedRoads;
			Vector3 position5 = lastRoadPart.position;
			dictionary.Add(Mathf.FloorToInt(position5.z), dragRacingRoad);
		}

		private void HandleDespawning()
		{
			Vector3 position = lastRoadPart.position;
			int key = Mathf.FloorToInt(position.z - despawnDistance * roadPartLength);
			spawnedRoads.TryGetValue(key, out DragRacingRoad value);
			if (value != null)
			{
				if (startRoadInstance != null)
				{
					UnityEngine.Object.Destroy(startRoadInstance.gameObject);
				}
				spawnedRoads.Remove(key);
				roadPool.Enqueue(value);
			}
		}

		private void InstantiateFirstPart()
		{
			DragRacingRoad dragRacingRoad = roadPool.Dequeue();
			dragRacingRoad.transform.position = Vector3.zero;
			lastRoadPart = dragRacingRoad.transform;
			Dictionary<int, DragRacingRoad> dictionary = spawnedRoads;
			Vector3 position = lastRoadPart.position;
			dictionary.Add(Mathf.FloorToInt(position.z), dragRacingRoad);
			dragRacingRoad.transform.parent = roadParent;
		}

		public float GetRoadPartLength()
		{
			return roadPartLength;
		}

		public void SetRoadsToSpawnAmount(int amount)
		{
			roadsToSpawnAmount = amount;
		}

		public void UnloadRoad()
		{
			UnityEngine.Object.Destroy(roadParent.gameObject);
		}
	}
}
