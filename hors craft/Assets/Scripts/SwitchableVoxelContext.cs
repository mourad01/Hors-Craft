// DecompilerFi decompiler from Assembly-CSharp.dll class: SwitchableVoxelContext
using Uniblocks;

public class SwitchableVoxelContext : InteractiveObjectContext
{
	public VoxelInfo voxelInfo;

	public override string GetContent()
	{
		return base.GetContent() + "voxel " + voxelInfo.GetVoxel();
	}
}
