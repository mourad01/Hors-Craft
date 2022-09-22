// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.PlacedBlueprint
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class PlacedBlueprint
	{
		public HashSet<Vector3> placedVoxels;

		public PlacedBlueprintData placedBlueprintData;

		public int lastPlacedVoxelYOffset;

		public bool loaded;

		public float progress
		{
			get
			{
				if (placedBlueprintData == null)
				{
					return 1f;
				}
				return (float)placedVoxels.Count / (float)placedBlueprintData.blocksToFillInBlueprint;
			}
		}

		public int adsToFillBlueprint
		{
			get
			{
				if (Manager.Contains<TicketsManager>())
				{
					return 1;
				}
				return Manager.Get<ModelManager>().craftingSettings.GetBlueprintCost(placedBlueprintData.blocksToFillInBlueprint);
			}
		}

		public bool InBounds(Vector3 worldPosition, int offset = 0)
		{
			return placedBlueprintData.InBounds(worldPosition, offset);
		}
	}
}
