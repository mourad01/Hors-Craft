// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.PlacedBlueprintData
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class PlacedBlueprintData
	{
		public List<Vector3> placedVoxelsList = new List<Vector3>();

		public int blocksToFillInBlueprint;

		public Vector3 position;

		public int rotation;

		public string dataName;

		public int craftableId;

		public Vector3 min;

		public Vector3 max;

		public Vector3 center;

		public bool built;

		public float instantFiledProgress;

		public bool InBounds(Vector3 worldPosition, int offset = 0)
		{
			Vector3 localPosition = GetLocalPosition(worldPosition);
			if (localPosition.x >= min.x - (float)offset && localPosition.x <= max.x + (float)offset && localPosition.y >= min.y - (float)offset && localPosition.y <= max.y + (float)offset && localPosition.z >= min.z - (float)offset && localPosition.z <= max.z + (float)offset)
			{
				return true;
			}
			return false;
		}

		public Vector3 GetLocalPosition(Vector3 worldPosition)
		{
			Vector3 vector = worldPosition - position;
			for (int num = rotation; num > 0; num--)
			{
				vector = vector.Rotate90left();
			}
			return vector + center;
		}

		public Vector3 GetWorldPosition(Vector3 localPosition)
		{
			Vector3 vector = localPosition - center;
			for (int num = rotation; num > 0; num--)
			{
				vector = vector.Rotate90right();
			}
			return vector + position;
		}
	}
}
