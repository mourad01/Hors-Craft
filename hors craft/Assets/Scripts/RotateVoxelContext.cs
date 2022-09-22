// DecompilerFi decompiler from Assembly-CSharp.dll class: RotateVoxelContext
using Uniblocks;

public class RotateVoxelContext : RotateContext
{
	public VoxelInfo info;

	public override string GetContent()
	{
		return base.GetContent() + "voxel: " + ((info == null) ? string.Empty : info.GetVoxel().ToString());
	}
}
