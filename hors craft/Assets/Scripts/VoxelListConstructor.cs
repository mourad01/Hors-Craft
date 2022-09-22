// DecompilerFi decompiler from Assembly-CSharp.dll class: VoxelListConstructor
using Gameplay;
using System.Collections.Generic;
using UnityEngine;

public class VoxelListConstructor : ScrollableListConstructor
{
	public string category = string.Empty;

	public override ScrollableListRawContent ConstructList()
	{
		List<GameObject> voxels = GetVoxels();
		VoxelListRawContent voxelListRawContent = new VoxelListRawContent();
		voxelListRawContent.voxels = voxels;
		return voxelListRawContent;
	}

	private List<GameObject> GetVoxels()
	{
		return MonoBehaviourSingleton<BlocksCategories>.get.GetVoxels(category);
	}
}
