// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.AutoConnectionSlidePipeVoxelEvents
namespace Uniblocks
{
	public class AutoConnectionSlidePipeVoxelEvents : AutoConnectionRailsVoxelEvents
	{
		private int limit = 3;

		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			if (CanBePlaced(voxelInfo))
			{
				base.OnBlockPlace(voxelInfo);
			}
			else
			{
				Voxel.ChangeBlock(voxelInfo, 0, 0);
			}
		}

		protected bool CanBePlaced(VoxelInfo voxelInfo)
		{
			for (int i = -1; i < 2; i++)
			{
				AutoConnectionRailsVoxelEvents.neighborIndexes(voxelInfo, out indexFront, out indexRight, out indexBack, out indexLeft, i);
				ConnectedBlock[] blockConnectionsID = base.blockConnectionsID;
				for (int j = 0; j < blockConnectionsID.Length; j++)
				{
					ConnectedBlock connectedBlock = blockConnectionsID[j];
					if (voxelInfo.chunk.GetVoxel(indexFront) == connectedBlock.blockVoxelPrefab.GetUniqueID() && GetConnectionInfo(indexFront, voxelInfo) >= limit)
					{
						return false;
					}
					if (voxelInfo.chunk.GetVoxel(indexRight) == connectedBlock.blockVoxelPrefab.GetUniqueID() && GetConnectionInfo(indexRight, voxelInfo) >= limit)
					{
						return false;
					}
					if (voxelInfo.chunk.GetVoxel(indexBack) == connectedBlock.blockVoxelPrefab.GetUniqueID() && GetConnectionInfo(indexBack, voxelInfo) >= limit)
					{
						return false;
					}
					if (voxelInfo.chunk.GetVoxel(indexLeft) == connectedBlock.blockVoxelPrefab.GetUniqueID() && GetConnectionInfo(indexLeft, voxelInfo) >= limit)
					{
						return false;
					}
				}
			}
			return true;
		}

		protected int GetConnectionInfo(Index voxelIndex, VoxelInfo v)
		{
			VoxelInfo voxelInfo = v.chunk.GetVoxelInfo(voxelIndex);
			int num = 0;
			for (int i = -1; i < 2; i++)
			{
				AutoConnectionRailsVoxelEvents.neighborIndexes(voxelInfo, out Index front, out Index right, out Index back, out Index left, i);
				ConnectedBlock[] blockConnectionsID = base.blockConnectionsID;
				for (int j = 0; j < blockConnectionsID.Length; j++)
				{
					ConnectedBlock connectedBlock = blockConnectionsID[j];
					if (voxelInfo.chunk.GetVoxel(front) == connectedBlock.blockVoxelPrefab.GetUniqueID())
					{
						num++;
					}
					if (voxelInfo.chunk.GetVoxel(right) == connectedBlock.blockVoxelPrefab.GetUniqueID())
					{
						num++;
					}
					if (voxelInfo.chunk.GetVoxel(back) == connectedBlock.blockVoxelPrefab.GetUniqueID())
					{
						num++;
					}
					if (voxelInfo.chunk.GetVoxel(left) == connectedBlock.blockVoxelPrefab.GetUniqueID())
					{
						num++;
					}
				}
			}
			return num;
		}
	}
}
