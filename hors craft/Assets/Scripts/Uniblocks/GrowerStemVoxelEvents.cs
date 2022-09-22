// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.GrowerStemVoxelEvents
namespace Uniblocks
{
	public class GrowerStemVoxelEvents : GrowerVoxelEvents
	{
		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			base.OnBlockPlace(voxelInfo);
			VoxelInfo voxelInfo2 = voxelInfo.chunk.GetVoxelInfo(voxelInfo.index.x, voxelInfo.index.y - 1, voxelInfo.index.z);
			voxelInfo2.SetVoxel(Engine.usefulIDs.dirtBlockID, updateMesh: true, 0);
		}
	}
}
