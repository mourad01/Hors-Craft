// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.GrowerVoxelEvents
using Common.Managers;
using Gameplay;

namespace Uniblocks
{
	public class GrowerVoxelEvents : DefaultVoxelEvents
	{
		public override void OnBlockPlace(VoxelInfo info)
		{
			base.OnBlockPlace(info);
			Manager.Get<FarmingManager>()?.Placed(info);
		}

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			base.OnBlockRebuilded(chunk, x, y, z);
			VoxelInfo voxelInfo = chunk.GetVoxelInfo(x, y, z);
			Manager.Get<FarmingManager>()?.RebuildPlaced(voxelInfo);
		}

		public override void OnBlockDestroy(VoxelInfo voxelInfo)
		{
			Manager.Get<FarmingManager>()?.Destroyed(voxelInfo);
			base.OnBlockDestroy(voxelInfo);
		}
	}
}
