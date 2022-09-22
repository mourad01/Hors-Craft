// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.AutoConnectionVoxelEvents
using System.Collections.Generic;

namespace Uniblocks
{
	public class AutoConnectionVoxelEvents : DefaultVoxelEvents
	{
		public ConnectedBlock[] blockConnectionsID;

		public Dictionary<byte, byte> connectionRotate = new Dictionary<byte, byte>
		{
			{
				0,
				0
			},
			{
				8,
				0
			},
			{
				1,
				1
			},
			{
				2,
				2
			},
			{
				4,
				3
			},
			{
				6,
				2
			},
			{
				12,
				3
			},
			{
				9,
				0
			},
			{
				3,
				1
			},
			{
				10,
				0
			},
			{
				5,
				3
			},
			{
				13,
				0
			},
			{
				11,
				1
			},
			{
				7,
				2
			},
			{
				14,
				3
			},
			{
				15,
				0
			}
		};

		public const byte connectionIdVertical = 5;

		public const byte connectionIdHorizontal = 10;

		protected Index indexFront;

		protected Index indexRight;

		protected Index indexBack;

		protected Index indexLeft;

		private static void neighborIndexes(VoxelInfo info, out Index front, out Index right, out Index back, out Index left)
		{
			front = new Index(info.index.x, info.index.y, info.index.z + 1);
			right = new Index(info.index.x + 1, info.index.y, info.index.z);
			back = new Index(info.index.x, info.index.y, info.index.z - 1);
			left = new Index(info.index.x - 1, info.index.y, info.index.z);
		}

		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			neighborIndexes(voxelInfo, out indexFront, out indexRight, out indexBack, out indexLeft);
			UpdateConnections(voxelInfo, indexFront, indexRight, indexBack, indexLeft);
		}

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			neighborIndexes(chunk.GetVoxelInfo(x, y, z), out indexFront, out indexRight, out indexBack, out indexLeft);
			UpdateConnections(chunk.GetVoxelInfo(x, y, z), indexFront, indexRight, indexBack, indexLeft);
		}

		public override void OnBlockDestroy(VoxelInfo voxelInfo)
		{
			voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: false, 0);
			neighborIndexes(voxelInfo, out indexFront, out indexRight, out indexBack, out indexLeft);
			UpdateConnections(voxelInfo, indexFront, indexRight, indexBack, indexLeft, updateSelf: false);
		}

		public void UpdateConnections(VoxelInfo voxelInfo, Index front, Index right, Index back, Index left, bool updateSelf = true)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (updateSelf)
			{
				CheckNeighborConnection(voxelInfo);
			}
			ConnectedBlock[] array = blockConnectionsID;
			for (int i = 0; i < array.Length; i++)
			{
				ConnectedBlock connectedBlock = array[i];
				if (voxelInfo.chunk.GetVoxel(front) == connectedBlock.blockVoxelPrefab.GetUniqueID() && !flag)
				{
					CheckNeighborConnection(voxelInfo.chunk.GetVoxelInfo(front));
					flag = true;
				}
				if (voxelInfo.chunk.GetVoxel(right) == connectedBlock.blockVoxelPrefab.GetUniqueID() && !flag2)
				{
					CheckNeighborConnection(voxelInfo.chunk.GetVoxelInfo(right));
					flag2 = true;
				}
				if (voxelInfo.chunk.GetVoxel(back) == connectedBlock.blockVoxelPrefab.GetUniqueID() && !flag3)
				{
					CheckNeighborConnection(voxelInfo.chunk.GetVoxelInfo(back));
					flag3 = true;
				}
				if (voxelInfo.chunk.GetVoxel(left) == connectedBlock.blockVoxelPrefab.GetUniqueID() && !flag4)
				{
					CheckNeighborConnection(voxelInfo.chunk.GetVoxelInfo(left));
					flag4 = true;
				}
			}
		}

		public virtual void CheckNeighborConnection(VoxelInfo voxelInfo)
		{
			int num = 0;
			int num2 = 0;
			neighborIndexes(voxelInfo, out indexFront, out indexRight, out indexBack, out indexLeft);
			ConnectedBlock[] array = blockConnectionsID;
			for (int i = 0; i < array.Length; i++)
			{
				ConnectedBlock connectedBlock = array[i];
				if (voxelInfo.chunk.GetVoxel(indexFront) == connectedBlock.blockVoxelPrefab.GetUniqueID())
				{
					num++;
					num2 += 4;
				}
				if (voxelInfo.chunk.GetVoxel(indexRight) == connectedBlock.blockVoxelPrefab.GetUniqueID())
				{
					num++;
					num2 += 8;
				}
				if (voxelInfo.chunk.GetVoxel(indexBack) == connectedBlock.blockVoxelPrefab.GetUniqueID())
				{
					num++;
					num2++;
				}
				if (voxelInfo.chunk.GetVoxel(indexLeft) == connectedBlock.blockVoxelPrefab.GetUniqueID())
				{
					num++;
					num2 += 2;
				}
			}
			CreateConnection(voxelInfo, num, (byte)num2);
		}

		public virtual void CreateConnection(VoxelInfo voxelInfo, int connections, byte connectionId, int level = 0, int topConnections = 0, int bottomConnections = 0)
		{
			switch (connections)
			{
			case 0:
				Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections].blockVoxelPrefab.GetUniqueID(), connectionRotate[connectionId]);
				break;
			case 1:
				Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections].blockVoxelPrefab.GetUniqueID(), connectionRotate[connectionId]);
				break;
			case 2:
				if (connectionId == 5 || connectionId == 10)
				{
					Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections].blockVoxelPrefab.GetUniqueID(), connectionRotate[connectionId]);
				}
				else
				{
					Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections + 1].blockVoxelPrefab.GetUniqueID(), connectionRotate[connectionId]);
				}
				break;
			case 3:
				Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections + 1].blockVoxelPrefab.GetUniqueID(), connectionRotate[connectionId]);
				break;
			case 4:
				Voxel.ChangeBlock(voxelInfo, blockConnectionsID[connections + 1].blockVoxelPrefab.GetUniqueID(), connectionRotate[connectionId]);
				break;
			}
		}
	}
}
