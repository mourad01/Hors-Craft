// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveVoxelContext
using System;
using Uniblocks;

public class InteractiveVoxelContext : FactContext
{
	public VoxelInfo info;

	public Action useAction;

	public override string GetContent()
	{
		return base.ToString() + ((info == null) ? string.Empty : ("voxel: " + info.GetVoxel() + " global index " + info.GetGlobalIndex()));
	}
}
