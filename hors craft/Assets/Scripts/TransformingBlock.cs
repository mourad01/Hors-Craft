// DecompilerFi decompiler from Assembly-CSharp.dll class: TransformingBlock
using Uniblocks;
using UnityEngine;

public class TransformingBlock : DefaultVoxelEvents
{
	public Voxel changeForVoxel;

	public float possiblity;

	public override void OnBlockPlace(VoxelInfo voxelInfo)
	{
		if (Random.value > possiblity)
		{
			voxelInfo.chunk.SetVoxel(new Index(voxelInfo.index.x, voxelInfo.index.y, voxelInfo.index.z), voxelInfo.GetVoxel(), updateMesh: true, 0);
		}
		else
		{
			voxelInfo.chunk.SetVoxel(new Index(voxelInfo.index.x, voxelInfo.index.y, voxelInfo.index.z), changeForVoxel.GetUniqueID(), updateMesh: true, 0);
		}
	}
}
