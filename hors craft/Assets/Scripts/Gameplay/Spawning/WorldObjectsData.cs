// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Spawning.WorldObjectsData
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Spawning
{
	[Serializable]
	public class WorldObjectsData
	{
		public int worldId;

		public List<GamePlayObjectBase> resources = new List<GamePlayObjectBase>();

		public List<GamePlayDoorObject> doors = new List<GamePlayDoorObject>();

		public void AddDoorObject(GamePlayDoorObject newObject)
		{
			if (!doors.Contains(newObject))
			{
				doors.Add(newObject);
			}
		}

		public void RemoveDoorObject(GamePlayDoorObject newObject)
		{
			doors.Remove(newObject);
		}

		public void AddResource(GamePlayObjectBase newResource)
		{
			if (!resources.Contains(newResource))
			{
				resources.Add(newResource);
			}
		}

		public void SpawnAllResources(Transform parent = null)
		{
			if (resources != null)
			{
				foreach (GamePlayObjectBase resource in resources)
				{
					resource.Spawn(parent);
				}
			}
		}

		public GamePlayObjectBase GetResourceData(GameObject resourceInstance)
		{
			if (resources == null)
			{
				return null;
			}
			for (int i = 0; i < resources.Count; i++)
			{
				if (resourceInstance == resources[i].objectInstance)
				{
					return resources[i];
				}
			}
			return null;
		}

		public GamePlayDoorObject GetDoorData(Vector3 doorPosition)
		{
			if (doors == null)
			{
				return null;
			}
			for (int i = 0; i < doors.Count; i++)
			{
				if (Vector3.Distance(doors[i].position, doorPosition) < 0.1f)
				{
					return doors[i];
				}
			}
			return null;
		}
	}
}
