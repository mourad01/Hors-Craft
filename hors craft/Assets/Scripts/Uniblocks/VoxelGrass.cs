// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelGrass
namespace Uniblocks
{
	public class VoxelGrass : DefaultVoxelEvents
	{
		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			Index index = new Index(voxelInfo.index.x, voxelInfo.index.y - 1, voxelInfo.index.z);
			if (voxelInfo.GetVoxelType().VTransparency == Transparency.solid && voxelInfo.chunk.GetVoxel(index) == Engine.usefulIDs.grassBlockID)
			{
				voxelInfo.chunk.SetVoxel(index, Engine.usefulIDs.dirtBlockID, updateMesh: true, 0);
			}
		}
	}
}
