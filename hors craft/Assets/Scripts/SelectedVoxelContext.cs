// DecompilerFi decompiler from Assembly-CSharp.dll class: SelectedVoxelContext
using System;
using Uniblocks;

public class SelectedVoxelContext : FactContext
{
	public VoxelInfo voxel;

	public Action onAdd;

	public Action onDig;

	public override string GetContent()
	{
		return base.GetContent() + ((voxel == null) ? string.Empty : ("voxel " + voxel.GetVoxel()));
	}
}
