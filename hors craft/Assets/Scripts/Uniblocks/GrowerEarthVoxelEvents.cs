// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.GrowerEarthVoxelEvents
using Common.Managers;
using Gameplay;

namespace Uniblocks
{
	public class GrowerEarthVoxelEvents : GrowerVoxelEvents
	{
		public override void OnBlockPlace(VoxelInfo voxelInfo, ushort previousVoxel)
		{
			if (Manager.Contains<FarmingManager>() && Manager.Get<FarmingManager>().TryBuy(voxelInfo))
			{
				OnBlockPlace(voxelInfo);
				Index index = new Index(voxelInfo.index.x, voxelInfo.index.y - 1, voxelInfo.index.z);
				VoxelInfo voxelInfo2 = voxelInfo.chunk.GetVoxelInfo(index);
				voxelInfo2.SetVoxel(voxelInfo.GetVoxel(), updateMesh: true, 0);
				voxelInfo.SetVoxel(0, updateMesh: true, 0);
			}
			else
			{
				voxelInfo.SetVoxel(previousVoxel, updateMesh: true, 0);
			}
		}

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			if (Manager.Contains<FarmingManager>())
			{
				VoxelInfo voxelInfo = chunk.GetVoxelInfo(x, y, z);
				VoxelInfo voxelInfo2 = chunk.GetVoxelInfo(x, y + 1, z);
				Manager.Get<FarmingManager>().RebuildEarth(voxelInfo, voxelInfo2);
			}
		}

		public override void OnBlockDestroy(VoxelInfo voxelInfo)
		{
			if (Manager.Contains<FarmingManager>())
			{
				Index index = new Index(voxelInfo.index);
				index.y++;
				VoxelInfo voxelInfo2 = voxelInfo.chunk.GetVoxelInfo(index);
				Manager.Get<FarmingManager>().Destroyed(voxelInfo2);
			}
			base.OnBlockDestroy(voxelInfo);
		}
	}
}
