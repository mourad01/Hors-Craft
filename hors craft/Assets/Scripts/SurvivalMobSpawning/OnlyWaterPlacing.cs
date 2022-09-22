// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.OnlyWaterPlacing
using Common.Utils;
using Uniblocks;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class OnlyWaterPlacing : AbstractSpawnPlacingBehaviour
	{
		protected override ushort placingBlockId => Engine.usefulIDs.waterBlockID;

		private void Start()
		{
			spawnRadiusMin = 15f;
			spawnRadiusMax = 80f;
		}

		protected override Vector3 GetRandomSpawnPosition(GameObject player)
		{
			Vector3 randomSpawnPosition = base.GetRandomSpawnPosition(player);
			return randomSpawnPosition.WithY(Engine.TerrainGenerator.waterHeight + 2);
		}

		protected override bool MonsterFitsInPosition(GameObject monster, Vector3 position)
		{
			Bounds bounds = RenderersBounds.MaximumBounds(monster);
			Vector3 extents = bounds.extents;
			int num = Mathf.CeilToInt(extents.x);
			Vector3 extents2 = bounds.extents;
			int num2 = Mathf.CeilToInt(extents2.y) - 1;
			Vector3 extents3 = bounds.extents;
			int num3 = Mathf.CeilToInt(extents3.z);
			for (float num4 = position.x - (float)num; num4 <= position.x + (float)num; num4 += 1f)
			{
				for (float num5 = position.y - (float)num2; num5 <= position.y + (float)num2; num5 += 1f)
				{
					for (float num6 = position.z - (float)num3; num6 <= position.z + (float)num3; num6 += 1f)
					{
						VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(new Vector3(num4, num5, num6));
						if (voxelInfo.GetVoxel() != 0 && voxelInfo.GetVoxel() != Engine.usefulIDs.waterBlockID)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}
