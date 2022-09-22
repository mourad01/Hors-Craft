// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.AutoConnectionRailsVoxelEvents
using System;
using UnityEngine;

namespace Uniblocks
{
	public class AutoConnectionRailsVoxelEvents : AutoConnectionVoxelEvents
	{
		public bool isThatSwitch;

		public Voxel[] canSwitchTo;

		public Vector3[] inOutDirections;

		protected static void neighborIndexes(VoxelInfo info, out Index front, out Index right, out Index back, out Index left, int level = 0)
		{
			front = new Index(info.index.x, info.index.y + level, info.index.z + 1);
			right = new Index(info.index.x + 1, info.index.y + level, info.index.z);
			back = new Index(info.index.x, info.index.y + level, info.index.z - 1);
			left = new Index(info.index.x - 1, info.index.y + level, info.index.z);
		}

		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			if (CheckTopBottomVoxels(voxelInfo))
			{
				if (CheckIfNotLockedVoxel(voxelInfo))
				{
					for (int i = -1; i < 2; i++)
					{
						neighborIndexes(voxelInfo, out indexFront, out indexRight, out indexBack, out indexLeft, i);
						UpdateConnections(voxelInfo, indexFront, indexRight, indexBack, indexLeft);
					}
				}
				else
				{
					voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: false, 0);
				}
			}
			else
			{
				voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: false, 0);
			}
		}

		private bool CheckTopBottomVoxels(VoxelInfo voxelInfo)
		{
			for (int i = -2; i < 3; i++)
			{
				if (i != 0)
				{
					Index index = new Index(voxelInfo.index.x, voxelInfo.index.y + i, voxelInfo.index.z);
					Voxel voxelType = voxelInfo.chunk.GetVoxelType(index);
					AutoConnectionRailsVoxelEvents component = voxelType.GetComponent<AutoConnectionRailsVoxelEvents>();
					if (component != null)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override void OnBlockDestroy(VoxelInfo voxelInfo)
		{
			voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: false, 0);
			for (int i = -1; i < 2; i++)
			{
				neighborIndexes(voxelInfo, out indexFront, out indexRight, out indexBack, out indexLeft, i);
				UpdateConnections(voxelInfo, indexFront, indexRight, indexBack, indexLeft, updateSelf: false);
			}
		}

		private bool CheckIfNotLockedVoxel(VoxelInfo voxelInfo)
		{
			for (int i = -1; i < 2; i++)
			{
				neighborIndexes(voxelInfo, out indexFront, out indexRight, out indexBack, out indexLeft, i);
				ushort uniqueID = blockConnectionsID[blockConnectionsID.Length - 1].blockVoxelPrefab.GetUniqueID();
				if (voxelInfo.chunk.GetVoxel(indexFront) == uniqueID)
				{
					return false;
				}
				if (voxelInfo.chunk.GetVoxel(indexRight) == uniqueID)
				{
					return false;
				}
				if (voxelInfo.chunk.GetVoxel(indexBack) == uniqueID)
				{
					return false;
				}
				if (voxelInfo.chunk.GetVoxel(indexLeft) == uniqueID)
				{
					return false;
				}
			}
			return true;
		}

		public override void OnBlockRotate(VoxelInfo voxelInfo)
		{
			byte voxelRotation = voxelInfo.GetVoxelRotation();
			Voxel.ChangeBlock(voxelInfo, GetNextElement(voxelInfo.GetVoxelType().GetUniqueID()), voxelRotation);
		}

		public override void CheckNeighborConnection(VoxelInfo voxelInfo)
		{
			int num = 0;
			int num2 = 0;
			int level = 0;
			int bottomConnections = 0;
			int topConnections = 0;
			for (int i = -1; i < 2; i++)
			{
				int[] array = CheckLevel(voxelInfo, i);
				num += array[1];
				if (array[1] != 0)
				{
					level = i;
				}
				if (i == -1)
				{
					bottomConnections = array[2];
				}
				if (i == 0)
				{
					num2 = array[2];
				}
				if (i == 1)
				{
					topConnections = array[2];
				}
			}
			CreateConnection(voxelInfo, num, (byte)num2, level, topConnections, bottomConnections);
		}

		private int[] CheckLevel(VoxelInfo voxelInfo, int level)
		{
			int num = 0;
			int num2 = 0;
			neighborIndexes(voxelInfo, out indexFront, out indexRight, out indexBack, out indexLeft, level);
			for (int i = 0; i < blockConnectionsID.Length; i++)
			{
				if (voxelInfo.chunk.GetVoxel(indexFront) == blockConnectionsID[i].blockVoxelPrefab.GetUniqueID())
				{
					num2++;
					num += 4;
				}
				if (voxelInfo.chunk.GetVoxel(indexRight) == blockConnectionsID[i].blockVoxelPrefab.GetUniqueID())
				{
					num2++;
					num += 8;
				}
				if (voxelInfo.chunk.GetVoxel(indexBack) == blockConnectionsID[i].blockVoxelPrefab.GetUniqueID())
				{
					num2++;
					num++;
				}
				if (voxelInfo.chunk.GetVoxel(indexLeft) == blockConnectionsID[i].blockVoxelPrefab.GetUniqueID())
				{
					num2++;
					num += 2;
				}
			}
			return new int[3]
			{
				level,
				num2,
				num
			};
		}

		public override void CreateConnection(VoxelInfo voxelInfo, int connections, byte connectionId, int level = 0, int topConnections = 0, int bottomConnections = 0)
		{
			int num = connectionId + topConnections + bottomConnections;
			switch (connections)
			{
			case 0:
				if (level == 0)
				{
					Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections].blockVoxelPrefab.GetUniqueID(), connectionRotate[(byte)num]);
				}
				break;
			case 1:
				Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections].blockVoxelPrefab.GetUniqueID(), connectionRotate[(byte)num]);
				break;
			case 2:
				if (num == 5 || num == 10)
				{
					if (level != 0 && ((topConnections != 0 && connectionId != 0) || (bottomConnections != 0 && connectionId != 0) || (topConnections != 0 && bottomConnections != 0)))
					{
						byte rotation = (byte)((connectionRotate[(byte)topConnections] - 1) % 4);
						Voxel.ChangeBlock(voxelInfo, blockConnectionsID[blockConnectionsID.Length - 1].blockVoxelPrefab.GetUniqueID(), rotation);
					}
					else
					{
						Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections].blockVoxelPrefab.GetUniqueID(), connectionRotate[(byte)num]);
					}
				}
				else if ((level == 0 && topConnections == 0) || (level == -1 && topConnections == 0 && connectionId == 0))
				{
					Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections + 1].blockVoxelPrefab.GetUniqueID(), connectionRotate[(byte)num]);
				}
				break;
			case 3:
				if (topConnections == 0)
				{
					Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections + 1].blockVoxelPrefab.GetUniqueID(), connectionRotate[(byte)num]);
				}
				break;
			case 4:
				if (topConnections == 0)
				{
					Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections + 1].blockVoxelPrefab.GetUniqueID(), connectionRotate[(byte)num]);
				}
				break;
			}
		}

		public ushort GetNextElement(ushort voxelId)
		{
			int num = 0;
			if (canSwitchTo.Length > 1)
			{
				for (int i = 0; i < canSwitchTo.Length; i++)
				{
					if (canSwitchTo[i].GetUniqueID() == voxelId)
					{
						num = i;
					}
				}
				if (num > canSwitchTo.Length - 1 || num < 0)
				{
					throw new Exception("Invalid index");
				}
				num = ((num != canSwitchTo.Length - 1) ? (num + 1) : 0);
				return canSwitchTo[num].GetUniqueID();
			}
			return voxelId;
		}

		public Vector3[] GetInOutDirections(byte currentRotation)
		{
			return new Vector3[2]
			{
				Quaternion.AngleAxis(90 * currentRotation, Vector3.up) * inOutDirections[0],
				Quaternion.AngleAxis(90 * currentRotation, Vector3.up) * inOutDirections[1]
			};
		}
	}
}
