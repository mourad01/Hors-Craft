// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.OneTimeBoatSpawnerEvents
using UnityEngine;

namespace Uniblocks
{
	public class OneTimeBoatSpawnerEvents : OneTimeSpawnerEvents
	{
		protected override void Spawn(VoxelInfo info)
		{
			Vector2[] array = new Vector2[5]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(-1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, -1f)
			};
			bool flag = false;
			Vector3 vector = Engine.VoxelInfoToPosition(info);
			ushort num = 0;
			Vector2[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Vector2 vector2 = array2[i];
				Vector3 vector3 = vector + vector2.x * Vector3.right + vector2.y * Vector3.forward;
				num = Engine.PositionToVoxelInfo(vector3).GetVoxel();
				if (IsWaterVoxel(num))
				{
					flag = true;
					vector = vector3;
					break;
				}
			}
			if (!flag)
			{
				bool flag2 = false;
				for (int j = -1; j <= 1; j++)
				{
					for (int k = -1; k <= 1; k++)
					{
						Vector3 vector4 = vector + j * Vector3.right + -1f * Vector3.up + k * Vector3.forward;
						num = Engine.PositionToVoxelInfo(vector4).GetVoxel();
						if (IsWaterVoxel(num))
						{
							flag = false;
							vector = vector4;
							flag2 = true;
						}
						if (flag2)
						{
							break;
						}
					}
					if (flag2)
					{
						break;
					}
				}
			}
			if (IsWaterVoxel(num) || GetComponent<Voxel>().isometricPlacment)
			{
				while (num == Engine.usefulIDs.waterBlockID)
				{
					vector += Vector3.up;
					num = Engine.PositionToVoxelInfo(vector).GetVoxel();
				}
				GameObject gameObject = Object.Instantiate(prefab);
				VehicleController componentInChildren = gameObject.GetComponentInChildren<VehicleController>();
				if (componentInChildren != null)
				{
					gameObject.GetComponentInChildren<VehicleController>().InitAfterSpawnByPlayer(ExampleInventory.HeldBlock);
				}
				gameObject.name = "alfa";
				vector.y += 0.6f;
				gameObject.transform.position = vector;
				info.chunk.SetVoxel(info.index, (ushort)(flag ? Engine.usefulIDs.waterBlockID : 0), updateMesh: true, 0);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Can't place block on non-water voxel");
			}
		}

		private bool IsWaterVoxel(ushort voxelId)
		{
			return Engine.usefulIDs.waterBlockID == voxelId;
		}
	}
}
